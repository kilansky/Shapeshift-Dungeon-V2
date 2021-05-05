using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worm_MoveState : MoveState
{
    private Worm enemy;
    private float timeElapsed;

    public Worm_MoveState(EnemyBase entity, FiniteStateMachine stateMachine, string animBoolName, D_MoveState stateData, Worm enemy) : base(entity, stateMachine, animBoolName, stateData)
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
        enemy.Anim.SetBool("isBelowGround", true);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        timeElapsed += Time.deltaTime;

        //Move worm down into the ground
        if(timeElapsed < enemy.timeToEmergeOrSubmerge)
        {
            float wormYPos = Mathf.Lerp(enemy.aboveGroundYPos, enemy.underGroundYPos, timeElapsed / enemy.timeToEmergeOrSubmerge);
            enemy.wormMover.transform.position = new Vector3(enemy.wormMover.transform.position.x, wormYPos, enemy.wormMover.transform.position.z);

            if (timeElapsed >= enemy.timeToEmergeOrSubmerge/2)//Wait until the worm is halfway into the ground before moving dirt down
            {
                float dirtYPos = Mathf.Lerp(enemy.dirtAboveGroundYPos, enemy.dirtUnderGroundYPos, (timeElapsed - (enemy.timeToEmergeOrSubmerge / 2)) / (enemy.timeToEmergeOrSubmerge/2));
                enemy.dirtCircle.transform.position = new Vector3(enemy.dirtCircle.transform.position.x, dirtYPos, enemy.dirtCircle.transform.position.z);
            }
        }

        //The worm is fully in the ground
        if(timeElapsed >= enemy.timeToEmergeOrSubmerge)
        {
            //Quench flames if burning underground
            if(enemy.GetComponent<StatusEffects>().isBurning)
                enemy.GetComponent<StatusEffects>().currTime = 10;

            //Teleport
            Transform safeTeleportTile = enemy.FindSafeTeleportTile();
            if (safeTeleportTile)
            {
                //Set enemy root position to the new tile
                Vector3 teleportPos = safeTeleportTile.position + new Vector3(0f, 5f, 0f);
                enemy.transform.position = teleportPos;

                //Modify the above and under ground y positions of the worm and dirt circle
                enemy.aboveGroundYPos = teleportPos.y;
                enemy.underGroundYPos = enemy.aboveGroundYPos - enemy.amountToMoveWorm;
                enemy.dirtAboveGroundYPos = teleportPos.y;
                enemy.dirtUnderGroundYPos = enemy.aboveGroundYPos - enemy.amountToMoveDirt;

                //Change the Material of the dirt circle according to the type of tile
                if (safeTeleportTile.GetComponent<Tile>().tileType == Tile.tileTypes.dirt)
                    enemy.dirtCircle.GetComponent<Renderer>().material = enemy.dirtMat;//set dirt mat
                else
                    enemy.dirtCircle.GetComponent<Renderer>().material = enemy.sandMat;//set sand mat
            }
            else
                Debug.Log("Worm Failed To Telport");

            stateMachine.ChangeState(enemy.idleState);
        }

        base.LogicUpdate();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
