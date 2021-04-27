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
    bool isChargingUp = false;

    private Transform monseterToHeal;

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
        //attack the player (if not a green crystal)
        if (!enemy.isGreenCrystal)
        {
            base.Enter();
            isChargingUp = true;
        }

        //Debug.Log("enemy is " + enemy.name);
        laser = enemy.GetComponent<LaserDispenser>();
        attackStartupTime = laser.startupTime;
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
        if (isAttacking || isChargingUp || (enemy.isGreenCrystal && !monseterToHeal))
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
        if (isAttacking && timeElapsed >= attackDuration && !enemy.isGreenCrystal)
        {
            isAttacking = false;
            //enemy.Anim.SetBool("isAttacking", false);
            enemy.pointLight.intensity = 5f;

            timeElapsed = 0;
            stateMachine.ChangeState(enemy.moveState);
        }

        //If this crystal is green, search for an enemy to heal
        if(!monseterToHeal && enemy.isGreenCrystal && timeElapsed >= 1f)
        {
            monseterToHeal = GetMonsterToHeal();

            if (monseterToHeal)
            {
                isChargingUp = true;
                base.Enter();
            }

            timeElapsed = 0;
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if(isAttacking && !enemy.isGreenCrystal)
            enemy.transform.Rotate(0, enemy.rotateSpeed, 0);
        else if(monseterToHeal && enemy.isGreenCrystal)//Have Green Crystal rotate towards enemy to heal
        {
            //Rotate Character in direction of mouse position
            Vector3 targetPoint = new Vector3(monseterToHeal.position.x, 0, monseterToHeal.position.z) - new Vector3(enemy.transform.position.x, 0, enemy.transform.position.z);
            //Debug.Log("targetPoint is: " + targetPoint);
            Quaternion targetRotation = Quaternion.LookRotation(targetPoint);
            //Debug.Log("targetRotation is: " + targetRotation);
            enemy.transform.rotation = targetRotation;

            //If the monster to heal reaches full health, set it to null and stop firing
            if(monseterToHeal.GetComponent<EnemyBase>().Health == monseterToHeal.GetComponent<EnemyBase>().healthBar.maxValue)
            {
                monseterToHeal = GetMonsterToHeal();

                if(monseterToHeal == null)
                {
                    isAttacking = false;
                    enemy.pointLight.intensity = 5f;
                    timeElapsed = 0;
                    stateMachine.ChangeState(enemy.moveState);
                }
            }
        }
    }

    public override void TriggerAttack()
    {
        //canAttack = false;
        //enemy.Anim.SetBool("isAttacking", true);

        //fire the laser
        enemy.GetComponent<LaserDispenser>().ToggleLaser(true);
    }

    //Returns the transform of a monster that can be healed
    private Transform GetMonsterToHeal()
    {
        //Get all monsters in the scene, store in 'monsters' array
        List<EnemyBase> monsters = new List<EnemyBase>(GameObject.FindObjectsOfType<EnemyBase>());
        monsters.Remove(enemy.GetComponent<EnemyBase>());

        //Search for a monster that is nearby and does not have full health
        foreach (EnemyBase monster in monsters)
        {
            float monsterDist = Vector3.Distance(enemy.transform.position, monster.transform.position);
            //Check if there is an enemy within range in need of healing
            if (monsterDist <= 15f && monster.Health < monster.healthBar.maxValue)
            {
                //Check if there is line of sight between the green crystal and the enemy to heal
                RaycastHit hit;
                Vector3 rayDirection = monster.transform.position - enemy.transform.position;
                if (Physics.Raycast(enemy.transform.position, rayDirection, out hit))
                {
                    if (hit.transform == monster.transform)
                        return monster.transform;
                }
            }
        }
        return null;
    }
}
