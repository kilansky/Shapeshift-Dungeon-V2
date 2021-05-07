using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Skeleton_AttackState : AttackState
{
    private Skeleton enemy;

    public Skeleton_AttackState(EnemyBase entity, FiniteStateMachine stateMachine, string animBoolName, D_AttackState stateData, Skeleton enemy) : base(entity, stateMachine, animBoolName, stateData)
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
        enemy.isBlocking = false;
        enemy.Anim.SetBool("isStunned", false);
        enemy.Anim.SetBool("isBlocking", false);
        //enemy.shield.GetComponent<BoxCollider>().enabled = false;
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
            //TODO double check logic here 
            //base.Enter();
        }
        else if(!enemy.isAttacking)
        {
            //stateMachine.ChangeState(enemy.moveState);
            stateMachine.ChangeState(enemy.moveState);
            //enemy.isBlocking = true;
            //enemy.Anim.SetBool("isAttacking", false);
            //enemy.shield.GetComponent<BoxCollider>().enabled = true;
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
