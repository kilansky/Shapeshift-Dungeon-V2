using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goblin_MoveState : MoveState
{
    private Goblin enemy;

    //set max value to 11, bc random.range won't return max value
    int num = Random.Range(1, 11);
    

    public Goblin_MoveState(EnemyBase entity, FiniteStateMachine stateMachine, string animBoolName, D_MoveState stateData, Goblin enemy) : base(entity, stateMachine, animBoolName, stateData)
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
        Debug.Log("oh he movin");
        Debug.Log("num is equal to " + num);

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
            Debug.Log("I'm swingin!!");
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
            //attack back
            enemy.SetNewTarget(enemy.BackTarget);
            entity.SetDestination();
            Debug.Log("I'm gonna backstab you");
        }else if (num > 5 || num < 9)
        {
            //attack sides
            enemy.SetNewTarget(enemy.SideTarget);
            entity.SetDestination();
            Debug.Log("Imma attack from the side");
        }else if (num > 8)
        {
            //attack front
            enemy.SetNewTarget(enemy.FrontTarget);
            entity.SetDestination();
            Debug.Log("I'm going straight at the player");
        }
        //set destination to the player, look check should go here
        //entity.SetDestination();

    }
}
