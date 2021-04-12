using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Goblin_AttackState : AttackState
{
    private Goblin enemy;
    
    protected Transform attackPosition;

    public GameObject meleeHitBox;

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
        base.Enter();

        //how much damage do they do, and where does it come from
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (enemy.CheckPlayerInMinAttackRange())
        {
            //TODO double check logic here 
            TriggerAttack();
        }
        else
        {
            stateMachine.ChangeState(enemy.moveState);
            //stateMachine.ChangeState(enemy.lookForPlayerState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public override void TriggerAttack()
    {
        base.TriggerAttack();
        Debug.Log("he triggering the attack rn");
        enemy.Anim.SetBool("isAttacking", true);
        //hit the player
        //meleeHitBox.GetComponent<MeshCollider>().enabled = true;
        //turn on mesh collider hit box for melee attack
        //meleeHitBox.GetComponent<MeshRenderer>().enabled = true;
        
        //swipe at the player

    }
}
