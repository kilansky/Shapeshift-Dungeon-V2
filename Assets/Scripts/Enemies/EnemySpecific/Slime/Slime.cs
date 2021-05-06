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

    public static int staticSlimeID = 0;
    [HideInInspector] public int slimeID = 0;
    [HideInInspector] public bool isBaseSlime = true;

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

        //If this is the initial slime that spawned, set its ID number
        //This is done to only kill 1 slime for the monster spawn system rather than get multiple kills per slime
        if(isBaseSlime)
        {
            slimeID = staticSlimeID;
            staticSlimeID++;
        }
        Debug.Log("slimeID is: " + slimeID);

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

        yield return new WaitForSeconds(1f);

        switch(slimeType)
        {
            case SlimeType.big:
                SlimeSplit(mediumSlime);
                break;
            case SlimeType.medium:
                SlimeSplit(smallSlime);
                break;
            case SlimeType.small:
                SlimeKill();
                break;
            default:
                break;
        }
    }

    private void SlimeSplit(GameObject newSlimeType)
    {
        GameObject newSlime = Instantiate(newSlimeType, transform.position, transform.rotation);
        newSlime.GetComponent<Slime>().isBaseSlime = false;
        newSlime.GetComponent<Slime>().slimeID = slimeID;

        if (GetComponent<GemMonster>().isGemMonster)//Pass the gem from this slime onto one of its splits
            newSlime.GetComponent<GemMonster>().SetGemMonster();

        newSlime = Instantiate(newSlimeType, transform.position, transform.rotation);
        newSlime.GetComponent<Slime>().isBaseSlime = false;
        newSlime.GetComponent<Slime>().slimeID = slimeID;

        SlimeKill();
    }

    private void SlimeKill()
    {
        if(slimeType == SlimeType.small)
        {
            //Check whether this is the last slime of its ID value
            bool isLastSlime = true;
            foreach (Slime slime in GameObject.FindObjectsOfType<Slime>())
            {
                if (slime != this && slime.slimeID == slimeID)
                    isLastSlime = false;
            }
            Debug.Log("isLastSlime is: " + isLastSlime);
            Debug.Log("slimeID is: " + slimeID);

            //Update the monster count of the room IF this is the last small slime of its ID
            if (isLastSlime)
                MonsterSpawner.Instance.MonsterKilled();

            if (GetComponent<GemMonster>().isGemMonster)
                DropGem();
        }

        Instantiate(deathEffect, transform.position + new Vector3(0, agent.height / 2, 0), Quaternion.identity);

        //Destroy self from root object
        Destroy(transform.root.gameObject);

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
