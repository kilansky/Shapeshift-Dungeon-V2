using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton_MoveState : MoveState
{
    private Skeleton enemy;

    public Skeleton_MoveState(EnemyBase entity, FiniteStateMachine stateMachine, string animBoolName, D_MoveState stateData, Skeleton enemy) : base(entity, stateMachine, animBoolName, stateData)
    {
        this.enemy = enemy;
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();
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
