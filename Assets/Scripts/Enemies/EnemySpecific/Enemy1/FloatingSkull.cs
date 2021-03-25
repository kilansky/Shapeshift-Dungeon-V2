using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]

//add Idamageable later
public class FloatingSkull : EnemyBase
{
    public FloatingSkull_IdleState idleState { get; private set; }
    public FloatingSkull_MoveState moveState { get; private set; }
    public FloatingSkull_PlayerDetected playerDetectedState { get; private set; }
    public FloatingSkull_AttackState attackState { get; private set; }
    public FloatingSkull_LookForPlayer lookForPlayerState { get; private set; }
    public FloatingSkull_StunState stunState { get; private set; }


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

    public Transform firePoint;
    public GameObject fireball;
    public float chargeRate = 1f;

    public override void Start()
    {
        base.Start();

        moveState = new FloatingSkull_MoveState(this, stateMachine, "move", moveStateData, this);
        idleState = new FloatingSkull_IdleState(this, stateMachine, "idle", idleStateData, this);
        playerDetectedState = new FloatingSkull_PlayerDetected(this, stateMachine, "playerDetected", playerDetectedData, this);
        attackState = new FloatingSkull_AttackState(this, stateMachine, "attack", /*firePoint,*/ attackStateData, this);
        lookForPlayerState = new FloatingSkull_LookForPlayer(this, stateMachine, "lookForPlayer", lookForPlayerStateData, this);
        stunState = new FloatingSkull_StunState(this, stateMachine, "stun", stunStateData, this);

        //this line is what got rid of my NullReferenceExceptions
        stateMachine.Initialize(moveState);
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

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("KillBox"))
        {
            Damage(100);
            Debug.Log("MONSTER TOUCHED KILLBOX");
        }
    }
}
