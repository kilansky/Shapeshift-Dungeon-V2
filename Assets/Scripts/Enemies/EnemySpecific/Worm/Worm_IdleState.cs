using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worm_IdleState : IdleState
{
    private Worm enemy;

    public Worm_IdleState(EnemyBase entity, FiniteStateMachine stateMachine, string animBoolName, D_IdleState stateData, Worm enemy) : base(entity, stateMachine, animBoolName, stateData)
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
