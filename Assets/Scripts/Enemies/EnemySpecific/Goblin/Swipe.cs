using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swipe : MonoBehaviour
{
    private Goblin enemy;

    public GameObject hitBox;
    private MeshCollider hit;

    public bool showHitBoxes = false;


    public void MeleeAttack(GameObject hitBox, Transform hitPoint)
    {
        enemy.Anim.SetBool("isAttacking", true);
        //hit the player
        //turn on mesh collider hit box for melee attack
        hitBox.GetComponent<MeshCollider>().enabled = true;
        //show the hitbox for the attack
        if (showHitBoxes)
            hitBox.GetComponent<MeshRenderer>().enabled = true;
    }

    private void OnTriggerEnter(Collider other)
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
    }

    IEnumerator SlashBox()
    {
        //enable the swipe hitbox
        hit = GetComponent<MeshCollider>();
        hit.enabled = true;
        yield return new WaitForSeconds(0.5f);
        //switch the meshCollider off
        hit.enabled = false;
    }

}
