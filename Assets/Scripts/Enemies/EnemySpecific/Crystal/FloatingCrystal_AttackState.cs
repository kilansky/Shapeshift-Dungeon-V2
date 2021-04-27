using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingCrystal_AttackState : AttackState
{
    private FloatingCrystal enemy;
    private LaserDispenser laser;
    float timeElapsed;
    float attackDuration = 8f;
    float attackStartupTime;
    bool isAttacking = false;
    bool isChargingUp = true;

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

        //Debug.Log("enemy is " + enemy.name);
        laser = enemy.GetComponent<LaserDispenser>();
        attackStartupTime = laser.startupTime;
        isChargingUp = true;
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

        //Increase time elapsed
        if (isAttacking || isChargingUp)
            timeElapsed += Time.deltaTime;

        //Brighten crystal pointlight while charging
        if (isChargingUp && timeElapsed < attackStartupTime)
            enemy.pointLight.intensity = Mathf.Lerp(5f, 8f, timeElapsed / attackStartupTime);

        //Charging Up Laser Attack - Wait to begin rotating
        if (isChargingUp && timeElapsed >= attackStartupTime)
        {
            isChargingUp = false;
            isAttacking = true;
            enemy.pointLight.intensity = 10f;

            timeElapsed = 0;
        }

        //Waiting for attack duration to end the laser attack
        if (isAttacking && timeElapsed >= attackDuration)
        {
            isAttacking = false;
            //enemy.Anim.SetBool("isAttacking", false);
            enemy.pointLight.intensity = 5f;

            timeElapsed = 0;
            stateMachine.ChangeState(enemy.moveState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if(isAttacking)
            enemy.transform.Rotate(0, enemy.rotateSpeed, 0);
    }

    public override void TriggerAttack()
    {
        //canAttack = false;
        //enemy.Anim.SetBool("isAttacking", true);

        //fire the laser
        enemy.GetComponent<LaserDispenser>().ToggleLaser(true);
    }
}
