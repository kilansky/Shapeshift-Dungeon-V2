using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Goblin_IdleState : IdleState
{
    private Goblin enemy;
    private float timeElapsed = 0f;
    private float timeToCheckForPlayer = 0.5f;

    public Goblin_IdleState(EnemyBase entity, FiniteStateMachine stateMachine, string animBoolName, D_IdleState stateData, Goblin enemy) : base(entity, stateMachine, animBoolName, stateData)
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
        enemy.agent.SetDestination(enemy.transform.position);
        //Debug.Log("i'm waiting for " + idleTime + " seconds");
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        timeElapsed += Time.deltaTime;

        //Wait 0.5 seconds before checking if there is a path to the player (too expensive to do each frame)
        if(timeElapsed >= timeToCheckForPlayer)
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
