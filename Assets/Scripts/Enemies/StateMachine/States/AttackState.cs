using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : State
{
    protected D_AttackState stateData;

    protected bool playerInAttackRange;

    public AttackState(EnemyBase entity, FiniteStateMachine stateMachine, string animBoolName, D_AttackState stateData) : base(entity, stateMachine, animBoolName)
    {
        this.stateData = stateData;
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();

        //isChargeTimeOver = false;
        entity.agent.SetDestination(entity.transform.position);
        TriggerAttack();
        //playerInAttackRange = entity.CheckPlayerInMinAttackRange();
        //entity.SetVelocity(0f);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public virtual void TriggerAttack()
    {

    }

}
