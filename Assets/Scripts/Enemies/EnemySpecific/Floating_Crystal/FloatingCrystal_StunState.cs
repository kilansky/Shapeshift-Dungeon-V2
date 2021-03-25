using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingCrystal_StunState : StunState
{
    private FloatingCrystal enemy;

    public FloatingCrystal_StunState(EnemyBase entity, FiniteStateMachine stateMachine, string animBoolName, D_StunState stateData, FloatingCrystal enemy) : base(entity, stateMachine, animBoolName, stateData)
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
