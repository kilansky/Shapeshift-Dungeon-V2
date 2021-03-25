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
        charge = enemy.GetComponent<SkullCharge>();

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

    /*IEnumerator BulletCharge(GameObject bullet)
        {
            canAttack = false;
            //set movespeed to 0
            float bulletSpeed = bullet.GetComponent<Bullet>().moveSpeed;
            bullet.GetComponent<Bullet>().moveSpeed = 0f;

            float currentScale = bullet.GetComponent<Bullet>().minBulletSize;
            bullet.transform.localScale = new Vector3(currentScale, currentScale, currentScale);

            while (currentScale < bullet.GetComponent<Bullet>().maxBulletSize)
            {
                //scale bullet until it reaches max bullet size
                currentScale += chargeRate * Time.deltaTime;
                bullet.transform.localScale = new Vector3(currentScale, currentScale, currentScale);
                yield return new WaitForEndOfFrame();
            }

            //WE DESTROY THE GD BULLET WE JUST WENT THROUGH CREATING
            Object.Destroy(bullet);

            //WHY DOES THIS WORK
            bullet = Object.Instantiate(fireball, firePoint.transform.position, bullet.transform.rotation);
            bullet.transform.localScale = new Vector3(currentScale, currentScale, currentScale);

            //bullet.transform.localEulerAngles = Vector3.right;
            bullet.GetComponent<Bullet>().moveSpeed = bulletSpeed;
            bullet.GetComponent<Bullet>().canDamage = true;

            yield return new WaitForSeconds(timeBetweenAttacks);
            canAttack = true;
            isAttacking = false;
        }*/

    public override void TriggerAttack()
    {
        enemy.Anim.SetBool("isAttacking", true);
        charge.Attack(enemy.fireball, enemy.firePoint.transform, enemy.chargeRate);
    }

}

