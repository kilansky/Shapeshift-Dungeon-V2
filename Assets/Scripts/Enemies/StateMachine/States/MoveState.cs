using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveState : State
{
    protected D_MoveState stateData;

    protected bool hasHitWall;
    protected bool hasHitLedge;
    protected bool isPlayerInMinAgroRange;

    public MoveState(EnemyBase entity, FiniteStateMachine stateMachine, string animBoolName, D_MoveState stateData) : base(entity, stateMachine, animBoolName)
    {
        this.stateData = stateData;
    }

    public override void DoChecks()
    {
        base.DoChecks();

        //when we start, check if character is in front of wall or ledge
        hasHitLedge = entity.CheckLedge();
        hasHitWall = entity.CheckWall();
        isPlayerInMinAgroRange = entity.CheckPlayerInMinAgroRange();
    }

    //in case we need to override one of the functions in 
    //our State class 
    public override void Enter()
    {
        //calls function from State
        base.Enter();
        entity.SetDestination();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
