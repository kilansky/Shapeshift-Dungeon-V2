using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton_StunState : StunState
{
    private Skeleton enemy;

    public Skeleton_StunState(EnemyBase entity, FiniteStateMachine stateMachine, string animBoolName, D_StunState stateData, Skeleton enemy) : base(entity, stateMachine, animBoolName, stateData)
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
        enemy.Anim.SetBool("isStunned", true);
        enemy.Anim.SetBool("isAttacking", false);
        enemy.isBlocking = false;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (isStunTimeOver)
        {
            if (isPlayerInMinAttackRange)
            {
                stateMachine.ChangeState(enemy.attackState);
            }
            else
            {
                stateMachine.ChangeState(enemy.moveState);
                enemy.isBlocking = true;
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

}
