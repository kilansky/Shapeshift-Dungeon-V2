using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goblin_PlayerDetected : PlayerDetectedState
{
    private Goblin enemy;

    public Goblin_PlayerDetected(EnemyBase entity, FiniteStateMachine stateMachine, string animBoolName, D_PlayerDetected stateData, Goblin enemy) : base(entity, stateMachine, animBoolName, stateData)
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
