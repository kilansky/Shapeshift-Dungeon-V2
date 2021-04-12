using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    public GameObject hitEffect;
    public bool dealsExtraDamage; //deals extra damage based on player's attack3 dmg mod if true

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<EnemyBase>())
        {
            //Spawn hit effect on enemy
            Vector3 enemyPos = other.transform.position;
            Instantiate(hitEffect, new Vector3(enemyPos.x, transform.position.y, enemyPos.z), Quaternion.identity);

            //Determine dameage to deal based on player's current attack damage
            float damageToDeal = PlayerController.Instance.CurrAttackDamage;

            if (dealsExtraDamage)//Check to deal additional damage
                damageToDeal *= PlayerController.Instance.attack3DmgModifier;

            //Apply damage to enemy
            other.GetComponent<EnemyBase>().Damage(damageToDeal);

        }
    }
}
