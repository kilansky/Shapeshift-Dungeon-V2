using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingCrystal_AttackState : AttackState
{
    private FloatingCrystal enemy;



    public FloatingCrystal_AttackState(EnemyBase entity, FiniteStateMachine stateMachine, string animBoolName, D_AttackState stateData, FloatingCrystal enemy) : base(entity, stateMachine, animBoolName, stateData)
    {
        this.enemy = enemy;
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        //attack the player
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        //TODO: add another if statement for if already attacking
        if (enemy.CheckPlayerInMinAttackRange())
        {
            TriggerAttack();
        }
        else
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
        //canAttack = false;
        base.TriggerAttack();
    }
}
