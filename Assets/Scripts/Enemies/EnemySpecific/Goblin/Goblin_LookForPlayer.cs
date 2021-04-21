using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goblin_LookForPlayer : LookForPlayerState
{
    private Goblin enemy;

    public Goblin_LookForPlayer(EnemyBase entity, FiniteStateMachine stateMachine, string animBoolName, D_LookForPlayer stateData, Goblin enemy) : base(entity, stateMachine, animBoolName, stateData)
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

        //if we look for the player, and can still see him, switch to detected
        if (isPlayerInMinAgroRange)
        {
            //TODO: double check logic here
            stateMachine.ChangeState(enemy.playerDetectedState);
        }else if (!isPlayerInMinAgroRange)
        {
            stateMachine.ChangeState(enemy.moveState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
