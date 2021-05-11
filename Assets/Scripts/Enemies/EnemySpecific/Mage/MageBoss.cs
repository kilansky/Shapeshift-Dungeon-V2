using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.VFX;

public class MageBoss : MonoBehaviour, IDamageable
{
    //-----------Public-----------
    [Header("Health & Stun")]
    public float startingHealth;
    public float dmgInvincibilityTime = 0.25f;
    public float stunResistance;
    public float stunRecoveryTime;
    public Material hitMat;
    public Material normalMat;

    [Header("Attacking")]
    public float sightRange = 20f;
    public float minAttackRange;
    public float timeBetweenAttacks = 3f;
    public float projectileChargeTime = 0.75f;
    public float attack2RotateSpeed = 5f;
    public GameObject magicProjectile;

    [Header("Teleporting")]
    public float timeToTeleportAgain = 12f;
    public GameObject teleportParticles;
    //public float minTeleportRange = 18f;
    //public float maxTeleportRange = 25f;
    //public float minTeleportRangeFromPlayer = 18f;
    //public float maxTeleportRangeFromPlayer = 25f;

    [Header("Phase Changes")]
    public float phaseChange1 = 0.75f;
    public float phaseChange2 = 0.4f;
    public float phaseChange3 = 0.1f;

    [Header("Fire Points")]
    public Transform[] firePoints1;
    public Transform[] firePoints2;
    public Transform firePoints2Root;

    [Header("Object References")]
    public GameObject aliveMage;
    public GameObject deathEffect;
    public GameObject phase2Map;
    public GameObject phase3Map;
    public GameObject phase4Map;

    public float Health { get; set; }

    [HideInInspector] public List<GameObject> unfiredProjectiles = new List<GameObject>();
    [HideInInspector] public float distanceToPlayer;
    [HideInInspector] public bool isStunned = false;
    [HideInInspector] public bool isInvincible = false;

    //-----------Private-----------
    private Slider healthBar;
    private GameObject player;
    private Animator animator;
    private Renderer mageRenderer;
    private float currentStunResistance;
    private float stunTimeRemaining;
    private float lastTimeAttacked;
    private Quaternion lastTargetRotation;
    private List<Transform> teleportPoints = new List<Transform>();
    private List<Transform> furthest3Points = new List<Transform>();
    private Transform currentTeleportPoint;
    private Transform previousTeleportPoint;
    private Vector3 hiddenTeleportPoint = new Vector3(0, 5, -42); //Teleport out of bounds during phase changes
    private float timeTillNextTeleport;
    private float changeToUseAttack2;

    private bool isAttacking = false;
    private bool isSpawningMonsters = false;
    private bool rotateTowardsPlayer = true;
    private bool stopMoving = false;

    private bool phase1Complete = false;
    private bool phase2Complete = false;
    private bool phase3Complete = false;

    [HideInInspector] public bool startBossFight = false;

    public virtual void Start()
    {
        player = FindObjectOfType<PlayerController>().gameObject;
        distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        animator = aliveMage.GetComponent<Animator>();
        mageRenderer = aliveMage.GetComponent<Renderer>();

        currentStunResistance = stunResistance;
        lastTargetRotation = Quaternion.identity;

        //set health variables
        Health = startingHealth;
        healthBar = HUDController.Instance.bossHealthBar;
        HUDController.Instance.ShowBossHealthBar();
        healthBar.maxValue = Health;
        healthBar.value = Health;

        changeToUseAttack2 = 0.2f;

        isStunned = true;
        InvokeRepeating("CheckToStartAttacking", 0, 0.25f);
    }

    private void Update()
    {
        //if the enemy has not been hit recently, reset resistance to stuns
        if (Time.time >= lastTimeAttacked + stunRecoveryTime)
            ResetStunResistance();

        Rotate();
        //Debug.Log("timeTillNextTeleport is: " + timeTillNextTeleport);

        if (isAttacking)
            firePoints2Root.Rotate(0f, 10f * attack2RotateSpeed * Time.deltaTime, 0f);

        if(startBossFight)
        {
            startBossFight = false;
            isStunned = false;

            transform.parent = null;

            //Set teleport points and teleport to a point within the map
            GetAllTeleportPoints();
            Teleport();
            StartCoroutine(WaitToTeleport());
        }
    }

