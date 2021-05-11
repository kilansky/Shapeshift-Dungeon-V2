using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    public GameObject hitEffect;
    public bool dealsExtraDamage; //deals extra damage based on player's attack3 dmg mod if true

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<EnemyBase>() && !other.GetComponent<EnemyBase>().isInvincible)
        {
            //Debug.Log(other.gameObject);
            if (other.GetComponent<Skeleton>())
            {
                if (other.GetComponent<Skeleton>().isBlocking)
                    AudioManager.Instance.Play("Block");
                else if (!other.GetComponent<EnemyBase>().isInvincible)
                    AudioManager.Instance.Play("Hit");
            }
            else if (!other.GetComponent<EnemyBase>().isInvincible)
                AudioManager.Instance.Play("Hit");

            //Spawn hit effect on enemy
            Vector3 enemyPos = other.transform.position;
            Instantiate(hitEffect, new Vector3(enemyPos.x, transform.position.y, enemyPos.z), Quaternion.identity);

            //Apply slight camera shake
            CineShake.Instance.Shake(1f, 0.1f);

            //Determine dameage to deal based on player's current attack damage
            float damageToDeal = PlayerController.Instance.CurrAttackDamage;

            if (dealsExtraDamage)//Check to deal additional damage
                damageToDeal *= PlayerController.Instance.attack3DmgMod;

            //Apply damage to enemy
            other.GetComponent<EnemyBase>().Damage(damageToDeal);

            //Apply Knockback to enemy
            StartCoroutine(other.GetComponent<EnemyBase>().EnemyKnockBack());  
        }

        if (other.GetComponent<DestructibleProp>())
        {
            //Spawn hit effect
            //Vector3 objectPos = other.transform.position;
            //Instantiate(hitEffect, new Vector3(objectPos.x, transform.position.y, objectPos.z), Quaternion.identity);

            //Apply slight camera shake
            CineShake.Instance.Shake(1f, 0.1f);

            //Destroy Prop
            other.GetComponent<DestructibleProp>().ShatterObject();
        }

        if(other.GetComponent<ExplodingBarrel>())
        {
            other.GetComponent<ExplodingBarrel>().TriggerFuse();
        }
    }
}
