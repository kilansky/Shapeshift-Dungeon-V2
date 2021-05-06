using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class BombAttack : MonoBehaviour
{
    //Variable Initialization/Declaration
    public float damage; //Damage variable to adjust the damage anyone in the field would take
    public float destroyTime; //This is the time that it will take before the bomb gets destroyed
    public float explosionTime; //Variable to keep track of how long it takes to explode after the destroy time goes off
    public float explosionRadius; //The radius of the overall explosion/bomb
    public GameObject explosionEffect;
    private bool isDamage = false; //Bool to keep track of if the bomb went off and what would take damage

    //Called before the first frame to start the IEnumerator so the bomb goes off at the specified time
    private void Awake()
    {
        StartCoroutine(BombTimer());
    }

    /// <summary>
    /// OnTrigger so when the bool is true it checks to see what would take damage - AHL (3/9/21)
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        //Checks to see if the is damaging bool true (The bomb explodes)
        if(isDamage)
        {
            //If the other object is Monster (Contains the enemy base script) then go on with the rest of the damage
            if (other.GetComponent<EnemyBase>())
                other.GetComponent<EnemyBase>().Damage(damage);

            //Checks if the player is in the range
            if (other.GetComponent<PlayerHealth>())
            {
                if (!PlayerHealth.Instance.isInvincible)
                    AnalyticsEvents.Instance.PlayerDamaged("Bomb"); //Sends analytics event about damage source

                PlayerHealth.Instance.Damage(damage, gameObject);
            }

            if (other.GetComponent<DestructibleProp>())
            {
                other.GetComponent<DestructibleProp>().ShatterObject();
            }

            if(other.GetComponent<ExplodingBarrel>())
            {
                other.GetComponent<ExplodingBarrel>().TriggerFuse();
            }
        }
    }

    /// <summary>
    /// The IEnumerator to keep track when when the bomb will be destroyed and deal damage to the player - AHL (3/10/21)
    /// </summary>
    IEnumerator BombTimer()
    {
        float timer = 0f; //Sets a timer variable to keep track of the explosion hitbox growth
        Vector3 initialScale = transform.localScale; //Sets the initial size of the hitbox
        Vector3 finalScale = initialScale * 100f * explosionRadius; //Sets the final size of the hitbox

        yield return new WaitForSeconds(destroyTime); //Wait until it is time to destroy the bomb to grow the explosion radius (It go boom now)

        //Spawn explosion vfx
        Instantiate(explosionEffect, transform.position, Quaternion.identity);
        GetComponent<AudioSource>().Play();

        //Sets it to damage the player
        isDamage = true;

        //Shakes Camera
        CineShake.Instance.Shake(3f, explosionTime * 2);

        //While loop to lerp the scale of the explosion hitbox
        while (timer <= 1)
        {
            transform.localScale = Vector3.Lerp(initialScale, finalScale, timer);
            timer += Time.deltaTime / explosionTime;
            yield return null;
        }
        //Makes sure the hitbox scale is set to the final scale
        transform.localScale = finalScale;

        GetComponent<SphereCollider>().enabled = false;

        Destroy(transform.parent.gameObject, 2f); //Destroys the bomb parent object
    }
}
