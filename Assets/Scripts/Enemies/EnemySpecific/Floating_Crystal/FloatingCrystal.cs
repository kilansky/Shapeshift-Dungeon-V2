using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]

public class FloatingCrystal : EnemyBase
{
    //script contains all the states of the floating crystal
    public FloatingCrystal_IdleState idleState { get; private set; }
    public FloatingCrystal_MoveState moveState { get; private set; }
    public FloatingCrystal_PlayerDetected playerDetectedState { get; private set; }
    public FloatingCrystal_AttackState attackState { get; private set; }
    public FloatingCrystal_LookForPlayer lookForPlayerState { get; private set; }
    public FloatingCrystal_StunState stunState { get; private set; }


    [SerializeField]
    private D_IdleState idleStateData;
    [SerializeField]
    private D_MoveState moveStateData;
    [SerializeField]
    private D_PlayerDetected playerDetectedData;
    [SerializeField]
    private D_AttackState attackStateData;
    [SerializeField]
    private D_LookForPlayer lookForPlayerStateData;
    [SerializeField]
    private D_StunState stunStateData;

    //firing positions of front and back lasers
    public Transform firePointFront;
    public Transform firePointBack;
    //public Transform target;

    public GameObject laser;
    //public GameObject crystal;
    public float chargeRate = 1f;

    public Vector3 walkPoint;
    
    public bool walkPointSet;
    public float walkPointRange = 20f;
    public float rotateSpeed = 5f;


    public override void Awake()
    {
        base.Awake();
        //find any other crystals in the scene
        //crystal = FindObjectsOfType<FloatingCrystal>().gameObject;
    }


    public override void Start()
    {
        
        base.Start();

        moveState = new FloatingCrystal_MoveState(this, stateMachine, "move", moveStateData, this);
        idleState = new FloatingCrystal_IdleState(this, stateMachine, "idle", idleStateData, this);
        playerDetectedState = new FloatingCrystal_PlayerDetected(this, stateMachine, "playerDetected", playerDetectedData, this);
        attackState = new FloatingCrystal_AttackState(this, stateMachine, "attack", /*firePoint,*/ attackStateData, this);
        lookForPlayerState = new FloatingCrystal_LookForPlayer(this, stateMachine, "lookForPlayer", lookForPlayerStateData, this);
        stunState = new FloatingCrystal_StunState(this, stateMachine, "stun", stunStateData, this);

        //create an array of any crystals in the scene
        FloatingCrystal[] crystals = FindObjectsOfType<FloatingCrystal>();

        //set target to any other crystals position
        foreach (FloatingCrystal floatingCrystal in crystals)
        {
            Debug.Log("there are " + crystals.Length + " crystals in the scene");
        }

        //this line is what got rid of my NullReferenceExceptions
        //start the enemy in its move state
        stateMachine.Initialize(moveState);
    }

    //set current state to stunState if isStunned
    public override void Damage(float damage)
    {
        base.Damage(damage);

        /*if (isDead)
        {
            stateMachine.ChangeState(deadState);
        }*/
        if (isStunned && stateMachine.currentState != stunState)
        {
            stateMachine.ChangeState(stunState);
        }
    }

    public void Patrol()
    {
        //walkPoint = Random.insideUnitSphere * walkPointRange;
        /*walkPoint += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(walkPoint, out hit, walkPointRange, 1);
        Vector3 finalPosition = hit.position;*/
        //transform.Rotate(0, rotateSpeed, 0);
        //move to a walk point set in the scene
        if (!walkPointSet)
            SearchWalkPoint();

        if (walkPointSet)
        {
            //set the destination to the walkpoint from searchwalkpoint
            agent.SetDestination(walkPoint);
            if (!agent.pathPending)
            {
                //if you've reached your destination do something
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    walkPointSet = false;
                    //if the agent doesn't have a path to a point, or they are stopped
                    if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                    {
                        walkPointSet = false;
                    }
                }
            }
        }
        /*Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if (distanceToWalkPoint.magnitude <= 1f)
            walkPointSet = false;*/
    }

    public void SearchWalkPoint()
    {
        //find a walk point after you've moved to one already
        walkPoint = new Vector3(Random.insideUnitSphere.x * walkPointRange, transform.position.y, Random.insideUnitSphere.z * walkPointRange);
        Debug.Log("my walkpoint is " + walkPoint);
        walkPointSet = true;
        
        /*float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        //walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
        //walkPointSet = true;
        Debug.Log("my walk point is " + walkPoint);*/

        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(walkPoint, out hit, walkPointRange, 1))
        {
            
            finalPosition = hit.position;
            walkPoint = finalPosition;
            
            //Debug.Log("walkpointset is " + walkPointSet);
            //Debug.Log("i'm going to " + finalPosition);
            
        }

        //may need a parameter for what is walkable ground or not
        //walkPointSet = true;

        /*if (Physics.Raycast(walkPoint, -transform.up, 2f))
        {
            walkPointSet = true;
            Debug.Log("walkPoint set SHOULD be true, and it is " + walkPointSet);
        }*/
    }

    /*public virtual bool HaveLineOfSight()
    {
        //handles determining if two crystals have line of sight with one another
        RaycastHit hit;
        //FloatingCrystal[] crystals = FindObjectsOfType<FloatingCrystal>();

        //draw a vector3 from crystals pos to target's pos
        //need to define target
        //Vector3 direction = target.transform.position - transform.position;

        //Debug.DrawRay(firePointFront.transform.position, direction, Color.blue);

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

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("KillBox"))
        {
            Damage(100);
            Debug.Log("MONSTER TOUCHED KILLBOX");
        }
    }

}
