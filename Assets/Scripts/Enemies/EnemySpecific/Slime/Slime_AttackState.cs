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
            //TODO double check logic here 
            base.Enter();//Trigger Attack and stop movement
            enemy.isAttacking = true;
        }
        else
        {
            //stateMachine.ChangeState(enemy.moveState);
            stateMachine.ChangeState(enemy.lookForPlayerState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public override void TriggerAttack()
    {
        base.TriggerAttack();

        enemy.Anim.SetBool("isAttacking", true);
    }
}
