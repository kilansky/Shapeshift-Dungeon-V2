using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FloatingSkull_AttackState : AttackState
{
    //attack state for the Floating Skull
    private FloatingSkull enemy;
    private SkullCharge charge;

    public float shootForce;
    public float timeBetweenAttacks;

    public FloatingSkull_AttackState(EnemyBase entity, FiniteStateMachine stateMachine, string animBoolName, D_AttackState stateData, FloatingSkull enemy) : base(entity, stateMachine, animBoolName, stateData)
    {
        this.enemy = enemy;
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        charge = enemy.aliveGO.GetComponent<SkullCharge>();

        base.Enter();
        
        //attacking = true;

        //Vector3 targetPoint;
        //targetPoint = entity.player.transform.localposition;

        //TriggerAttack();
        //StartCoroutine(BulletCharge(bullet));
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!charge.isAttacking)
        {
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
        else
        {
            Vector3 direction = (enemy.player.transform.position - enemy.transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public override void TriggerAttack()
    {
        enemy.Anim.SetBool("isAttacking", true);
        charge.Attack(enemy.fireball, enemy.firePoint.transform, enemy.chargeRate);
    }

}

