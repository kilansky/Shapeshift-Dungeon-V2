using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worm_LookForPlayer : LookForPlayerState
{
    private Worm enemy;

    public Worm_LookForPlayer(EnemyBase entity, FiniteStateMachine stateMachine, string animBoolName, D_LookForPlayer stateData, Worm enemy) : base(entity, stateMachine, animBoolName, stateData)
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
