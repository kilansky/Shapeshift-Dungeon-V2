using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goblin_MoveState : MoveState
{
    private Goblin enemy;

    public Goblin_MoveState(EnemyBase entity, FiniteStateMachine stateMachine, string animBoolName, D_MoveState stateData, Goblin enemy) : base(entity, stateMachine, animBoolName, stateData)
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
        Debug.Log("oh he movin");

        entity.SetVelocity(stateData.moveSpeed);
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
            Debug.Log("I'm swingin!!");
            stateMachine.ChangeState(enemy.attackState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        //set destination to the player, look check should go here
        entity.SetDestination();

        //create a random number generator 1-10
        //set back to be #1-5, sides to be #6-8, front to be #9,10
        
    }
}
