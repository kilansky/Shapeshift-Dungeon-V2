using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetectedState : State
{
    //Enemy Detects the player
    protected D_PlayerDetected stateData;
    //protected GameObject player;
    //protected Transform target;

    public float minAttackRange = 15f;
    public float sightRange = 20f;

    protected bool isPlayerInMinAgroRange;
    //protected bool isPlayerInMaxAgroRange;
    protected bool isPlayerInMinAttackRange;
    protected bool doRangedAttack;
    protected bool canAttack = true;
    protected bool isAttacking = false;


    [SerializeField]
    public LayerMask whatIsPlayer;

    public PlayerDetectedState(EnemyBase entity, FiniteStateMachine stateMachine, string animBoolName, D_PlayerDetected stateData) : base(entity, stateMachine, animBoolName)
    {
        this.stateData = stateData;
    }

    public override void DoChecks()
    {
        base.DoChecks();
        isPlayerInMinAgroRange = entity.CheckPlayerInMinAgroRange();
        //isPlayerInMaxAgroRange = entity.CheckPlayerInMaxAgroRange();
        isPlayerInMinAttackRange = entity.CheckPlayerInMinAttackRange();
    }

    public override void Enter()
    {
        base.Enter();
        doRangedAttack = false;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        player = Object.FindObjectOfType<PlayerController>().gameObject;
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
