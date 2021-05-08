using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worm_IdleState : IdleState
{
    private Worm enemy;
    private float timeElapsed;
    private float timeElapsedToEmerge;
    private float timeToEmerge = 2f;
    private bool isEmerging;

    public Worm_IdleState(EnemyBase entity, FiniteStateMachine stateMachine, string animBoolName, D_IdleState stateData, Worm enemy) : base(entity, stateMachine, animBoolName, stateData)
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

        timeElapsed = 0f;
        timeElapsedToEmerge = 0f;
        enemy.wormIsMoving = true;
        isEmerging = false;
        enemy.dirtCloud.Play();
        enemy.dirtChunks.Play();
        enemy.healthCanvas.SetActive(false);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if(isEmerging)
            timeElapsed += Time.deltaTime;
        else
            timeElapsedToEmerge += Time.deltaTime;

        //Wait to move worm up from ground
        if (timeElapsedToEmerge >= timeToEmerge)
        {
            isEmerging = true;
            enemy.Anim.SetBool("isBelowGround", false);
            enemy.healthCanvas.SetActive(true);
        }


        //Move worm up from the ground
        if (isEmerging && timeElapsed <= enemy.timeToEmergeOrSubmerge)
        {
            float wormYPos = Mathf.Lerp(enemy.underGroundYPos, enemy.aboveGroundYPos, timeElapsed / enemy.timeToEmergeOrSubmerge);
            enemy.wormMover.transform.position = new Vector3(enemy.wormMover.transform.position.x, wormYPos, enemy.wormMover.transform.position.z);

            float dirtYPos = Mathf.Lerp(enemy.dirtUnderGroundYPos, enemy.dirtAboveGroundYPos, timeElapsed / (enemy.timeToEmergeOrSubmerge/2));
            enemy.dirtCircle.transform.position = new Vector3(enemy.dirtCircle.transform.position.x, dirtYPos, enemy.dirtCircle.transform.position.z);
        }

        //If worm is fully moved up, check to attack
        if (timeElapsed > enemy.timeToEmergeOrSubmerge)
        {
            enemy.wormIsMoving = false;

            //is player in attack range
            if (enemy.CheckPlayerInMinAttackRange())
                stateMachine.ChangeState(enemy.attackState);
        }      
        
        //if worm is idle for too long, go to move state    
        if (isIdleTimeOver)
            stateMachine.ChangeState(enemy.moveState);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
