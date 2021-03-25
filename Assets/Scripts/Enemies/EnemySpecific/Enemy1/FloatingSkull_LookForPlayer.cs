using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingSkull_LookForPlayer : LookForPlayerState
{
    private FloatingSkull enemy;

    public FloatingSkull_LookForPlayer(EnemyBase entity, FiniteStateMachine stateMachine, string animBoolName, D_LookForPlayer stateData, FloatingSkull enemy) : base(entity, stateMachine, animBoolName, stateData)
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
        } 
        
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
