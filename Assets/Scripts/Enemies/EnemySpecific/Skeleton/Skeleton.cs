using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]

public class Skeleton : EnemyBase
{
    //this script contains all the states of the skeleton
    //skeleton will need to block player attacks
    public Skeleton_IdleState idleState { get; private set; }
    public Skeleton_MoveState moveState { get; private set; }
    public Skeleton_PlayerDetected playerDetectedState { get; private set; }
    public Skeleton_AttackState attackState { get; private set; }
    public Skeleton_LookForPlayer lookForPlayerState { get; private set; }
    public Skeleton_StunState stunState { get; private set; }

    [SerializeField]
    private D_IdleState idleStateData;
    [SerializeField]
    private D_MoveState moveStateData;
    [SerializeField]
    private D_PlayerDetected playerDetectedData;
    [SerializeField]
    private D_AttackState attackStateData;
    [SerializeField]
    private D_LookForPlayer lookForPlayerStateData;
    [SerializeField]
    private D_StunState stunStateData;

    //will have a melee range instead of fireball
    public float AttackDamage { get { return attackDamage; } }

    public GameObject ragdoll;

    public LayerMask tileLayer;

    [HideInInspector] public GameObject FrontTarget;
    [HideInInspector] public GameObject SideTarget;
    [HideInInspector] public GameObject BackTarget;
    [HideInInspector] public bool isAttacking = false;
    [HideInInspector] public bool isBlocking = false;

    //shield that blocks all incoming damage from front of skeleton
    //public GameObject shield;

    //public GameObject meleeHitBox;
    //public Transform hitPoint;
    #region Skinned Mesh Renderers
    public SkinnedMeshRenderer feetRenderer;
    public SkinnedMeshRenderer femurRenderer;
    public SkinnedMeshRenderer forearmRenderer;
    public SkinnedMeshRenderer gutBasketRenderer;
    public SkinnedMeshRenderer handRenderer;
    public SkinnedMeshRenderer lowerLegRenderer;
    public SkinnedMeshRenderer miniRibRenderer;
    public SkinnedMeshRenderer ribCageRenderer;
    public SkinnedMeshRenderer shoulderRenderer;
    public SkinnedMeshRenderer skullRenderer;
    public SkinnedMeshRenderer spineRenderer;
    public SkinnedMeshRenderer upperArmRenderer;
    #endregion

    private float attackDamage;


    public override void Start()
    {
        SetNewTarget(player);
        base.Start();

        moveState = new Skeleton_MoveState(this, stateMachine, "move", moveStateData, this);
        idleState = new Skeleton_IdleState(this, stateMachine, "idle", idleStateData, this);
        playerDetectedState = new Skeleton_PlayerDetected(this, stateMachine, "playerDetected", playerDetectedData, this);
        attackState = new Skeleton_AttackState(this, stateMachine, "attack", attackStateData, this);
        lookForPlayerState = new Skeleton_LookForPlayer(this, stateMachine, "lookForPlayer", lookForPlayerStateData, this);
        stunState = new Skeleton_StunState(this, stateMachine, "stun", stunStateData, this);

        //initialize the goblin in the idle state
        //stateMachine.Initialize(idleState);

        //this line is what got rid of my NullReferenceExceptions
        //stateMachine.Initialize(moveState);
        stateMachine.Initialize(idleState);

        //renderer = GetComponentInChildren<Skeleton>()

        FrontTarget = PlayerController.Instance.frontTarget;
        SideTarget = PlayerController.Instance.sideTarget;
        BackTarget = PlayerController.Instance.backTarget;

        //shield = GetComponent<GameObject>();
    }
        

    //set current state to stunState if isStunned
    public override void Damage(float damage)
    {
        if (!isBlocking)
        {
            base.Damage(damage);
            Flash();
            /*if (isDead)
            {
                stateMachine.ChangeState(deadState);
            }*/
            if (isStunned && stateMachine.currentState != stunState && !isBlocking)
            {
                stateMachine.ChangeState(stunState);
            }
        }
    }

    public override void Kill()
    {
        base.Kill();

        //ragdoll.transform.parent = null;
        ragdoll.gameObject.SetActive(true);

        //Set the starting tile to be occupied by the worm
        RaycastHit hit;
        if (Physics.Raycast(transform.position + new Vector3(0, 10, 0), Vector3.down, out hit, 20, tileLayer))
        {
            //set ragdoll's parent to tile it is on
            ragdoll.transform.parent = hit.transform;
        }
    }

    public override void SetNewTarget(GameObject newTarget)
    {
        //this will be used for the dummy item
        target = newTarget.transform;
    }

    public bool HaveLineOfSight()
    {
        RaycastHit sightHit;

        //vector3 drawn from the goblin to the player
        Vector3 direction = player.transform.position - transform.position;
        //Debug.DrawRay(transform.position, direction, Color.red);
        if (Physics.Raycast(transform.position, direction, out sightHit/*, may need distance var here*/))
        {
            //if we see our target gameObject, draw the line
            //if(sightHit.transform.gameObject.name.Equals.BackTarget)
            //Debug.DrawRay(transform.position, direction, Color.red);
        }

        return false;
    }

    /*public void Block()
    {
        if (isBlocking)
        {
            shield.GetComponent<BoxCollider>().enabled = true;
        }
    }*/

    public override void Flash()
    {
        //sets enemy's color to the hitMat (red)
        feetRenderer.material = hitMat;
        femurRenderer.material = hitMat;
        forearmRenderer.material = hitMat;
        gutBasketRenderer.material = hitMat;
        handRenderer.material = hitMat;
        lowerLegRenderer.material = hitMat;
        miniRibRenderer.material = hitMat;
        ribCageRenderer.material = hitMat;
        shoulderRenderer.material = hitMat;
        skullRenderer.material = hitMat;
        spineRenderer.material = hitMat;
        upperArmRenderer.material = hitMat;
        StartCoroutine(WaitToResetColor());
    }

    public override void ResetColor()
    {
        base.ResetColor();
        feetRenderer.material = normalMat;
        femurRenderer.material = normalMat;
        forearmRenderer.material = normalMat;
        gutBasketRenderer.material = normalMat;
        handRenderer.material = normalMat;
        lowerLegRenderer.material = normalMat;
        miniRibRenderer.material = normalMat;
        ribCageRenderer.material = normalMat;
        shoulderRenderer.material = normalMat;
        skullRenderer.material = normalMat;
        spineRenderer.material = normalMat;
        upperArmRenderer.material = normalMat;
    }
}
