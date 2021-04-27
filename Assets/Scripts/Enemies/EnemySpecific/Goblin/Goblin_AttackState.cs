using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Goblin_AttackState : AttackState
{
    private Goblin enemy;
    //private Swipe attack;


    //protected Transform attackPosition;

    //public GameObject meleeHitBox;
    //public GameObject meleeVFX;
   //public bool showHitBoxes = false;


    public Goblin_AttackState(EnemyBase entity, FiniteStateMachine stateMachine, string animBoolName, D_AttackState stateData, Goblin enemy) : base(entity, stateMachine, animBoolName, stateData)
    {
        this.enemy = enemy;
        //this.attackPosition = attackPosition;
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        //attack = enemy.aliveGO.GetComponent<Swipe>();
        //how much damage do they do, and where does it come from
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (enemy.CheckPlayerInMinAttackRange() && !enemy.isAttacking)
        {
            //TODO double check logic here 
            base.Enter();
            enemy.isAttacking = true;
        }
        else
        {
            //stateMachine.ChangeState(enemy.moveState);
            stateMachine.ChangeState(enemy.lookForPlayerState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public override void TriggerAttack()
    {
        base.TriggerAttack();

        enemy.Anim.SetBool("isAttacking", true);
        //enemy.isAttackOver = true;
        //attack.MeleeAttack();

        /*
        //hit the player
        //turn on mesh collider hit box for melee attack
        meleeHitBox.GetComponent<MeshCollider>().enabled = true;
        //show the hitbox for the attack
        if (showHitBoxes)
            meleeHitBox.GetComponent<MeshRenderer>().enabled = true;
        */
        //swipe at the player

    }
}
