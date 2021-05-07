using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum SlimeType { small, medium, big}

[RequireComponent(typeof(NavMeshAgent))]

public class Slime : EnemyBase
{
    //this script contains all the states of the Slime
    public Slime_IdleState idleState { get; private set; }
    public Slime_MoveState moveState { get; private set; }
    public Slime_PlayerDetected playerDetectedState { get; private set; }
    public Slime_AttackState attackState { get; private set; }
    public Slime_LookForPlayer lookForPlayerState { get; private set; }
    public Slime_StunState stunState { get; private set; }

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

    public GameObject mediumSlime;
    public GameObject smallSlime;
    public SlimeType slimeType;
    public GameObject canvas;

    public SkinnedMeshRenderer renderer;

    [HideInInspector] public bool isAttacking = false;
    private float attackDamage;

    public override void Start()
    {
        SetNewTarget(player);
        base.Start();

        moveState = new Slime_MoveState(this, stateMachine, "move", moveStateData, this);
        idleState = new Slime_IdleState(this, stateMachine, "idle", idleStateData, this);
        playerDetectedState = new Slime_PlayerDetected(this, stateMachine, "playerDetected", playerDetectedData, this);
        attackState = new Slime_AttackState(this, stateMachine, "attack", attackStateData, this);
        lookForPlayerState = new Slime_LookForPlayer(this, stateMachine, "lookForPlayer", lookForPlayerStateData, this);
        stunState = new Slime_StunState(this, stateMachine, "stun", stunStateData, this);

        //initialize the slime in the idle state
        stateMachine.Initialize(moveState);
    }

    //set current state to stunState if isStunned
    public override void Damage(float damage)
    {
        base.Damage(damage);
        Flash();

        if (isStunned && stateMachine.currentState != stunState)
        {
            stateMachine.ChangeState(stunState);
        }
    }

    public override void Kill()
    {
        int randomSound = Random.Range(0, 2);
        switch (randomSound)
        {
            case 0:
                AudioManager.Instance.Play("EnemyDeath1");
                break;
            case 1:
                AudioManager.Instance.Play("EnemyDeath2");
                break;
            default:
                Debug.LogError("I broke the switch statement");
                break;
        }
        canvas.SetActive(false);
        isInvincible = true;

        StartCoroutine(SlimeDeathAnim());
    }

    private IEnumerator SlimeDeathAnim()
    {
        Anim.SetBool("isDead", true);

        yield return new WaitForSeconds(0.75f);
        Instantiate(deathEffect, transform.position + new Vector3(0, agent.height / 2, 0), Quaternion.identity);

        //Called the killed function of the slime group based on this slime's type
        switch (slimeType)
        {
            case SlimeType.big:
                transform.root.GetComponent<SlimeGroup>().BigSlimeKilled(this);
                break;
            case SlimeType.medium:
                transform.root.GetComponent<SlimeGroup>().MediumSlimeKilled(this);
                break;
            case SlimeType.small:
                transform.root.GetComponent<SlimeGroup>().SmallSlimeKilled(this);
                break;
            default:
                break;
        }
    }

    public override void SetNewTarget(GameObject newTarget)
    {
        //this will be used for the dummy item
        target = newTarget.transform;
    }

    public override void Flash()
    {
        //sets enemy's color to the hitMat (red)
        renderer.material = hitMat;
        StartCoroutine(WaitToResetColor());

    }

    public override void ResetColor()
    {
        base.ResetColor();
        renderer.material = normalMat;
    }
}