    private void CheckToStartAttacking()
    {
        if (!isAttacking && !isStunned && timeTillNextTeleport > 2f)
        {
            if(timeTillNextTeleport < 4f)
                StartCoroutine(Attack1());
            else
            {
                float randAttackNum = Random.value;

                if (randAttackNum < changeToUseAttack2)
                    StartCoroutine(Attack2());
                else
                    StartCoroutine(Attack1());
            }
        }
    }

    //================================================================
    //------------------------Player Detection------------------------
    //================================================================

    private void Rotate()
    {
        if (!rotateTowardsPlayer)
            return;

        //Get direction to player
        Vector3 targetPoint = new Vector3(player.transform.position.x, 0, player.transform.position.z) - new Vector3(aliveMage.transform.position.x, 0, aliveMage.transform.position.z);
        Quaternion targetRotation = Quaternion.LookRotation(targetPoint);
        float rotSpeed = 5f;

        //Smoothly rotate towards player
        lastTargetRotation = Quaternion.Slerp(aliveMage.transform.rotation, targetRotation, rotSpeed * Time.deltaTime);
        aliveMage.transform.rotation = lastTargetRotation;
    }

    public bool CheckPlayerInMinAttackRange()
    {
        distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        return (distanceToPlayer < minAttackRange) ? true : false;
    }

    //================================================================
    //----------------------------Attacking---------------------------
    //================================================================

