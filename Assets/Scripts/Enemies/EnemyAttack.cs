using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [Header("Attack Hitbox")]
    public GameObject hitbox;
    public bool showHitBoxes = false;

    private Goblin goblinEnemy;
    private Skeleton skeletonEnemy;
    private Slime slimeEnemy;
    private Worm wormEnemy;
    private Animator attackAnim;

    private void Start()
    {
        if (transform.root.GetComponent<Goblin>())
            goblinEnemy = transform.root.GetComponent<Goblin>();

        if (transform.root.GetComponent<Skeleton>())
            skeletonEnemy = transform.root.GetComponent<Skeleton>();

        if (transform.root.GetComponent<Slime>())
            slimeEnemy = transform.root.GetComponent<Slime>();

        if (transform.root.GetComponent<Worm>())
            wormEnemy = transform.root.GetComponent<Worm>();

        attackAnim = GetComponent<Animator>();
    }

    public void EnableHitBox()
    {
        //enemy.Anim.SetBool("isAttacking", true);
        //hit the player
        //turn on mesh collider hit box for melee attack
        hitbox.GetComponent<Collider>().enabled = true;
        //show the hitbox for the attack
        if (showHitBoxes)
            hitbox.GetComponent<MeshRenderer>().enabled = true;
    }

    public void DisableHitBox()
    {
        //hit the player
        //turn on mesh collider hit box for melee attack
        hitbox.GetComponent<Collider>().enabled = false;
        
        //show the hitbox for the attack
        if (showHitBoxes)
            hitbox.GetComponent<MeshRenderer>().enabled = false;
    }

    public void EndAttack()
    {
        attackAnim.SetBool("isAttacking", false);
        StartCoroutine(WaitToEnableAttack());
    }

    IEnumerator WaitToEnableAttack()
    {   
        yield return new WaitForSeconds(0.5f);
        
        if(goblinEnemy)
            goblinEnemy.isAttacking = false;

        if (skeletonEnemy)
            skeletonEnemy.isAttacking = false;

        if (slimeEnemy)
            slimeEnemy.isAttacking = false;

        if (wormEnemy)
            wormEnemy.isAttacking = false;
    }

}
