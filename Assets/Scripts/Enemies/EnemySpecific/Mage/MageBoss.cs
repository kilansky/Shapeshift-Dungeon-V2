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
    public float projectileChargeTime = 0.5f;
    public float attack2RotateSpeed = 5f;
    public GameObject magicProjectile;

    [Header("Teleporting")]
    public float minTeleportRange = 15f;
    public float maxTeleportRange = 30f;
    public float minTeleportRangeFromPlayer = 15f;
    public float maxTeleportRangeFromPlayer = 30f;
    public GameObject teleportParticles;

    [Header("Fire Points")]
    public Transform[] firePoints1;
    public Transform[] firePoints2;
    public Transform firePoints2Root;

    [Header("Object References")]
    public GameObject aliveMage;
    public GameObject deathEffect;

    public float Health { get; set; }

    [HideInInspector] public List<GameObject> unfiredProjectiles = new List<GameObject>();
    [HideInInspector] public float distanceToPlayer;
    [HideInInspector] public bool isStunned;
    [HideInInspector] public bool isInvincible = false;

    //-----------Private-----------
    private Slider healthBar;
    private GameObject player;
    private Animator animator;
    private Renderer mageRenderer;
    private float currentStunResistance;
    private float lastTimeAttacked;
    private Quaternion lastTargetRotation;

    private bool isAttacking = false;
    private bool rotateTowardsPlayer = true;
    private bool stopMoving = false;


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

        //InvokeRepeating("Attack1", 5f, 5f);
        StartCoroutine(Attack2());
    }

    private void Update()
    {
        //if the enemy has not been hit recently, reset resistance to stuns
        if (Time.time >= lastTimeAttacked + stunRecoveryTime)
            ResetStunResistance();

        Rotate();

        if (isAttacking)
        {
            //rotateTowardsPlayer = false;
            firePoints2Root.Rotate(0f, 10f * attack2RotateSpeed * Time.deltaTime, 0f);
        }
    }

    private void Rotate()
    {
        if (!rotateTowardsPlayer)
            return;

        //Get direction to plater
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

    public void Damage(float damage)
    {
        //start timer
        lastTimeAttacked = Time.time;
        //TODO: check what player damage variable is called, and script name
        //subtract the players damage from the enemies stun resistance
        currentStunResistance -= damage;

        if (!isInvincible)
        {
            //enemy takes damage from the player
            Health -= damage;
            Flash();
            UpdateUI();
            //Debug.Log("Enemy Took Damage");

            //When the enemy takes enough damage and is killed it will do the kill function then the player kapala item special item charge function from player controller - AHL (4/20/21)
            if (Health <= 0)
                Kill();

            //if currently stunned, flip bool
            //Debug.Log("stun resistance is set to: " + currentStunResistance);
            if (currentStunResistance <= 0)
            {
                //Debug.Log("currentStunResistance is less than 0");
                isStunned = true;
            }

            //prevent from taking damage temporarily
            StartCoroutine(InvincibilityFrames());
        }
    }

    public void FireDamage(float damage)
    {
        Flash();

        //enemy takes damage from the player
        Health -= damage;
        UpdateUI();

        //When the enemy takes enough damage and is killed it will do the kill function then the player kapala item special item charge function from player controller - AHL (4/20/21)
        if (Health <= 0)
            Kill();
    }

    public void Flash()
    {
        //sets enemy's color to the hitMat (red)
        mageRenderer.material = hitMat;
        StartCoroutine(WaitToResetColor());

    }

    public void ResetColor()
    {
        mageRenderer.material = normalMat;
    }

    public void Heal(float heal)
    {
        //heal the enemy
        Health = Mathf.Clamp(Health + heal, 0, healthBar.maxValue);
        UpdateUI();
        //Debug.Log("Enemy Healed");
    }

    public void Kill()
    {
        Instantiate(deathEffect, transform.position + new Vector3(0, 2, 0), Quaternion.identity);
        HUDController.Instance.HideBossHealthBar();

        //Destroy self
        Destroy(gameObject);
    }

    private void UpdateUI()
    {
        healthBar.value = Health;
    }

    public void Attack1()
    {
        //Charge up and fire a projectile from each fire point
        foreach (Transform firePoint in firePoints1)
            StartCoroutine(ChargeAttack1(magicProjectile, firePoint, projectileChargeTime));
    }

    public IEnumerator Attack2()
    {
        while(true)
        {
            yield return new WaitForSeconds(1.5f);
            isAttacking = true;

            //Charge up a ring of projectiles
            foreach (Transform firePoint in firePoints2)
            {
                StartCoroutine(ChargeAttack2(magicProjectile, firePoint, projectileChargeTime));
                yield return new WaitForSeconds(0.25f);
            }

            yield return new WaitForSeconds(1f);

            //Fire each projectile at the player
            foreach (Transform firePoint in firePoints2)
            {
                Attack2End(magicProjectile, firePoint, projectileChargeTime);
                yield return new WaitForSeconds(0.5f);
            }

            isAttacking = false;

            yield return new WaitForSeconds(0.5f);
            Teleport();
        }
    }

    IEnumerator ChargeAttack1(GameObject fireball, Transform firePoint, float chargeTime)
    {
        GameObject bullet = Instantiate(fireball, firePoint.transform.position, firePoint.transform.rotation, firePoint.transform);
        Vector3 originalScale = bullet.transform.localScale;
        float vfxPercent = 0;

        //set movespeed to 0
        bullet.GetComponent<Bullet>().moveSpeed = 0f;
        bullet.transform.GetChild(0).gameObject.SetActive(false);

        yield return new WaitForSeconds(.1f);

        //set scale of bullet to .1 scale up, doesn't scale vfx
        bullet.transform.GetChild(0).gameObject.SetActive(true);
        bullet.transform.localScale = Vector3.one * .1f;

        //Sets the bullets parent game object to this one to make the Damage Tracker acquire the skull as the correct game object for tracking
        bullet.GetComponent<Bullet>().parentObject = gameObject;

        //disable inner collider so it doesn't destroy while charging
        bullet.GetComponent<CapsuleCollider>().enabled = false;

        float timeElapsed = 0f; //Counter to keep track of time elapsed
        while (timeElapsed < chargeTime) //This while loop scales object over time
        {
            if (isStunned)
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

        //WE DESTROY THE GD BULLET WE JUST WENT THROUGH CREATING
        Destroy(bullet);

        //WHY DOES THIS WORK
        bullet = Instantiate(fireball, firePoint.transform.position, firePoint.transform.rotation);
        bullet.transform.localScale = originalScale;
        bullet.GetComponent<Bullet>().canDamage = true;
        //sparks.Play();

        //yield return new WaitForSeconds(attackCoolDown);
        isAttacking = false;
    }

    IEnumerator ChargeAttack2(GameObject fireball, Transform firePoint, float chargeTime)
    {
        GameObject bullet = Instantiate(fireball, firePoint.transform.position, firePoint.transform.rotation, firePoint.transform);
        Vector3 originalScale = bullet.transform.localScale;
        float vfxPercent = 0;
        isAttacking = true;

        bullet.GetComponent<Bullet>().moveSpeed = 0f;
        bullet.GetComponent<Bullet>().canBeDestroyed = false;
        bullet.GetComponent<Bullet>().parentObject = gameObject;
        bullet.transform.GetChild(0).gameObject.SetActive(false);

        yield return new WaitForSeconds(.1f);

        //set scale of bullet to .1 scale up, doesn't scale vfx
        bullet.transform.GetChild(0).gameObject.SetActive(true);
        bullet.transform.localScale = Vector3.one * .1f;

        //disable inner collider so it doesn't destroy while charging
        bullet.GetComponent<CapsuleCollider>().enabled = false;

        float timeElapsed = 0f; //Counter to keep track of time elapsed
        while (timeElapsed < chargeTime) //This while loop scales object over time
        {
            if (isStunned)
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

    private void Attack2End(GameObject fireball, Transform firePoint, float chargeTime)
    {
        if (unfiredProjectiles.Count == 0)
            return;

        int randIndex = Random.Range(0, unfiredProjectiles.Count);
        GameObject bullet = unfiredProjectiles[randIndex];
        Vector3 bulletPos = bullet.transform.position;
        unfiredProjectiles.Remove(unfiredProjectiles[randIndex]);

        Destroy(bullet);

        //Spawn new projectiles in the direction of the player
        Vector3 targetPoint = new Vector3(player.transform.position.x, 0, player.transform.position.z) - new Vector3(bulletPos.x, 0, bulletPos.z);
        Quaternion targetRotation = Quaternion.LookRotation(targetPoint);
        bullet = Instantiate(fireball, bulletPos, targetRotation);

        //Sets the bullets parent game object to this one to make the Damage Tracker acquire the skull as the correct game object for tracking
        bullet.GetComponent<Bullet>().parentObject = gameObject;
        bullet.GetComponent<Bullet>().canDamage = true;
        //sparks.Play();
    }

    public void Teleport()
    {
        //teleport to a new position
        Transform safeTeleportTile = FindSafeTeleportTile();

        if(safeTeleportTile)
        {
            GameObject vfx = Instantiate(teleportParticles, transform.position + new Vector3(0, 1.5f, 0), Quaternion.identity);
            Destroy(vfx, 2f);

            Vector3 teleportPos = safeTeleportTile.position + new Vector3(0f, 5f, 0f);
            transform.position = teleportPos;
        }
    }

    private Transform FindSafeTeleportTile()
    {
        List<Tile> tiles = new List<Tile>();
        foreach (Tile tile in GameObject.FindObjectsOfType<Tile>())
        {
            //Check if the tile is safe to stand on
            if(tile.tileType == Tile.tileTypes.stone || tile.tileType == Tile.tileTypes.dirt || tile.tileType == Tile.tileTypes.stoneGrass || tile.tileType == Tile.tileTypes.dirtGrass)
            {
                float distToTile = Vector3.Distance(transform.position, tile.transform.position);
                float playerDistToTile = Vector3.Distance(player.transform.position, tile.transform.position);

                //Check if the distance to this new tile is far from the mage, and within a certain range of the player
                if((distToTile > minTeleportRange && distToTile < maxTeleportRange) && (playerDistToTile > minTeleportRangeFromPlayer && playerDistToTile < maxTeleportRangeFromPlayer))
                {
                    //Add this tile to the list of potential safe tiles to teleport to
                    tiles.Add(tile);
                }
            }
        }

        if(tiles.Count == 0)
            Debug.LogError("Mage attempted to teleport but found no valid tiles");
        else
        {
            Debug.Log("Mage found " + tiles.Count + " safe tiles to teleport to");
            int randTileIndex = Random.Range(0, tiles.Count);
            return tiles[randTileIndex].transform;
        }

        return null;
    }

    //function for enemies ability to resist stuns
    public void ResetStunResistance()
    {
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

    public IEnumerator WaitToResetColor()
    {
        //reset enemy's color to normalMat
        yield return new WaitForSeconds(dmgInvincibilityTime);
        ResetColor();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, minTeleportRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, maxTeleportRange);
    }
}
