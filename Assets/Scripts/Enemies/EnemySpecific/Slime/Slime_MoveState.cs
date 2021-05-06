using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime_MoveState : MoveState
{
    private Slime enemy;

    public Slime_MoveState(EnemyBase entity, FiniteStateMachine stateMachine, string animBoolName, D_MoveState stateData, Slime enemy) : base(entity, stateMachine, animBoolName, stateData)
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
        //Debug.Log("oh he movin");
        //Debug.Log("num is equal to " + num);

        entity.SetVelocity(stateData.moveSpeed);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        enemy.SetDestination();

        if (enemy.CheckPlayerInMinAttackRange())
        {
            //Debug.Log("I'm attacking!!");
            stateMachine.ChangeState(enemy.attackState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (!enemy.isKnockedBack)
            entity.SetDestination();

        //set destination to the player, look check should go here
        //entity.SetDestination();
    }
}
