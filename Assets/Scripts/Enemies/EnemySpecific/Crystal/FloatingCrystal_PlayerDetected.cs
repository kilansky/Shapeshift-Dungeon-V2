using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingCrystal_PlayerDetected : PlayerDetectedState
{
    private FloatingCrystal enemy;

    public FloatingCrystal_PlayerDetected(EnemyBase entity, FiniteStateMachine stateMachine, string animBoolName, D_PlayerDetected stateData, FloatingCrystal enemy) : base(entity, stateMachine, animBoolName, stateData)
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
