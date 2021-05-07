using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Skeleton_IdleState : IdleState
{
    private Skeleton enemy;

    private float timeElapsed = 0f;
    private float timeToCheckForPlayer = 0.5f;

    public Skeleton_IdleState(EnemyBase entity, FiniteStateMachine stateMachine, string animBoolName, D_IdleState stateData, Skeleton enemy) : base(entity, stateMachine, animBoolName, stateData)
    {
        this.enemy = enemy;
    }

    public override void Enter()
    {
        base.Enter();
        enemy.isBlocking = false;
        enemy.Anim.SetBool("isStunned", false);
        enemy.Anim.SetBool("isMoving", false);
        enemy.Anim.SetBool("isBlocking", false);
        enemy.agent.SetDestination(enemy.transform.position);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        //is player in attack range
        /*if (enemy.CheckPlayerInMinAgroRange())
        {
            stateMachine.ChangeState(enemy.playerDetectedState);
        }
        else if (isIdleTimeOver) //if player out of attack range, move
        {
            stateMachine.ChangeState(enemy.moveState);
            enemy.isBlocking = true;
            enemy.Anim.SetBool("isBlocking", true);
        }*/

        timeElapsed += Time.deltaTime;

        //Wait 0.5 seconds before checking if there is a path to the player (too expensive to do each frame)
        if (timeElapsed >= timeToCheckForPlayer)
        {
            //Calculate a new path and see if it was a complete path to the player
            NavMeshPath newPath = new NavMeshPath();
            enemy.agent.CalculatePath(enemy.player.transform.position, newPath);

            if (newPath.status == NavMeshPathStatus.PathComplete)
            {
                //Debug.Log("Found complete path, moving to player");
                stateMachine.ChangeState(enemy.moveState);
            }

            timeElapsed = 0f;
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
