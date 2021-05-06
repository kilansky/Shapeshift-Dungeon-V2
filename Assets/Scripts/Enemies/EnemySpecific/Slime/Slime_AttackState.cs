using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime_AttackState : AttackState
{
    private Slime enemy;

    public Slime_AttackState(EnemyBase entity, FiniteStateMachine stateMachine, string animBoolName, D_AttackState stateData, Slime enemy) : base(entity, stateMachine, animBoolName, stateData)
    {
        this.enemy = enemy;
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {

    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (enemy.CheckPlayerInMinAttackRange() && !enemy.isAttacking)
        {
            TriggerAttack();
            enemy.SetDestination();
        }
        else if(!enemy.isAttacking)
        {
            stateMachine.ChangeState(enemy.moveState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public override void TriggerAttack()
    {
        base.TriggerAttack();
        enemy.isAttacking = true;
        enemy.Anim.SetBool("isAttacking", true);
    }
}
