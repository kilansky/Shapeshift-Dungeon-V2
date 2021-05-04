using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]

public class Worm : EnemyBase
{
    //this script contains all the states of the Worm
    public Worm_IdleState idleState { get; private set; }
    public Worm_MoveState moveState { get; private set; }
    public Worm_PlayerDetected playerDetectedState { get; private set; }
    public Worm_AttackState attackState { get; private set; }
    public Worm_LookForPlayer lookForPlayerState { get; private set; }
    public Worm_StunState stunState { get; private set; }

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
    [HideInInspector] public bool isAttacking = false;

    //public GameObject meleeHitBox;
    //public Transform hitPoint;

    public SkinnedMeshRenderer renderer;

    private float attackDamage;


    public override void Start()
    {
        SetNewTarget(player);
        base.Start();

        moveState = new Worm_MoveState(this, stateMachine, "move", moveStateData, this);
        idleState = new Worm_IdleState(this, stateMachine, "idle", idleStateData, this);
        playerDetectedState = new Worm_PlayerDetected(this, stateMachine, "playerDetected", playerDetectedData, this);
        attackState = new Worm_AttackState(this, stateMachine, "attack", attackStateData, this);
        lookForPlayerState = new Worm_LookForPlayer(this, stateMachine, "lookForPlayer", lookForPlayerStateData, this);
        stunState = new Worm_StunState(this, stateMachine, "stun", stunStateData, this);

        //initialize the goblin in the idle state
        //stateMachine.Initialize(idleState);

        //this line is what got rid of my NullReferenceExceptions
        stateMachine.Initialize(idleState);

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
