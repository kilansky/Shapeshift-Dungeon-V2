using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyBase : MonoBehaviour, IDamageable
{
    //All enemy scripts will inherit from this script

    //Shared Stats amongst all enemies:
    //Health, Move Speed

    //Shared Functions:
    //Move(), TakeDamage(), Die(), TargetPlayer()
    #region Public Variables
    public GameObject aliveGO;
    public GameObject player;
    public float health;
    
    //public Renderer renderer;
    //public SkinnedMeshRenderer renderer;

    public Material hitMat;
    public Material normalMat;

    public Slider healthBar;
    public D_Entity entityData;
    public FiniteStateMachine stateMachine;

    public float dmgInvincibilityTime = 0.5f;
    public float sightRange = 20f;
    public float minAttackRange;
    //public float maxAttackRange = 20f;
    public float timeBetweenAttacks = 3f;
    public float minAgroRange;
    public GameObject deathEffect;
    public GameObject gemPrefab;

    public Vector3 knockbackDirection;
    public float knockbackForce = 10f;
    public bool isKnockedBack = false;
    

    [HideInInspector] public float distanceToPlayer;
    [HideInInspector] public NavMeshAgent agent;
    [HideInInspector] public bool isStunned;
    
    #endregion

    #region Getters and Setters
    public Rigidbody RB { get; private set; }
    public Animator Anim { get; private set; }
    //public float minAgroRange { get; private set; }
    public float maxAgroRange { get; private set; }
    public float FacingDirection { get; private set; }
    public float Health { get; set; }
    #endregion
    
    #region Serialize Fields
    [SerializeField]
    public LayerMask whatIsPlayer;

    [SerializeField]
    private Transform wallCheck;
    [SerializeField]
    private Transform ledgeCheck;
    [SerializeField]
    private Transform playerCheck;
    //[SerializeField]
    //private Transform groundCheck;
    #endregion

    #region Private Variables
    private float currentStunResistance;
    private float lastTimeAttacked;
    
    private Vector3 velocityWorkspace;
    
    //private bool isAttacking = false;
    private bool stopMoving = false;
    private bool isInvincible = false;
    #endregion

    #region Protected Variables nothing in here atm
    protected Transform target;
    //protected bool canAttack = true;
    //protected bool isPlayerInMinAgroRange;
    //protected bool isPlayerInMinAttackRange;
    #endregion

    public virtual void Awake()
    {
        player = FindObjectOfType<PlayerController>().gameObject;
        distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
    }

    public virtual void Start()
    {
        //possibly unnecessary variable
        FacingDirection = 1;
        //how resistant is this enemy to stuns
        currentStunResistance = entityData.stunResistance;

        agent = GetComponent<NavMeshAgent>();
        RB = aliveGO.GetComponent<Rigidbody>();
        Anim = aliveGO.GetComponent<Animator>();

        //find player pos and go to it
        //SetNewTarget(player);
        //SetDestination();

        //set health variables

        if (GetComponent<GemMonster>().isGemMonster)
            health *= GetComponent<GemMonster>().healthMod;

        Health = health;
        healthBar.maxValue = Health;
        healthBar.value = Health;

        stateMachine = new FiniteStateMachine();
    }

    public virtual void Update()
    {
        distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        stateMachine.currentState.LogicUpdate();

        //if the skull is recovered, reset resistance to stuns
        if(Time.time >= lastTimeAttacked + entityData.stunRecoveryTime)
        {
            ResetStunResistance();
        }
        #region player range checks, may get deleted
        /*isPlayerInMinAgroRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        isPlayerInMinAttackRange = Physics.CheckSphere(transform.position, minAttackRange, whatIsPlayer);
        float dist = Vector3.Distance(transform.position, target.transform.position);

        if (isPlayerInMinAgroRange)
        {
            //not every enemy attacks from distance, need to change conditions
            if (dist < 5 && canAttack)
            {
                Attack();
            }
            else if (!isAttacking)
            {
                //Debug.Log("i pretend not to see the player");
                SetDestination();
            }
        }

        if (isAttacking)
        {
            Vector3 targetPoint = new Vector3(player.transform.position.x, 0, player.transform.position.z) - new Vector3(transform.position.x, 0, transform.position.z);
            Quaternion targetRotation = Quaternion.LookRotation(targetPoint);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10 * Time.deltaTime);
        }*/
        #endregion
    }

    public virtual void FixedUpdate()
    {
        stateMachine.currentState.PhysicsUpdate();
    }

    public virtual void SetVelocity(float velocity)
    {
        //may need to change this
        velocityWorkspace.Set(FacingDirection * velocity * Time.fixedDeltaTime, RB.velocity.y, RB.velocity.x);
        RB.velocity = velocityWorkspace;
    }

    public virtual bool CheckWall()
    {
        //if enemy ran into a wall, find player pos
        //again and try to chase, floating skulls fly over walls if possible
        return Physics.Raycast(wallCheck.position, aliveGO.transform.right, entityData.wallCheckDistance, entityData.whatIsGround);
    }

    public virtual bool CheckLedge()
    {
        //if enemy ran into a ledge, find player pos
        //again and try to chase, floating skulls won't implement this
        return Physics.Raycast(ledgeCheck.position, Vector3.down, entityData.ledgeCheckDistance, entityData.whatIsGround);
    }

    /*public virtual bool CheckGround()
    {
        return Physics.OverlapSphere(groundCheck.position, entityData df, entityData.whatIsGround);
    }*/

    //min/max agro ranges for enemies
    public virtual bool CheckPlayerInMinAgroRange()
    {
        if (distanceToPlayer < minAgroRange)
            return true;
        else
            return false;
    }

    /*public virtual bool CheckPlayerInMaxAgroRange()
    {
        if (distanceToPlayer < maxAgroRange)
            return true;
        else
            return false;
        //return Physics.CheckSphere(playerCheck.position, maxAgroRange, entityData.whatIsPlayer);
    }*/
    //player in attack range check
    public virtual bool CheckPlayerInMinAttackRange()
    {
        if(distanceToPlayer < minAttackRange)
            return true;
        else
            return false;
    }

    /*public virtual bool CheckInSightRange()
    {
        RaycastHit hit;
        //FloatingCrystal[] crystals = FindObjectsOfType<FloatingCrystal>();

        //draw a vector3 from crystals pos to target's pos
        //need to define target
        Vector3 direction = transform.position - target.transform.position;

        Debug.DrawRay(firePointFront.transform.position, direction, Color.blue);

        if (Physics.Raycast(firePointFront.transform.position, direction, out hit))
        {
            if (hit.transform.CompareTag("Crystal"))
            {
                //connect with the crystal
                return true;
            }
            if (hit.transform.CompareTag("Player"))
            {
                //do damage to the player
                return true;
            }
        }
        return false;
    }*/

    public virtual void Damage(float damage)
    {
        //start timer
        lastTimeAttacked = Time.time;
        //TODO: check what player damage variable is called, and script name
        //subtract the players damage from the enemies stun resistance
        currentStunResistance -= damage;

        Flash();


        if (!isInvincible)
        {
            //enemy takes damage from the player
            Health -= damage;
            UpdateUI();

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

    public virtual void Flash()
    {
        //sets enemy's color to the hitMat (red)
        //renderer.material = hitMat;
        StartCoroutine(WaitToResetColor());
        
    }

    public virtual void ResetColor()
    {

    }

    public virtual void Heal(float heal)
    {
        Debug.Log("Enemy Healed");

        //heal the enemy
        Health += heal;
        UpdateUI();
    }

    public virtual void Kill()
    {
        //Update the monster count of the room
        MonsterSpawner.Instance.MonsterKilled();
        Instantiate(deathEffect, transform.position + new Vector3(0, 2, 0), Quaternion.identity);

        if (GetComponent<GemMonster>().isGemMonster)
            DropGem();

        //Destroy self from root object 
        Destroy(transform.root.gameObject);
    }

    private void DropGem()
    {
        GameObject gem = Instantiate(gemPrefab, transform.position + new Vector3(0, 1, 0), Quaternion.identity);
        gem.GetComponent<Rigidbody>().AddForce(Vector3.up * 350f);
    }

    private void UpdateUI()
    {
        healthBar.value = Health;
    }

    //Change the target object for the enemy to move to
    public virtual void SetNewTarget(GameObject newTarget)
    {
        //this will be used for the dummy item
        target = newTarget.transform;
        
    }

    public void SetDestination()
    {
        if (target != null && !isKnockedBack)
        {
            //target the player
            agent.SetDestination(target.position);
        }
        #region turn variables 
        //make sure the enemy faces the player
        //this will be the same for all enemies
        //Vector3 direction = (target.position - transform.position).normalized;
        //Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        //transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        #endregion
    }

    public virtual void Attack()
    {
        //attack the player
        //agent.SetDestination(transform.position);
        //isAttacking = true;

        //stateMachine.currentState.LogicUpdate();
        //stateMachine.ChangeState(AttackState);

        //these two lines will go into floatingSkull_attack
        //GameObject bullet = Instantiate(fireball, shootPoint.transform.position, transform.rotation, transform);
        //StartCoroutine(BulletCharge(bullet));
    }

    //function for enemies ability to resist stuns
    //for bigger enemies such as ogre's, worms, etc.
    public virtual void ResetStunResistance()
    {
        isStunned = false;
        //TODO: add stunResistance to entityData
        currentStunResistance = entityData.stunResistance;
    }

    //Makes the enemy invincible briefly
    IEnumerator InvincibilityFrames()
    {
        isInvincible = true;
        yield return new WaitForSeconds(dmgInvincibilityTime);
        isInvincible = false;
    }

    public IEnumerator WaitToResetColor()
    {
        //reset enemy's color to normalMat
        yield return new WaitForSeconds(dmgInvincibilityTime);
        //renderer.material = normalMat;
        ResetColor();
    }

    public IEnumerator EnemyKnockBack()
    {
        Debug.Log("enemybase script called");

        isKnockedBack = true;
        agent.enabled = false;
        RB.isKinematic = false;

        //where do they get knocked back too?
        knockbackDirection.y = 0;
        knockbackDirection = (transform.position - player.transform.position).normalized;

        RB.velocity = knockbackDirection * knockbackForce;
        RB.AddForce(knockbackDirection.normalized * knockbackForce, ForceMode.Impulse);


        yield return new WaitForSeconds(0.5f);

        agent.enabled = true;
        RB.isKinematic = true;
        isKnockedBack = false;
    }


    public void OnDrawGizmos()
    {
        Gizmos.DrawLine(wallCheck.position, wallCheck.position + (Vector3)(Vector2.right * FacingDirection * entityData.wallCheckDistance));
        Gizmos.DrawLine(ledgeCheck.position, ledgeCheck.position + (Vector3)(Vector2.down * entityData.ledgeCheckDistance));
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, minAgroRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, minAttackRange);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("KillBox"))
        {
            Debug.Log("MONSTER TOUCHED KILLBOX");
            MonsterSpawner.Instance.MonsterKilledPrematurly();

            Destroy(transform.root.gameObject);
        }
    }
}
