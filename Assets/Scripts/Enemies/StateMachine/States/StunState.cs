using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunState : State
{
    protected D_StunState stateData;

    protected bool isStunTimeOver;
    protected bool isStopped;
    protected bool isPlayerInMinAttackRange;

    public StunState(EnemyBase entity, FiniteStateMachine stateMachine, string animBoolName, D_StunState stateData) : base(entity, stateMachine, animBoolName)
    {
        this.stateData = stateData;
    }

    public override void DoChecks()
    {
        base.DoChecks();
        isPlayerInMinAttackRange = entity.CheckPlayerInMinAttackRange();
    }

    public override void Enter()
    {
        base.Enter();
        //Debug.Log("holy shit i made it to stun state");
        //stun enemy 
        isStunTimeOver = false;
        isStopped = false;
    }

    public override void Exit()
    {
        base.Exit();
        entity.ResetStunResistance();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        //set timer to exit stun state
        if(Time.time >= startTime + stateData.stunTime)
        {
            isStunTimeOver = true;
        }//stun the enemy, set velocity to 0, and flip bool
        if(Time.time >= startTime + stateData.stunKnockbackTime && !isStopped)
        {
            isStopped = true;
            //entity.SetVelocity(0f);
        }  
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
