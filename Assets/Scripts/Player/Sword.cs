using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    public bool dealsExtraDamage; //deals extra damage based on player's attack3 dmg mod if true

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<EnemyBase>())
        {
            //Determine dameage to deal based on player's current attack damage
            float damageToDeal = PlayerController.Instance.CurrAttackDamage; ;

            if (dealsExtraDamage)//Check to deal additional damage
                damageToDeal *= PlayerController.Instance.attack3DmgModifier;

            //Apply damage to enemy
            other.GetComponent<EnemyBase>().Damage(damageToDeal);     
        }
    }
}
