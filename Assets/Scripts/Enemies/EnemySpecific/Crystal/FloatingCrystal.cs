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
    public MeshRenderer renderer;
    //public GameObject laser;
    //public GameObject laserSpawnPoint;
    //public GameObject crystal;
    public float chargeRate = 1f;

    
    //public float rotateSpeed = 3f;


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
        Debug.Log("Floating Crytal Damage called");
        base.Damage(damage);

        if (isStunned && stateMachine.currentState != stunState)
        {
            stateMachine.ChangeState(stunState);
        }
    }

    public override void Flash()
    {
        //sets enemy's color to the hitMat (red)
        renderer.material = hitMat;
        StartCoroutine(WaitToResetColor());

    }

    public override void ResetColor()
    {
        base.ResetColor();
        renderer.material = normalMat;
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
}
