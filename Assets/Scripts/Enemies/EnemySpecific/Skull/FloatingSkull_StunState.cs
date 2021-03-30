using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingSkull_StunState : StunState
{
    private FloatingSkull enemy;

    public FloatingSkull_StunState(EnemyBase entity, FiniteStateMachine stateMachine, string animBoolName, D_StunState stateData, FloatingSkull enemy) : base(entity, stateMachine, animBoolName, stateData)
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
        //Debug.Log("even more holy shit i made it to FS_stun state");
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (isStunTimeOver)
        {
            if (isPlayerInMinAttackRange)
            {
                stateMachine.ChangeState(enemy.playerDetectedState);
            }
            else
            {
                stateMachine.ChangeState(enemy.lookForPlayerState);
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}

