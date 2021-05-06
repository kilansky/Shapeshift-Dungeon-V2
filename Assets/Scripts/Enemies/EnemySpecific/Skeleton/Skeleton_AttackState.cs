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
        enemy.shield.GetComponent<BoxCollider>().enabled = false;
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
            base.Enter();
            enemy.isAttacking = true;
        }
        else
        {
            //stateMachine.ChangeState(enemy.moveState);
            stateMachine.ChangeState(enemy.lookForPlayerState);
            enemy.isBlocking = true;
            enemy.shield.GetComponent<BoxCollider>().enabled = true;
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
