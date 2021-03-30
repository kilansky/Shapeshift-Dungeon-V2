using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingSkull_PlayerDetected : PlayerDetectedState
{
    private FloatingSkull enemy;

    public FloatingSkull_PlayerDetected(EnemyBase entity, FiniteStateMachine stateMachine, string animBoolName, D_PlayerDetected stateData, FloatingSkull enemy) : base(entity, stateMachine, animBoolName, stateData)
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

        //not every enemy attacks from distance, need to change conditions
        if (enemy.CheckPlayerInMinAttackRange())
        {
            //switch to attackstate
            stateMachine.ChangeState(enemy.attackState);
        }
        else if (!enemy.CheckPlayerInMinAttackRange())
        {
            //if too far away, go to the player
            stateMachine.ChangeState(enemy.moveState);
        }

    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
