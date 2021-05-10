using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SlimeMove : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;
    private float baseSpeed;

    private void Start()
    {
        agent = transform.parent.GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        baseSpeed = agent.speed;
    }

    //Called from animation event
    public void CrawlMovement()
    {
        agent.speed = baseSpeed;
    }

    //Called from animation event
    public void AttackMovement()
    {
        agent.speed = baseSpeed * 1.35f;
    }

    //Called from animation event
    public void StopMovement()
    {
        agent.speed = 0;
    }
}
