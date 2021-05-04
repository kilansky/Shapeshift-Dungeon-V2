using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton_LookForPlayer : LookForPlayerState
{
    private Skeleton enemy;

    public Skeleton_LookForPlayer(EnemyBase entity, FiniteStateMachine stateMachine, string animBoolName, D_LookForPlayer stateData, Skeleton enemy) : base(entity, stateMachine, animBoolName, stateData)
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
