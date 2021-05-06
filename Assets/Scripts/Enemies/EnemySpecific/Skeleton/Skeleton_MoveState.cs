using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton_MoveState : MoveState
{
    private Skeleton enemy;

    int num = Random.Range(1, 11);

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

        entity.SetVelocity(stateData.moveSpeed);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (enemy.CheckPlayerInMinAgroRange())
        {
            //go to player detected state next and have that transistion to attack
            stateMachine.ChangeState(enemy.playerDetectedState);
        }
        if (enemy.CheckPlayerInMinAttackRange())
        {
            //Debug.Log("I'm attacking!!");
            stateMachine.ChangeState(enemy.attackState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

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
}
