using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Goblin_AttackState : AttackState
{
    private Goblin enemy;

    //protected AttackDetails attackDetails;

    public Goblin_AttackState(EnemyBase entity, FiniteStateMachine stateMachine, string animBoolName, D_AttackState stateData, Goblin enemy) : base(entity, stateMachine, animBoolName, stateData)
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
            //TODO double check logic here 
            TriggerAttack();
        }
        else
        {
            stateMachine.ChangeState(enemy.moveState);
            //stateMachine.ChangeState(enemy.lookForPlayerState);
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
        
        //swipe at the player

    }
}
