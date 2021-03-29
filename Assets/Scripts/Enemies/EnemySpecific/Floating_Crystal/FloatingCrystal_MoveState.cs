using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FloatingCrystal_MoveState : MoveState
{
    //movement for the floating crystal
    private FloatingCrystal enemy;
    //target will be other crystals in the scene
    public Transform target;
    FloatingCrystal[] crystals = Object.FindObjectsOfType<FloatingCrystal>();

    public FloatingCrystal_MoveState(EnemyBase entity, FiniteStateMachine stateMachine, string animBoolName, D_MoveState stateData, FloatingCrystal enemy) : base(entity, stateMachine, animBoolName, stateData)
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
        
        //enemy.Patrol();
    }
    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        //should be if no other crystals in the scene, move to attack state
        /*if (enemy.CheckPlayerInMinAttackRange())
        {
            stateMachine.ChangeState(enemy.attackState);
        }*/
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        enemy.Patrol();
    }

}
