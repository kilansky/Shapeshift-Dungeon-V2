using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swipe : MonoBehaviour
{
    public GameObject hitBox;
    private MeshCollider hit;

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
