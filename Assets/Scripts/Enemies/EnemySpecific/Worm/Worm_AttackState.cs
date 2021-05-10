using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worm_AttackState : AttackState
{
    private Worm enemy;

    public Worm_AttackState(EnemyBase entity, FiniteStateMachine stateMachine, string animBoolName, D_AttackState stateData, Worm enemy) : base(entity, stateMachine, animBoolName, stateData)
    {
        this.enemy = enemy;
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        //base.Enter();
        TriggerAttack();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        //Debug.Log("wormIsAttacking: " + enemy.isAttacking);

        //Attack if the player is in range, and the worm is not attacking or on fire
        if (enemy.CheckPlayerInMinAttackRange() && !enemy.isAttacking && !enemy.GetComponent<StatusEffects>().isBurning)
        {
            TriggerAttack();
        }
        //If player out of range and the worm is no longer attacking, move to new position
        else if (!enemy.isAttacking)
            stateMachine.ChangeState(enemy.moveState);
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
