using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]

//add Idamageable later
public class Goblin : EnemyBase
{
    //this script contains all the states of the floating skull
    public Goblin_IdleState idleState { get; private set; }
    public Goblin_MoveState moveState { get; private set; }
    public Goblin_PlayerDetected playerDetectedState { get; private set; }
    public Goblin_AttackState attackState { get; private set; }
    public Goblin_LookForPlayer lookForPlayerState { get; private set; }
    public Goblin_StunState stunState { get; private set; }


    [SerializeField]
    private D_IdleState idleStateData;
    [SerializeField]
    private D_MoveState moveStateData;
    [SerializeField]
    private D_PlayerDetected playerDetectedData;
    [SerializeField]
    private D_AttackState attackStateData;
    [SerializeField]
    private D_LookForPlayer lookForPlayerStateData;
    [SerializeField]
    private D_StunState stunStateData;

    //will have a melee range instead of fireball
    

    public override void Start()
    {
        SetNewTarget(player);
        base.Start();

        moveState = new Goblin_MoveState(this, stateMachine, "move", moveStateData, this);
        idleState = new Goblin_IdleState(this, stateMachine, "idle", idleStateData, this);
        playerDetectedState = new Goblin_PlayerDetected(this, stateMachine, "playerDetected", playerDetectedData, this);
        attackState = new Goblin_AttackState(this, stateMachine, "attack", attackStateData, this);
        lookForPlayerState = new Goblin_LookForPlayer(this, stateMachine, "lookForPlayer", lookForPlayerStateData, this);
        stunState = new Goblin_StunState(this, stateMachine, "stun", stunStateData, this);

        //initialize the goblin in the idle state
        stateMachine.Initialize(idleState);

        //this line is what got rid of my NullReferenceExceptions
        //stateMachine.Initialize(moveState);

    }

    //set current state to stunState if isStunned
    public override void Damage(float damage)
    {
        base.Damage(damage);

        /*if (isDead)
        {
            stateMachine.ChangeState(deadState);
        }*/
        if (isStunned && stateMachine.currentState != stunState)
        {
            stateMachine.ChangeState(stunState);
        }
    }
}