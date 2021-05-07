using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Skeleton_MoveState : MoveState
{
    private Skeleton enemy;

    int num = Random.Range(1, 11);

    private float timeElapsed = 0f;
    private float timeToCheckForPlayer = 1f;

    //skeleton should be moving with shield up

    public Skeleton_MoveState(EnemyBase entity, FiniteStateMachine stateMachine, string animBoolName, D_MoveState stateData, Skeleton enemy) : base(entity, stateMachine, animBoolName, stateData)
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
        enemy.Anim.SetBool("isAttacking", false);
        enemy.Anim.SetBool("isBlocking", false);
        enemy.Anim.SetBool("isMoving", true);
        entity.SetVelocity(stateData.moveSpeed);

        //create a random number generator 1-10
        //set back to be #1-5, sides to be #6-8, front to be #9,10
        //set new target to whichever target gets chosen from the number generator
        if (num < 6)
        {
            if (!enemy.isKnockedBack)
            {
                //attack back
                enemy.SetNewTarget(enemy.BackTarget);
                entity.SetNewDestination();
                enemy.HaveLineOfSight();
                //Debug.Log("I'm going to backstab the player");
            }
        }
        else if (num >= 6 || num < 9 && !enemy.isKnockedBack)
        {
            //attack sides
            if (!enemy.isKnockedBack)
            {
                enemy.SetNewTarget(enemy.SideTarget);
                entity.SetNewDestination();
                enemy.HaveLineOfSight();
                //Debug.Log("I'm attacking from the side");
            }
        }
        else if (num >= 9 && !enemy.isKnockedBack)
        {
            if (!enemy.isKnockedBack)
            {
                //attack front
                enemy.SetNewTarget(enemy.FrontTarget);
                entity.SetNewDestination();
                enemy.HaveLineOfSight();
                //Debug.Log("I'm going straight at the player");
            }
        }
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        /*if (enemy.CheckPlayerInMinAgroRange())
        {
            //go to player detected state next and have that transistion to attack
            stateMachine.ChangeState(enemy.playerDetectedState);
        }*/

        timeElapsed += Time.deltaTime;

        //Wait 1 second before checking if there is a path to the player (too expensive to do each frame)
        if (timeElapsed >= timeToCheckForPlayer)
        {
            //Calculate a new path and see if it was a complete path to the player
            NavMeshPath newPath = new NavMeshPath();
            enemy.agent.CalculatePath(enemy.player.transform.position, newPath);

            if (newPath.status != NavMeshPathStatus.PathComplete)
            {
                //Debug.Log("Found not found, going back to idle");
                enemy.Anim.SetBool("isMoving", false);
                stateMachine.ChangeState(enemy.idleState);
            }

            timeElapsed = 0f;
        }

        if (enemy.distanceToPlayer < 3f)
        {
            enemy.isBlocking = true;
            enemy.Anim.SetBool("isBlocking", true);
            //enemy.Anim.SetBool("isMoving", false);
            if (enemy.CheckPlayerInMinAttackRange())
            {
                //Debug.Log("I'm attacking!!");
                stateMachine.ChangeState(enemy.attackState);
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        //Update the position of the target to move to
        entity.SetNewDestination();
    }
}
