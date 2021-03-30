using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingCrystal_AttackState : AttackState
{
    private FloatingCrystal enemy;
    [HideInInspector] public float rotateSpeed = 3f;
    private LaserDispenser laser;
    float timeElapsed;
    float attackDuration = 8f;
    bool isAttacking = true;

    public FloatingCrystal_AttackState(EnemyBase entity, FiniteStateMachine stateMachine, string animBoolName, D_AttackState stateData, FloatingCrystal enemy) : base(entity, stateMachine, animBoolName, stateData)
    {
        this.enemy = enemy;
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        //attack the player
        base.Enter();
        isAttacking = true;
        Debug.Log("enemy is " + enemy.name);
        laser = enemy.GetComponent<LaserDispenser>();
        TriggerAttack();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        //TODO: add another if statement for if already attacking
        //if no other crystals in the room fire 2 beams from front
        //and back that are a fixed length, and spin

        if (isAttacking)
            timeElapsed += Time.deltaTime;

        if (timeElapsed >= attackDuration && isAttacking)
        {
            isAttacking = false;
            enemy.Anim.SetBool("isAttacking", false);
            timeElapsed = 0;
            stateMachine.ChangeState(enemy.moveState);
        }

        /*if (enemy.CheckPlayerInMinAttackRange())
        {
            TriggerAttack();
        }
        else
        {
            stateMachine.ChangeState(enemy.moveState);
        }*/
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        if(isAttacking)
            enemy.transform.Rotate(0, rotateSpeed, 0);
    }

    public override void TriggerAttack()
    {
        //canAttack = false;
        base.TriggerAttack();
        enemy.Anim.SetBool("isAttacking", true);
        
        //fire the lasers from the front and back
        enemy.GetComponent<LaserDispenser>().ToggleLaser(true);
    }

}
