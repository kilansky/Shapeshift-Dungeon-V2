using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swipe : MonoBehaviour
{
    private Goblin enemy;

    public GameObject hitbox;
    //private MeshCollider hit;

    public Animator attackAnim;

    public bool showHitBoxes = false;

    private void Start()
    {
        //attackAnim = GetComponent<Animator>();
    }

    public void EnableHitBox()
    {
        //enemy.Anim.SetBool("isAttacking", true);
        //hit the player
        //turn on mesh collider hit box for melee attack
        hitbox.GetComponent<MeshCollider>().enabled = true;
        //show the hitbox for the attack
        if (showHitBoxes)
            hitbox.GetComponent<MeshRenderer>().enabled = true;
    }

    public void DisableHitBox()
    {
        //hit the player
        //turn on mesh collider hit box for melee attack
        hitbox.GetComponent<MeshCollider>().enabled = false;
        
        //show the hitbox for the attack
        if (showHitBoxes)
            hitbox.GetComponent<MeshRenderer>().enabled = false;
    }

    public void EndAttack()
    {
        Debug.Log("End Attack Called");
        attackAnim.SetBool("isAttacking", false);
    }

    /*private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>())
        {

            StartCoroutine(SlashBox());
            //Determine dameage to deal based on player's current attack damage
            float damageToDeal = PlayerController.Instance.CurrAttackDamage;
            //float damageDealt = Goblin.;

            //Apply damage to player
            //other.GetComponent<PlayerHealth>().Damage(damageDealt);
        }
    }*/

    /*IEnumerator SlashBox()
    {
        //enable the swipe hitbox
        hit = GetComponent<MeshCollider>();
        hit.enabled = true;
        yield return new WaitForSeconds(0.5f);
        //switch the meshCollider off
        hit.enabled = false;
    }*/

}
