using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]

public class Worm : EnemyBase
{
    //this script contains all the states of the Worm
    public Worm_IdleState idleState { get; private set; }
    public Worm_MoveState moveState { get; private set; }
    public Worm_PlayerDetected playerDetectedState { get; private set; }
    public Worm_AttackState attackState { get; private set; }
    public Worm_LookForPlayer lookForPlayerState { get; private set; }
    public Worm_StunState stunState { get; private set; }

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
    [HideInInspector] public bool isAttacking = false;
    [HideInInspector] public float aboveGroundYPos;
    [HideInInspector] public float underGroundYPos;
    [HideInInspector] public float dirtAboveGroundYPos;
    [HideInInspector] public float dirtUnderGroundYPos;
    [HideInInspector] public bool wormIsMoving;
    [HideInInspector] public Tile currOccupiedTile;

    [Header("Worm Moving")]
    public GameObject wormMover;
    public float minMoveRangeFromPlayer = 4.5f;
    public float maxMoveRangeFromPlayer = 21f;
    public LayerMask tileLayer;

    [Header("Submerging")]
    public float timeToEmergeOrSubmerge = 1f;
    public float amountToMoveWorm = 6f;

    [Header("Dirt Circle")]
    public GameObject dirtCircle;
    public Material dirtMat;
    public Material sandMat;
    public ParticleSystem dirtCloud;
    public ParticleSystem dirtChunks;
    public float amountToMoveDirt = 2f;

    [Header("Renderers")]
    public SkinnedMeshRenderer headRenderer;
    public SkinnedMeshRenderer bodyRenderer;
    public GameObject healthCanvas;

    private float attackDamage;

    public override void Start()
    {
        SetNewTarget(player);
        base.Start();

        moveState = new Worm_MoveState(this, stateMachine, "move", moveStateData, this);
        idleState = new Worm_IdleState(this, stateMachine, "idle", idleStateData, this);
        playerDetectedState = new Worm_PlayerDetected(this, stateMachine, "playerDetected", playerDetectedData, this);
        attackState = new Worm_AttackState(this, stateMachine, "attack", attackStateData, this);
        lookForPlayerState = new Worm_LookForPlayer(this, stateMachine, "lookForPlayer", lookForPlayerStateData, this);
        stunState = new Worm_StunState(this, stateMachine, "stun", stunStateData, this);

        //Set starting positions for the worm model & dirt circle
        aboveGroundYPos = wormMover.transform.position.y;
        underGroundYPos = aboveGroundYPos - amountToMoveWorm;
        wormMover.transform.position = new Vector3(wormMover.transform.position.x, underGroundYPos, wormMover.transform.position.z);

        dirtAboveGroundYPos = dirtCircle.transform.position.y;
        dirtUnderGroundYPos = aboveGroundYPos - amountToMoveDirt;
        dirtCircle.transform.position = new Vector3(dirtCircle.transform.position.x, dirtUnderGroundYPos, dirtCircle.transform.position.z);

        //Set the starting tile to be occupied by the worm
        RaycastHit hit;
        if(Physics.Raycast(transform.position + new Vector3(0, 10, 0), Vector3.down, out hit, 50, tileLayer))
        {
            currOccupiedTile = hit.transform.GetComponent<Tile>();
            currOccupiedTile.occupiedByWorm = true;
        }

        //initialize the worm in the idle state
        stateMachine.Initialize(idleState);
    }

    public override void Update()
    {
        base.Update();

        //Get direction to player
        Vector3 targetPoint = new Vector3(player.transform.position.x, 0, player.transform.position.z) - new Vector3(wormMover.transform.position.x, 0, wormMover.transform.position.z);
        Quaternion targetRotation = Quaternion.LookRotation(targetPoint);
        float rotSpeed = 5f;

        //Smoothly rotate towards player
        Quaternion lastTargetRotation = Quaternion.Slerp(wormMover.transform.rotation, targetRotation, rotSpeed * Time.deltaTime);
        wormMover.transform.rotation = lastTargetRotation;
    }

    //set current state to stunState if isStunned
    public override void Damage(float damage)
    {
        base.Damage(damage);
        Flash();

        //Make sure worm is not moving before transitioning to stun state
        if (isStunned && stateMachine.currentState != stunState && !wormIsMoving)
        {
            stateMachine.ChangeState(stunState);
        }
    }

    public override void FireDamage(float damage)
    {
        base.FireDamage(damage);

        //Worm goes to the move state if it can to try and quench its flames
        if (!wormIsMoving && !isAttacking)
        {
            stateMachine.ChangeState(moveState);
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
        headRenderer.material = hitMat;
        bodyRenderer.material = hitMat;
        StartCoroutine(WaitToResetColor());
    }

    public override void ResetColor()
    {
        base.ResetColor();
        headRenderer.material = normalMat;
        bodyRenderer.material = normalMat;
    }

    public Transform FindSafeTeleportTile()
    {
        List<Tile> tiles = new List<Tile>();
        foreach (Tile tile in GameObject.FindObjectsOfType<Tile>())
        {
            //Check if the tile is safe to stand on
            if (tile.tileType == Tile.tileTypes.dirt || tile.tileType == Tile.tileTypes.sand)
            {
                //Get a tile that is not occupied by another worm and is within a certain range of the player
                float playerDistToTile = Vector3.Distance(player.transform.position, tile.transform.position + new Vector3(0,5,0));
                if (!tile.GetComponent<Tile>().occupiedByWorm && playerDistToTile > minMoveRangeFromPlayer && playerDistToTile < maxMoveRangeFromPlayer)
                    tiles.Add(tile); //Add this tile to the list of potential safe tiles to teleport to
            }
        }

        if (tiles.Count != 0)
        {
            //Debug.Log("Worm found " + tiles.Count + " safe tiles to teleport to");
            int randTileIndex = Random.Range(0, tiles.Count);
            return tiles[randTileIndex].transform;
        }

        return null;
    }

    public override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, minMoveRangeFromPlayer);
        Gizmos.DrawWireSphere(transform.position, maxMoveRangeFromPlayer);
    }
}
