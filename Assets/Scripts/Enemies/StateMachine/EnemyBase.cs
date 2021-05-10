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

    [Header("Object References")]
    public GameObject aliveGO;
    public Slider healthBar;
    public D_Entity entityData;
    public FiniteStateMachine stateMachine;
    public GameObject deathEffect;
    public GameObject gemPrefab;

    [Header("Health & Taking Damage")]
    public float health;
    public float dmgInvincibilityTime = 0.25f;
    public float knockBackStrength = 5f;
    public Material hitMat;
    public Material normalMat;

    [Header("Attacking")]
    public float sightRange = 20f;
    public float minAgroRange = 20f;
    public float minAttackRange = 3f;
    public float timeBetweenAttacks = 3f;
    public float meleeDamage = 3f;

    [HideInInspector] public GameObject player;
    [HideInInspector] public float distanceToPlayer;
    [HideInInspector] public NavMeshAgent agent;
    [HideInInspector] public bool isStunned;
    [HideInInspector] public bool isInvincible = false;
    [HideInInspector] public bool isKnockedBack;

    #endregion

    #region Getters and Setters
    //public Rigidbody RB { get; private set; }
    public Rigidbody topRB { get; private set; }
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
    private Vector3 knockbackDirection;
    private bool isBeingKnockedBack = false;
    private bool stopMoving = false;
    private bool isMageBoss = false;

    #endregion
    #region Protected Variables nothing in here atm
    protected Transform target;

    protected Vector3 OGposition;
    protected Quaternion OGrotation;

    //protected bool canAttack = true;
    //protected bool isPlayerInMinAgroRange;
    //protected bool isPlayerInMinAttackRange;
    #endregion

    public virtual void Awake()
    {
        player = FindObjectOfType<PlayerController>().gameObject;
        distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        isMageBoss = GetComponent<MageBoss>() ? true : false;
    }

    public virtual void Start()
    {
        if (!isMageBoss)
        {
            //possibly unnecessary variable
            FacingDirection = 1;
            //how resistant is this enemy to stuns
            currentStunResistance = entityData.stunResistance;

            agent = GetComponent<NavMeshAgent>();
            topRB = GetComponent<Rigidbody>();
            Anim = aliveGO.GetComponent<Animator>();

            //Set increased health if gem monster
            if (GetComponent<GemMonster>().isGemMonster)
                health *= GetComponent<GemMonster>().healthMod;

            //set health variables
            Health = health;
            healthBar.maxValue = Health;
            healthBar.value = Health;

            //If the player has the Monster Mask Item then the enemy will slow down - AHL (5/6/21)
            if (PlayerController.Instance.hasMonsterMask)
            {
                agent.speed = agent.speed * 0.85f; //Adjusts the movement speed of the enemy so it is slower
                Anim.SetFloat("attackSpeed", 0.85f); //Adjusts the animation speed of the attacks and movement so it all appears slower for added visual feedback to the player
            }

            //SetNewTarget();

            stateMachine = new FiniteStateMachine();
        }
    }

    public virtual void Update()
    {
        if (!isMageBoss)
        {
            distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
            stateMachine.currentState.LogicUpdate();

            //if the skull is recovered, reset resistance to stuns
            if (Time.time >= lastTimeAttacked + entityData.stunRecoveryTime)
            {
                ResetStunResistance();
            }
        }
    }

    public virtual void FixedUpdate()
    {
        if (!isMageBoss)
            stateMachine.currentState.PhysicsUpdate();
    }

    public virtual void SetVelocity(float velocity)
    {
        //may need to change this
        //velocityWorkspace.Set(FacingDirection * velocity * Time.fixedDeltaTime, RB.velocity.y, RB.velocity.x);
        //RB.velocity = velocityWorkspace;
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

    public virtual bool CheckPlayerInMinAgroRange()
    {
        if (distanceToPlayer < minAgroRange)
            return true;
        else
            return false;
    }

    public virtual bool CheckPlayerInMinAttackRange()
    {
        if (distanceToPlayer < minAttackRange)
            return true;
        else
            return false;
    }

    public virtual void Damage(float damage)
    {
        if (!isMageBoss)
        {
            //start timer
            lastTimeAttacked = Time.time;
            //TODO: check what player damage variable is called, and script name

            if (!isInvincible)
            {
                //enemy takes damage from the player
                Health -= damage;
                UpdateUI();
                Flash();
                //Debug.Log("Enemy Took Damage");

                //When the enemy takes enough damage and is killed it will do the kill function then the player kapala item special item charge function from player controller - AHL (4/20/21)
                if (Health <= 0)
                {
                    Kill();
                    PlayerController.Instance.KapalaSpecialRecharge();
                }

                //Debug.Log("stun resistance is set to: " + currentStunResistance);
                //subtract the players damage from the enemies stun resistance
                currentStunResistance -= damage;
                if (currentStunResistance <= 0)
                    isStunned = true; //if currently stunned, flip bool

                //prevent from taking damage temporarily
                StartCoroutine(InvincibilityFrames());
            }
        }
        else //Use the corresponding function of the MageBoss instead
            GetComponent<MageBoss>().Damage(damage);

        //AudioManager.Instance.Play("Hit");
    }

    public virtual void FireDamage(float damage)
    {
        if (!isMageBoss)
        {
            Flash();

            //enemy takes damage from the player
            Health -= damage;
            UpdateUI();

            //When the enemy takes enough damage and is killed it will do the kill function then the player kapala item special item charge function from player controller - AHL (4/20/21)
            if (Health <= 0)
            {
                PlayerController.Instance.KapalaSpecialRecharge();
                Kill();
            }
        }
        else //Use the corresponding function of the MageBoss instead
            GetComponent<MageBoss>().FireDamage(damage);
    }

    public virtual void Flash()
    {
        //sets enemy's color to the hitMat (red)
        StartCoroutine(WaitToResetColor());
    }

    public virtual void ResetColor()
    {
        //base place holder function to reset color of enemies
    }

    public virtual void Heal(float heal)
    {
        if (!isMageBoss)
        {
            Debug.Log("Enemy Healed");

            //heal the enemy
            Health = Mathf.Clamp(Health + heal, 0, healthBar.maxValue);
            UpdateUI();
        }
        else //Use the corresponding function of the MageBoss instead
            GetComponent<MageBoss>().Heal(heal);
    }

    public virtual void Kill()
    {
        if (!isMageBoss)
        {
            //Update the monster count of the room
            MonsterSpawner.Instance.MonsterKilled();
            Instantiate(deathEffect, transform.position + new Vector3(0, agent.height / 2, 0), Quaternion.identity);

            if (GetComponent<GemMonster>().isGemMonster)
                DropGem();

            int randomSound = Random.Range(0, 2);
            switch(randomSound)
            {
                case 0:
                    AudioManager.Instance.Play("EnemyDeath1");
                    break;
                case 1:
                    AudioManager.Instance.Play("EnemyDeath2");
                    break;
                default:
                    Debug.LogError("I broke the switch statement");
                    break;
            }
            
            //Destroy self from root object
            Destroy(transform.root.gameObject);
        }
    }

    public virtual void DropGem()
    {
        GameObject gem = Instantiate(gemPrefab, transform.position + new Vector3(0, 1, 0), Quaternion.identity);
        gem.GetComponent<Rigidbody>().AddForce(Vector3.up * 350f);
    }

    public virtual void UpdateUI()
    {
        healthBar.value = Health;
    }

    //Change the target object for the enemy to move to
    public virtual void SetNewTarget(GameObject newTarget)
    {
        //this will be used for the dummy item
        target = newTarget.transform;
    }

    public virtual void SetNewDestination()
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

    //function for enemies ability to resist stuns
    //for bigger enemies such as ogre's, worms, etc.
    public virtual void ResetStunResistance()
    {
        isStunned = false;
        //TODO: add stunResistance to entityData
        currentStunResistance = entityData.stunResistance;
    }


    //Makes the enemy invincible briefly
    public virtual IEnumerator InvincibilityFrames()
    {
        isInvincible = true;
        yield return new WaitForSeconds(dmgInvincibilityTime);

        if(Health > 0)
            isInvincible = false;
    }

    public virtual IEnumerator WaitToResetColor()
    {
        //reset enemy's color to normalMat
        yield return new WaitForSeconds(dmgInvincibilityTime);
        //renderer.material = normalMat;
        ResetColor();
    }

    //public IEnumerator EnemyKnockBack()
    public virtual IEnumerator EnemyKnockBack()
    {
        if (!isMageBoss && !isBeingKnockedBack)
        {
            Debug.Log("Should Knockback Enemy");

            isBeingKnockedBack = true;

            OGposition = topRB.transform.position;
            OGrotation = topRB.transform.rotation;
            //Debug.Log("OG chillin at " + OGposition);

            isKnockedBack = true;
            agent.enabled = false;
            topRB.isKinematic = false;

            //don't knock them back in the air

            //TODO: double check the logic here should be enemy - knockbacker object
            knockbackDirection = (transform.position - player.transform.position).normalized;
            knockbackDirection.y = 0;
            //Debug.Log("knockbackdirection is " + knockbackDirection);

            topRB.AddForce(knockbackDirection * knockBackStrength, ForceMode.Impulse);

            topRB.transform.position = OGposition;
            topRB.transform.rotation = OGrotation;

            yield return new WaitForSeconds(0.5f);

            if (Health > 0)
            {
                isKnockedBack = false;
                agent.enabled = true;
                topRB.isKinematic = true;
                isBeingKnockedBack = false;
            }
        }
    }

    public virtual void OnDrawGizmos()
    {
        if (wallCheck)
            Gizmos.DrawLine(wallCheck.position, wallCheck.position + (Vector3)(Vector2.right * FacingDirection * entityData.wallCheckDistance));

        if (ledgeCheck)
            Gizmos.DrawLine(ledgeCheck.position, ledgeCheck.position + (Vector3)(Vector2.down * entityData.ledgeCheckDistance));

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, minAgroRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, minAttackRange);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!isMageBoss && other.CompareTag("KillBox"))
        {
            Debug.LogError("MONSTER TOUCHED KILLBOX");
            MonsterSpawner.Instance.MonsterKilledPrematurly();

            Destroy(transform.root.gameObject);
        }
    }
}
