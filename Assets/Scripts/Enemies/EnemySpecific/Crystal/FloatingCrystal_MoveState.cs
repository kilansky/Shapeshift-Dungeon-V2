using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FloatingCrystal_MoveState : MoveState
{
    //movement for the floating crystal
    private FloatingCrystal enemy;
    private LaserDispenser laser;
    //target will be other crystals in the scene
    public Transform target;

    public float minAttackDelay = 3f;
    public float maxAttackDelay = 6f;
    public float timeToAttack;
    float timeElapsed = 0;
    [HideInInspector] public bool isAttacking = false;

    public Vector3 walkPoint;

    public bool walkPointSet;
    public float walkPointRange = 20f;

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
        isAttacking = false;
        entity.SetVelocity(stateData.moveSpeed);
        laser = enemy.GetComponent<LaserDispenser>();
        enemy.GetComponent<LaserDispenser>().ToggleLaser(false);
        timeToAttack = Random.Range(minAttackDelay, maxAttackDelay);
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
        //for now attack every 5 - 8 seconds
        /*if(!isAttacking)
            timeElapsed += Time.deltaTime;*/

        //if enough time has passed, or the player is in agro range change to attack
        if (!isAttacking && entity.distanceToPlayer < entity.minAgroRange)
        {
            isAttacking = true;
            stateMachine.ChangeState(enemy.attackState);
            timeElapsed = 0;
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        Patrol();
    }

    public void Patrol()
    {
        //walkPoint = Random.insideUnitSphere * walkPointRange;
        /*walkPoint += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(walkPoint, out hit, walkPointRange, 1);
        Vector3 finalPosition = hit.position;*/

        //transform.Rotate(0, rotateSpeed, 0);

        //move to a walk point set in the scene
        if (!walkPointSet)
            SearchWalkPoint();

        if (walkPointSet)
        {
            //set the destination to the walkpoint from searchwalkpoint
            enemy.agent.SetDestination(walkPoint);
            if (!enemy.agent.pathPending)
            {
                //if you've reached your destination do something
                if (enemy.agent.remainingDistance <= enemy.agent.stoppingDistance)
                {
                    walkPointSet = false;
                    //if the agent doesn't have a path to a point, or they are stopped
                    if (!enemy.agent.hasPath || enemy.agent.velocity.sqrMagnitude == 0f)
                    {
                        walkPointSet = false;
                    }
                }
            }
        }
        /*Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if (distanceToWalkPoint.magnitude <= 1f)
            walkPointSet = false;*/
    }

    public void SearchWalkPoint()
    {
        //find a walk point after you've moved to one already
        walkPoint = new Vector3(Random.insideUnitSphere.x * walkPointRange, enemy.transform.position.y, Random.insideUnitSphere.z * walkPointRange);
        Debug.Log("my walkpoint is " + walkPoint);
        walkPointSet = true;

        /*float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        //walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
        //walkPointSet = true;
        Debug.Log("my walk point is " + walkPoint);*/

        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(walkPoint, out hit, walkPointRange, 1))
        {

            finalPosition = hit.position;
            walkPoint = finalPosition;

            //Debug.Log("walkpointset is " + walkPointSet);
            //Debug.Log("i'm going to " + finalPosition);

        }

        //may need a parameter for what is walkable ground or not
        //walkPointSet = true;

        /*if (Physics.Raycast(walkPoint, -transform.up, 2f))
        {
            walkPointSet = true;
            Debug.Log("walkPoint set SHOULD be true, and it is " + walkPointSet);
        }*/
    }
}
