using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]

//add Idamageable later
public class Goblin : EnemyBase
{
    //this script contains all the states of the floating skull
    public Goblin_IdleState idleState { get; private set; }
    public Goblin_MoveState moveState { get; private set; }
    public Goblin_PlayerDetected playerDetectedState { get; private set; }
    public Goblin_AttackState attackState { get; private set; }
    public Goblin_LookForPlayer lookForPlayerState { get; private set; }
    public Goblin_StunState stunState { get; private set; }

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

    //will have a melee range instead of fireball
    public float AttackDamage { get { return attackDamage; } }

    [HideInInspector] public GameObject FrontTarget;
    [HideInInspector] public GameObject SideTarget;
    [HideInInspector] public GameObject BackTarget;

    //public GameObject meleeHitBox;
    //public Transform hitPoint;

    public SkinnedMeshRenderer renderer;

    private float attackDamage;


    public override void Start()
    {
        SetNewTarget(player);
        base.Start();

        moveState = new Goblin_MoveState(this, stateMachine, "move", moveStateData, this);
        idleState = new Goblin_IdleState(this, stateMachine, "idle", idleStateData, this);
        playerDetectedState = new Goblin_PlayerDetected(this, stateMachine, "playerDetected", playerDetectedData, this);
        attackState = new Goblin_AttackState(this, stateMachine, "attack", attackStateData, this);
        lookForPlayerState = new Goblin_LookForPlayer(this, stateMachine, "lookForPlayer", lookForPlayerStateData, this);
        stunState = new Goblin_StunState(this, stateMachine, "stun", stunStateData, this);

        //initialize the goblin in the idle state
        //stateMachine.Initialize(idleState);

        //this line is what got rid of my NullReferenceExceptions
        stateMachine.Initialize(moveState);

        FrontTarget = PlayerController.Instance.frontTarget;
        SideTarget = PlayerController.Instance.sideTarget;
        BackTarget = PlayerController.Instance.backTarget;
    }

    //set current state to stunState if isStunned
    public override void Damage(float damage)
    {
        base.Damage(damage);
        Flash();
        /*if (isDead)
        {
            stateMachine.ChangeState(deadState);
        }*/
        if (isStunned && stateMachine.currentState != stunState)
        {
            stateMachine.ChangeState(stunState);
        }
    }

    public override void SetNewTarget(GameObject newTarget)
    {
        //this will be used for the dummy item
        target = newTarget.transform;

    }

    public bool HaveLineOfSight()
    {
        RaycastHit sightHit;

        //vector3 drawn from the goblin to the player
        Vector3 direction = player.transform.position - transform.position;
        //Debug.DrawRay(transform.position, direction, Color.red);
        if(Physics.Raycast(transform.position, direction, out sightHit/*, may need distance var here*/))
        {
            //if we see our target gameObject, draw the line
            //if(sightHit.transform.gameObject.name.Equals.BackTarget)
                //Debug.DrawRay(transform.position, direction, Color.red);
        }


        return false;
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
}