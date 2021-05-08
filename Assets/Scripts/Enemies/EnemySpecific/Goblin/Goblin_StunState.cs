using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goblin_StunState : StunState
{
    private Goblin enemy;

    public Goblin_StunState(EnemyBase entity, FiniteStateMachine stateMachine, string animBoolName, D_StunState stateData, Goblin enemy) : base(entity, stateMachine, animBoolName, stateData)
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
        enemy.Anim.SetBool("isMoving", false);
        enemy.Anim.SetBool("isAttacking", false);

        enemy.aliveGO.GetComponent<EnemyAttack>().DisableHitBox();
        enemy.agent.SetDestination(enemy.transform.position);
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
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
