using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikedBoots : MonoBehaviour
{
    public float damage; //Variable to keep track of how much damage the dash hitbox deals to enemies

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<EnemyBase>())
        {
            //Apply slight camera shake
            CineShake.Instance.Shake(1f, 0.1f);

            //Apply damage to enemy
            other.GetComponent<EnemyBase>().Damage(damage);

            //Apply Knockback to enemy
            StartCoroutine(other.GetComponent<EnemyBase>().EnemyKnockBack());

            AudioManager.Instance.Play("Hit");
        }

        /*if (other.GetComponent<MageBoss>())
        {
            //Apply slight camera shake
            CineShake.Instance.Shake(1f, 0.1f);

            //Apply damage to enemy
            other.GetComponent<MageBoss>().Damage(damage);

            AudioManager.Instance.Play("Hit");
        }*/

        if (other.GetComponent<DestructibleProp>())
        {
            //Apply slight camera shake
            CineShake.Instance.Shake(1f, 0.1f);

            //Destroy Prop
            other.GetComponent<DestructibleProp>().ShatterObject();
            AudioManager.Instance.Play("WoodBreak");
        }

        if (other.GetComponent<ExplodingBarrel>())
        {
            other.GetComponent<ExplodingBarrel>().TriggerFuse();
        }
    }
}
