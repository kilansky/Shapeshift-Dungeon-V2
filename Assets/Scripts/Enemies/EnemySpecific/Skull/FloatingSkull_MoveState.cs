using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingSkull_MoveState : MoveState
{
    private FloatingSkull enemy;

    public FloatingSkull_MoveState(EnemyBase entity, FiniteStateMachine stateMachine, string animBoolName, D_MoveState stateData, FloatingSkull enemy) : base(entity, stateMachine, animBoolName, stateData)
    {
        this.enemy = enemy;
    }

    public override void Enter()
    {
        base.Enter();
        //player = GameObject.FindWithTag("Player");
        //SetNewTarget(player);
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
            stateMachine.ChangeState(enemy.attackState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        entity.SetDestination();
        //entity.SetVelocity(stateData.moveSpeed);
    }
}