    public IEnumerator Attack1()
    {
        isAttacking = true;

        //Debug.Log("Attack1 Started");

        //Charge up and fire a projectile from each fire point
        foreach (Transform firePoint in firePoints1)
            StartCoroutine(ChargeAttack1(magicProjectile, firePoint, projectileChargeTime));

        float timeElapsed = 0f;
        while (timeElapsed < timeBetweenAttacks)
        {
            if (!isAttacking)
                break;

            timeElapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        isAttacking = false;
    }

    IEnumerator ChargeAttack1(GameObject fireball, Transform firePoint, float chargeTime)
    {
        GameObject bullet = Instantiate(fireball, firePoint.transform.position, firePoint.transform.rotation, firePoint.transform);
        Vector3 originalScale = bullet.transform.localScale;
        float vfxPercent = 0;

        //set movespeed to 0
        bullet.GetComponent<Bullet>().moveSpeed = 0f;
        //Sets the bullets parent game object to this one to make the Damage Tracker acquire the skull as the correct game object for tracking
        bullet.GetComponent<Bullet>().parentObject = gameObject;

        bullet.transform.GetChild(0).gameObject.SetActive(false);
        yield return new WaitForEndOfFrame();
        //set scale of bullet to .1 scale up, doesn't scale vfx
        bullet.transform.GetChild(0).gameObject.SetActive(true);
        bullet.transform.localScale = Vector3.one * .1f;

        //disable inner collider so it doesn't destroy while charging
        bullet.GetComponent<CapsuleCollider>().enabled = false;

        float timeElapsed = 0f; //Counter to keep track of time elapsed
        while (timeElapsed < chargeTime) //This while loop scales object over time
        {
            if (isStunned || timeTillNextTeleport <= 0)
            {
                Destroy(bullet);
                //myAnim.SetBool("isAttacking", false);
                yield break;
            }

            timeElapsed += Time.deltaTime;
            bullet.transform.localScale = Vector3.Lerp(bullet.transform.localScale, originalScale, timeElapsed / chargeTime);
            vfxPercent = Mathf.Lerp(1f, 100f, timeElapsed / chargeTime);
            bullet.GetComponent<Bullet>().SetVFXScale(vfxPercent);

            yield return new WaitForEndOfFrame();
        }
        Destroy(bullet);

        //Spawn new projectile in the direction of the player
        bullet = Instantiate(fireball, firePoint.transform.position, firePoint.transform.rotation);

        bullet.transform.localScale = originalScale;
        bullet.GetComponent<Bullet>().parentObject = gameObject;
        bullet.GetComponent<Bullet>().canDamage = true;
        //sparks.Play();

        GetComponent<AudioSource>().Play();
    }

    public IEnumerator Attack2()
    {
        isAttacking = true;
        timeTillNextTeleport += 4f;

        //Charge up a ring of projectiles
        foreach (Transform firePoint in firePoints2)
        {
            if (!isAttacking || isStunned || timeTillNextTeleport <= 0)
                break;

            StartCoroutine(ChargeAttack2(magicProjectile, firePoint, projectileChargeTime));
            yield return new WaitForSeconds(0.25f);
        }

        if (isAttacking)
            yield return new WaitForSeconds(1f);

        //Fire each projectile at the player
        foreach (Transform firePoint in firePoints2)
        {
            if (!isAttacking || isStunned || timeTillNextTeleport <= 0)
                break;

            Attack2FireProjectile(magicProjectile, firePoint, projectileChargeTime);
            yield return new WaitForSeconds(0.5f);
        }

        isAttacking = false;
    }

    IEnumerator ChargeAttack2(GameObject fireball, Transform firePoint, float chargeTime)
    {
        GameObject bullet = Instantiate(fireball, firePoint.transform.position, firePoint.transform.rotation, firePoint.transform);
        Vector3 originalScale = bullet.transform.localScale;
        float vfxPercent = 0;

        bullet.GetComponent<Bullet>().moveSpeed = 0f;
        bullet.GetComponent<Bullet>().canBeDestroyed = false;
        bullet.GetComponent<Bullet>().parentObject = gameObject;
    
        bullet.transform.GetChild(0).gameObject.SetActive(false);
        yield return new WaitForEndOfFrame();

        //set scale of bullet to .1 scale up, doesn't scale vfx
        bullet.transform.GetChild(0).gameObject.SetActive(true);
        bullet.transform.localScale = Vector3.one * .1f;

        //disable inner collider so it doesn't destroy while charging
        bullet.GetComponent<CapsuleCollider>().enabled = false;

        float timeElapsed = 0f; //Counter to keep track of time elapsed
        while (timeElapsed < chargeTime) //This while loop scales object over time
        {
            if (isStunned || timeTillNextTeleport <= 0)
            {
                Destroy(bullet);
                //myAnim.SetBool("isAttacking", false);
                yield break;
            }

            timeElapsed += Time.deltaTime;
            bullet.transform.localScale = Vector3.Lerp(bullet.transform.localScale, originalScale, timeElapsed / chargeTime);
            vfxPercent = Mathf.Lerp(1f, 100f, timeElapsed / chargeTime);
            bullet.GetComponent<Bullet>().SetVFXScale(vfxPercent);

            yield return new WaitForEndOfFrame();
        }

        bullet.GetComponent<Bullet>().canDamage = true;
        unfiredProjectiles.Add(bullet);
    }

    private void Attack2FireProjectile(GameObject fireball, Transform firePoint, float chargeTime)
    {
        if (unfiredProjectiles.Count == 0)
            return;

        int randIndex = Random.Range(0, unfiredProjectiles.Count);
        GameObject bullet = unfiredProjectiles[randIndex];
        Vector3 bulletPos = bullet.transform.position;
        unfiredProjectiles.Remove(unfiredProjectiles[randIndex]);

        Destroy(bullet);

        //Spawn new projectiles in the direction of the player
        Vector3 targetPoint = new Vector3(player.transform.position.x, player.transform.position.y + 1, player.transform.position.z) - bulletPos;
        Quaternion targetRotation = Quaternion.LookRotation(targetPoint);
        bullet = Instantiate(fireball, bulletPos, targetRotation);

        //Sets the bullets parent game object to this one to make the Damage Tracker acquire the skull as the correct game object for tracking
        bullet.GetComponent<Bullet>().parentObject = gameObject;
        bullet.GetComponent<Bullet>().canDamage = true;
        //sparks.Play();

        GetComponent<AudioSource>().Play();
    }

    /*
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, minTeleportRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, maxTeleportRange);
    }
    */

    //================================================================
    //------------------------Monster Spawning------------------------
    //================================================================

    /*
    //Spawns a single monster, waits, and is called again recursively until all monsters have been killed
    private void SpawnMonsters()
    {
        isSpawningMonsters = true;

        //Only spawn if the # of monsters in the room is less than maxMonsters
        if (monstersInRoom < currFloorInfo.maxMonsters && monsterSpawnPoints.Count > 0)
        {
            //Get a random spawn point index
            int randSpawnPoint = Random.Range(0, monsterSpawnPoints.Count);

            //Get a random monster to spawn
            //int randMonster = Random.Range(0, currFloorInfo.monsters.Length);
            GameObject monsterToSpawn = currFloorInfo.GetMonsterToSpawn();

            //Spawn the monster and disable the spawn point temporarily
            monsterSpawnPoints[randSpawnPoint].SpawnMonster(monsterToSpawn, CheckForGem());
            monstersInRoom++;
            monstersSpawned++;
            StartCoroutine(DisableSpawner(randSpawnPoint));
        }

        //Recursively attempt to spawn until the total # of monsters to spawn have been killed
        if (monstersSpawned < currFloorInfo.totalMonsters)
            StartCoroutine(WaitToSpawnAgain());
        else
            isSpawningMonsters = false;
    }
    */

    //================================================================
    //---------------------------Teleporting--------------------------
    //================================================================

    public void Teleport()
    {
        isAttacking = false;
        stunTimeRemaining = 0;

        GameObject vfx = Instantiate(teleportParticles, transform.position + new Vector3(0, 1.5f, 0), Quaternion.identity);
        Destroy(vfx, 2f);

        //Check to destroy attack2 projectiles if left unfired before teleporting
        if(unfiredProjectiles.Count > 0)
        {
            foreach (GameObject projectile in unfiredProjectiles)
                Destroy(projectile, 0.1f);

            unfiredProjectiles.Clear();
        }

        //---------------------------Phase 1---------------------------
        if (!phase1Complete)
        {
            Transform newTeleportPoint = GetPhase1TeleportPoint();

            if (newTeleportPoint)
            {
                transform.position = newTeleportPoint.position;

                previousTeleportPoint = currentTeleportPoint;
                currentTeleportPoint = newTeleportPoint;
            }
            else
                Debug.LogError("Mage Attempted To Teleport But Failed");
        }
        //---------------------------Phase 2---------------------------
        else if (!phase2Complete)
        {
            if (teleportPoints.Count == 0)
                GetAllTeleportPoints();

            int randTeleportPoint = Random.Range(0, teleportPoints.Count);
            transform.position = teleportPoints[randTeleportPoint].position;

            //Add the last teleport point back into the list        
            if (currentTeleportPoint)
                teleportPoints.Add(currentTeleportPoint);
            //then set the new current teleport point and remove it from the list
            currentTeleportPoint = teleportPoints[randTeleportPoint];
            teleportPoints.Remove(currentTeleportPoint);
        }
        //---------------------------Phase 3---------------------------
        else if (!phase3Complete)
        {
            if (teleportPoints.Count == 0)
                GetAllTeleportPoints();

            int randTeleportPoint = Random.Range(0, teleportPoints.Count);
            transform.position = teleportPoints[randTeleportPoint].position;

            //Add the last 2 teleport points back into the list        
            if (currentTeleportPoint)
                teleportPoints.Add(currentTeleportPoint);

            if (previousTeleportPoint)
                teleportPoints.Add(previousTeleportPoint);

            //then set the new current teleport point and remove it from the list
            previousTeleportPoint = currentTeleportPoint;
            currentTeleportPoint = teleportPoints[randTeleportPoint];
            teleportPoints.Remove(currentTeleportPoint);
            teleportPoints.Remove(previousTeleportPoint);
        }
        //---------------------------Phase 4---------------------------
        else
        {

        }

        GameObject vfx2 = Instantiate(teleportParticles, transform.position + new Vector3(0, 1.5f, 0), Quaternion.identity);
        Destroy(vfx2, 2f);
    }

    //Gets the 3 furthest points furthest from the mage's current position
    private Transform GetPhase1TeleportPoint()
    {       
        teleportPoints.Clear();
        furthest3Points.Clear();

        GetAllTeleportPoints();

        //Repeat getting the point that is furthest from the mage 3 times
        for (int i = 0; i < 3; i++)
        {
            //Get the point furthest from the mage
            Transform furthestPoint = null;
            float furthestDist = 0f;
            foreach (Transform point in teleportPoints)
            {
                float pointDist = Vector3.Distance(transform.position, point.position);
                if (pointDist > furthestDist)
                {
                    furthestPoint = point;
                    furthestDist = pointDist;
                }
            }
            //Add the point that is furthest to the furthest3Points list, and remove it from the teleportPoints list
            furthest3Points.Add(furthestPoint);
            teleportPoints.Remove(furthestPoint);
        }

        if (previousTeleportPoint)
            furthest3Points.Remove(previousTeleportPoint);

        int randTeleportPoint = Random.Range(0, furthest3Points.Count);
        return furthest3Points[randTeleportPoint];
    }

    private void GetAllTeleportPoints()
    {
        foreach (BossTeleportPoint teleportPoint in GameObject.FindObjectsOfType<BossTeleportPoint>())
            teleportPoints.Add(teleportPoint.transform);
    }

    private void DestroyOldTeleportPoints()
    {
        foreach (BossTeleportPoint teleportPoint in GameObject.FindObjectsOfType<BossTeleportPoint>())
            teleportPoint.DestroyTeleportPoint();
    }

    private IEnumerator WaitToTeleport()
    {
        while(timeTillNextTeleport > 0)
        {
            timeTillNextTeleport -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        timeTillNextTeleport = timeToTeleportAgain;

        Teleport();
        StartCoroutine(WaitToTeleport());
    }

    private void PhaseChangeTeleport()
    {
        isStunned = true;
        GameObject vfx = Instantiate(teleportParticles, transform.position + new Vector3(0, 1.5f, 0), Quaternion.identity);
        Destroy(vfx, 2f);

        transform.position = hiddenTeleportPoint;
    }

    //================================================================
    //---------------------Phases & Shapeshifting---------------------
    //================================================================

    private void CheckForPhaseChange()
    {
        if(!phase1Complete && Health / startingHealth < phaseChange1)
        {
            phase1Complete = true;
            //Debug.Log("Start Phase 2");
            StartCoroutine(StartPhase2());
        }
        else if (!phase2Complete && Health / startingHealth < phaseChange2)
        {
            phase2Complete = true;
            //Debug.Log("Start Phase 3");
            StartCoroutine(StartPhase3());
        }
        else if (!phase3Complete && Health / startingHealth < phaseChange3)
        {
            phase3Complete = true;
            //Debug.Log("Start Phase 4");
        }
    }

    private IEnumerator StartPhase2()
    {
        //Disable Natural Spawning
        LevelManager.Instance.disableSpawning = true;

        //Clear phase 1 teleport points
        DestroyOldTeleportPoints();
        teleportPoints.Clear();
        furthest3Points.Clear();
        previousTeleportPoint = null;
        currentTeleportPoint = null;
        StopCoroutine(WaitToTeleport());

        //Teleport away to hidden point
        PhaseChangeTeleport();
        timeToTeleportAgain = 16f;
        timeTillNextTeleport = 16f;

        //Alert player of dangerous tiles
        foreach (DangerTiles dangerousTile in GameObject.FindObjectsOfType<DangerTiles>())
            dangerousTile.StartFlashing();
        CineShake.Instance.Shake(1.5f, 2.5f);
        CameraController.Instance.ZoomOutLevelTransition();
        AudioManager.Instance.Play("Rumble");
        yield return new WaitForSeconds(2.5f);

        //Transition Level
        LevelManager.Instance.ChangeBossLevel(phase2Map);
        timeToTeleportAgain = 16f;
        timeTillNextTeleport = 16f;
        changeToUseAttack2 = 0.4f;
    }

    private IEnumerator StartPhase3()
    {
        //Clear phase 2 teleport points
        DestroyOldTeleportPoints();
        teleportPoints.Clear();
        furthest3Points.Clear();
        previousTeleportPoint = null;
        currentTeleportPoint = null;
        StopCoroutine(WaitToTeleport());

        //Teleport away to hidden point
        PhaseChangeTeleport();
        timeToTeleportAgain = 16f;
        timeTillNextTeleport = 16f;

        //Alert player of dangerous tiles
        foreach (DangerTiles dangerousTile in GameObject.FindObjectsOfType<DangerTiles>())
            dangerousTile.StartFlashing();
        CineShake.Instance.Shake(1.5f, 2.5f);
        CameraController.Instance.ZoomOutLevelTransition();
        AudioManager.Instance.Play("Rumble");
        yield return new WaitForSeconds(2.5f);

        //Transition Level
        LevelManager.Instance.ChangeBossLevel(phase3Map);
        timeToTeleportAgain = 16f;
        timeTillNextTeleport = 16f;
        changeToUseAttack2 = 0.7f;
    }


    //================================================================
    //-------------------------Health & Damage------------------------
    //================================================================

    public void Damage(float damage)
    {
        //start timer
        lastTimeAttacked = Time.time;
        //TODO: check what player damage variable is called, and script name
        //subtract the players damage from the enemies stun resistance


        if (!isInvincible)
        {
            //enemy takes damage from the player
            Health -= damage;
            Flash();
            UpdateHealthUI();
            //Debug.Log("Enemy Took Damage");

            //When the enemy takes enough damage and is killed it will do the kill function then the player kapala item special item charge function from player controller - AHL (4/20/21)
            if (Health <= 0)
                Kill();

            //Reduce stun resistance and check if stunned
            currentStunResistance -= damage;
            //Debug.Log("stun resistance is set to: " + currentStunResistance);
            if (isStunned)
                stunTimeRemaining = Mathf.Clamp(stunTimeRemaining + 0.5f, 0, stunRecoveryTime);
            else if (!isStunned && currentStunResistance <= 0)
            {
                isStunned = true;
                isAttacking = false;
                StartCoroutine(ResetStunResistance());
            }

            //prevent from taking damage temporarily
            StartCoroutine(InvincibilityFrames());

            //reduce the time before teleporting
            timeTillNextTeleport -= (damage / 2);

            CheckForPhaseChange();
        }
    }

    public void FireDamage(float damage)
    {
        Flash();

        //enemy takes damage from the player
        Health -= damage;
        UpdateHealthUI();

        //When the enemy takes enough damage and is killed it will do the kill function then the player kapala item special item charge function from player controller - AHL (4/20/21)
        if (Health <= 0)
            Kill();
    }

    public void Heal(float heal)
    {
        //heal the enemy
        Health = Mathf.Clamp(Health + heal, 0, healthBar.maxValue);
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        healthBar.value = Health;
    }

    public void Kill()
    {
        AudioManager.Instance.Play("BigBell");
        CineShake.Instance.Shake(1f, 2f);
        MusicManager.Instance.FloorCleared();

        Instantiate(deathEffect, transform.position + new Vector3(0, 2, 0), Quaternion.identity);
        HUDController.Instance.HideBossHealthBar();

        RunTimer.Instance.IncreaseTimer = false;
        HUDController.Instance.ShowWinScreen();

        //Destroy self
        Destroy(gameObject);
    }

    //function for enemies ability to resist stuns
    public IEnumerator ResetStunResistance()
    {
        isAttacking = false;

        //Check to destroy attack2 projectiles
        if (unfiredProjectiles.Count > 0)
        {
            foreach (GameObject projectile in unfiredProjectiles)
                Destroy(projectile, 0.1f);

            unfiredProjectiles.Clear();
        }

        stunTimeRemaining = stunRecoveryTime;
        while (stunTimeRemaining > 0)
        {
            stunTimeRemaining -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        isStunned = false;
        currentStunResistance = stunResistance;
    }

    //Makes the enemy invincible briefly
    public IEnumerator InvincibilityFrames()
    {
        isInvincible = true;
        yield return new WaitForSeconds(dmgInvincibilityTime);
        isInvincible = false;
    }

    public void Flash()
    {
        //sets enemy's color to the hitMat (red)
        mageRenderer.material = hitMat;
        StartCoroutine(WaitToResetColor());
    }

    public IEnumerator WaitToResetColor()
    {
        //reset enemy's color to normalMat
        yield return new WaitForSeconds(dmgInvincibilityTime);
        mageRenderer.material = normalMat;
    }
}
