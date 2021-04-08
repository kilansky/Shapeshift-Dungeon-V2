using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingSkull_IdleState : IdleState
{
    private FloatingSkull enemy;

    public FloatingSkull_IdleState(EnemyBase entity, FiniteStateMachine stateMachine, string animBoolName, D_IdleState stateData, FloatingSkull enemy) : base(entity, stateMachine, animBoolName, stateData)
    {
        this.enemy = enemy;
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

        if (enemy.CheckPlayerInMinAgroRange())
        {
            stateMachine.ChangeState(enemy.playerDetectedState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
