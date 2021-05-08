using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Skeleton_AttackState : AttackState
{
    private Skeleton enemy;

    private float initSpeed;

    public Skeleton_AttackState(EnemyBase entity, FiniteStateMachine stateMachine, string animBoolName, D_AttackState stateData, Skeleton enemy) : base(entity, stateMachine, animBoolName, stateData)
    {
        this.enemy = enemy;
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        //base.Enter();
        enemy.isBlocking = false;
        enemy.Anim.SetBool("isStunned", false);
        enemy.Anim.SetBool("isBlocking", false);

        enemy.agent.SetDestination(enemy.player.transform.position);
        initSpeed = enemy.agent.speed;
        enemy.agent.speed = 1;

        TriggerAttack();
    }

    public override void Exit()
    {
        base.Exit();
        enemy.agent.SetDestination(enemy.transform.position);
        enemy.agent.speed = initSpeed;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if(enemy.isAttacking)//move towards player slowly while attacking
            enemy.agent.SetDestination(enemy.player.transform.position);
        else//after attacking go back to move/block state
            stateMachine.ChangeState(enemy.moveState);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (enemy.isAttacking)
        {
            //Get direction to player
            Vector3 targetPoint = new Vector3(enemy.player.transform.position.x, 0, enemy.player.transform.position.z) - new Vector3(enemy.transform.position.x, 0, enemy.transform.position.z);
            Quaternion targetRotation = Quaternion.LookRotation(targetPoint);
            float rotSpeed = 4f;

            //Smoothly rotate towards player
            Quaternion lastTargetRotation = Quaternion.Slerp(enemy.transform.rotation, targetRotation, rotSpeed * Time.deltaTime);
            enemy.transform.rotation = lastTargetRotation;
        }
    }

    public override void TriggerAttack()
    {
        base.TriggerAttack();
        enemy.isAttacking = true;
        enemy.Anim.SetBool("isAttacking", true);
    }
}
