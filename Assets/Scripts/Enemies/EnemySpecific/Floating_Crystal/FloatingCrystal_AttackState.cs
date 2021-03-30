using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingCrystal_AttackState : AttackState
{
    private FloatingCrystal enemy;
    //public float rotateSpeed = 5f;
    //private Laser laser;


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
        //laser = enemy.GetComponent<Laser>();
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
        TriggerAttack();

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
    }

    public override void TriggerAttack()
    {
        //canAttack = false;
        base.TriggerAttack();
        enemy.Anim.SetBool("isAttacking", true);
        //still needs to move, but needs to attack now too
        enemy.Patrol();
        //enemy.transform.Rotate(0, rotateSpeed, 0);
        //fire the lasers from the front and back
        //GameObject laser = Instantiate(LaserBeam_Blue, firePointFront.transform.position, firePointFront.transform.rotation, transform);
        //GameObject laser = Instantiate(LaserBeam_Blue, firePointBack.transform.position, firePointBack.transform.rotation, transform);

        //rotate the crystal so it helicopters around the scene

    }
}
