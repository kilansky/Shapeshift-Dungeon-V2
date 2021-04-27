﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitBox : MonoBehaviour
{
    public GameObject hitEffect;
    public bool hasTorch = false;
    //public bool dealsExtraDamage; //deals extra damage based on player's attack3 dmg mod if true

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>())
        {
            //Spawn hit effect on player
            Vector3 playerPos = PlayerController.Instance.transform.position;
            Instantiate(hitEffect, new Vector3(playerPos.x, transform.position.y, playerPos.z), Quaternion.identity);

            //Apply slight camera shake
            CineShake.Instance.Shake(1f, 0.1f);

            //Determine dameage to deal based on player's current attack damage
            float damageToDeal = transform.parent.GetComponent<EnemyBase>().meleeDamage;

            //Set player on fire if this enemy is holding a torch
            if (hasTorch)
            {
                damageToDeal--;
                other.GetComponent<StatusEffects>().fireStatus(4);
            }
            //Apply damage to player
            other.GetComponent<PlayerHealth>().Damage(damageToDeal);

            //if (dealsExtraDamage)//Check to deal additional damage
            //  damageToDeal *= PlayerController.Instance.attack3DmgMod;

            
            //Apply Knockback to enemy
            //StartCoroutine(other.GetComponent<EnemyBase>().EnemyKnockBack());

            AudioManager.Instance.Play("Hit");
        }

        if (other.GetComponent<DestructibleProp>())
        {
            //Spawn hit effect
            //Vector3 objectPos = other.transform.position;
            //Instantiate(hitEffect, new Vector3(objectPos.x, transform.position.y, objectPos.z), Quaternion.identity);

            //Apply slight camera shake
            CineShake.Instance.Shake(1f, 0.1f);

            //Destroy Prop
            other.GetComponent<DestructibleProp>().DestroyObject();
            AudioManager.Instance.Play("WoodBreak");
        }

        if (other.GetComponent<ExplodingBarrel>())
        {
            other.GetComponent<ExplodingBarrel>().TriggerFuse();
        }
    }
}
