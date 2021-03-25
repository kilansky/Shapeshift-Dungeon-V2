using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State 
{
    //State belongs to FiniteStateMachine, classes 
    //inheriting from this class will have access to stateMachine
    protected FiniteStateMachine stateMachine;
    //which enemy entity do you belong too?
    protected EnemyBase entity;
    //when did the enemy enter the state?
    protected float startTime;

    protected GameObject player;

    protected string animBoolName;

    public State(EnemyBase entity, FiniteStateMachine stateMachine, string animBoolName)
    {
        this.entity = entity;
        this.stateMachine = stateMachine;
        this.animBoolName = animBoolName;
    }

    //virtual - function can be redefined in derived classes
    public virtual void Enter()
    {
        startTime = Time.time;
        entity.Anim.SetBool(animBoolName, true);
        DoChecks();
    }

    public virtual void Exit()
    {
        entity.Anim.SetBool(animBoolName, false);
    }

    public virtual void LogicUpdate()
    {

    }

    public virtual void PhysicsUpdate()
    {
        DoChecks();
    }

    //adding in this function so I don't have to change
    //enter and physicsUpdate for any future changes
    public virtual void DoChecks()
    {

    }
}
