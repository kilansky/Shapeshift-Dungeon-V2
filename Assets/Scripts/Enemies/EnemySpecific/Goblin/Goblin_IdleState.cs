using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goblin_IdleState : IdleState
{
    private Goblin enemy;

    public Goblin_IdleState(EnemyBase entity, FiniteStateMachine stateMachine, string animBoolName, D_IdleState stateData, Goblin enemy) : base(entity, stateMachine, animBoolName, stateData)
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
        //Debug.Log("i'm waiting for " + idleTime + " seconds");
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        //is player in attack range
        if (enemy.CheckPlayerInMinAgroRange())
        {
            //Debug.Log("I see the player");
            stateMachine.ChangeState(enemy.playerDetectedState);
        }
        else if (isIdleTimeOver) //if player out of attack range, move
        {
            //Debug.Log("Imma move now");
            stateMachine.ChangeState(enemy.moveState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
