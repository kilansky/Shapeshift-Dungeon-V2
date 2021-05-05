using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worm_IdleState : IdleState
{
    private Worm enemy;
    private float timeElapsed;

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
        enemy.wormIsMoving = true;
        enemy.Anim.SetBool("isBelowGround", false);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        timeElapsed += Time.deltaTime;

        //Move worm up from the ground
        if (timeElapsed <= enemy.timeToEmergeOrSubmerge)
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
