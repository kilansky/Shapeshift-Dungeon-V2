using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    //script for bullet interactions/stats
    //should be able to create custom projectile
    //prefabs from this one script 

    [Range(0f, 1f)]
    public bool canDamage = false;
    public float moveSpeed = 20f;

    //explosion variables
    public float bulletDamage = 5f;

    //bullet charge time
    public float minBulletSize;
    public float maxBulletSize;

    private Vector3 shootDir;

    private void Start()
    {
        Setup();
        Destroy(gameObject, 10f); //destroy this projectile if in the scene for 10 seconds
    }

    private void Update()
    {
        transform.position += shootDir * moveSpeed * Time.deltaTime;
    }

    //Check if player was hit & deal damage
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player") && canDamage)
        {
            if(!PlayerHealth.Instance.isInvincible)
            {
                PlayerHealth.Instance.Damage(bulletDamage);
                Destroy(gameObject);
            }
        }       
    }

    private void OnCollisionEnter(Collision collision)
    {
        //destroy the bullet if it hits the environment, the player, or stairs
        if(collision.gameObject.layer == 10 || collision.gameObject.layer == 8 || collision.gameObject.layer == 2)
        {
            Destroy(gameObject);
        }       
    }

    private void Setup()
    {
        shootDir = transform.forward;

        //implements bullet facing direction
        transform.eulerAngles = new Vector3(0, 0, GetAngleFromVectorFloat(shootDir));
    }

    //need this to change bullet facing direction
    public static float GetAngleFromVectorFloat(Vector3 dir)
    {
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if(n < 0)
        {
            n += 360;
        }
        return n;
    }
}
