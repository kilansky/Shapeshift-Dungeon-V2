using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkullCharge : MonoBehaviour
{
    private EnemyBase enemy;
    [HideInInspector] public bool isAttacking = false;
    public float attackCoolDown = 2f;

    private List<float> startingSize = new List<float>();

    /*private void Awake()
    {
        ParticleSystem[] particles = gameObject.GetComponentInChildren<GameObject>();
        foreach (ParticleSystem particle in particles)
        {
            startingSize.Add(particle.startSize);
            ParticleSystemRenderer renderer = particle.GetComponent<ParticleSystemRenderer>();
            if (renderer)
            {
                startingSize.Add(renderer.lengthScale);
                startingSize.Add(renderer.velocityScale);
            }
        }
    }*/

    private void Start()
    {
        enemy = GetComponent<EnemyBase>();
    }

    public void Attack(GameObject fireball, Transform firePoint, float chargeRate)
    {
        StartCoroutine(ChargeAttack(fireball, firePoint, chargeRate));
    }
    IEnumerator ChargeAttack(GameObject fireball, Transform firePoint, float chargeRate)
    {
        //attack state specific to floating skull

        GameObject bullet = Instantiate(fireball, firePoint.transform.position, firePoint.transform.rotation, transform);
        //canAttack = false;
        isAttacking = true;

        //set movespeed to 0
        float bulletSpeed = bullet.GetComponent<Bullet>().moveSpeed;
        bullet.GetComponent<Bullet>().moveSpeed = 0f;

        //set scale of bullet to .1 scale up, doesn't scale vfx
        float currentScale = bullet.GetComponent<Bullet>().minBulletSize;
        
        bullet.transform.localScale = new Vector3(currentScale, currentScale, currentScale);

        //disable inner collider so it doesn't destroy while charging
        bullet.GetComponent<CapsuleCollider>().enabled = false;

        while (currentScale < bullet.GetComponent<Bullet>().maxBulletSize)
        {
            //if the enemy get's attacked while charging
            //destroy the bullet, and exit the loop
            if (enemy.isStunned)
            {
                Destroy(bullet);
                enemy.Anim.SetBool("isAttacking", false);
                yield break;
            }
            
            //scale bullet until it reaches max bullet size
            currentScale += chargeRate * Time.deltaTime;
            bullet.transform.localScale = new Vector3(currentScale, currentScale, currentScale);
            yield return new WaitForEndOfFrame();
        }

        //WE DESTROY THE GD BULLET WE JUST WENT THROUGH CREATING
        Destroy(bullet);

        //WHY DOES THIS WORK
        bullet = Instantiate(fireball, firePoint.transform.position, transform.rotation);
        bullet.transform.localScale = new Vector3(currentScale, currentScale, currentScale);
        bullet.transform.parent = this.gameObject.transform; //Sets the bullet to be the child of the skull to help with tracking - AHL (3/25/21)

        //bullet.transform.localEulerAngles = Vector3.right;
        bullet.GetComponent<Bullet>().moveSpeed = bulletSpeed;
        bullet.GetComponent<Bullet>().canDamage = true;

        yield return new WaitForSeconds(attackCoolDown);
        isAttacking = false;
    }

    public void AttackEnded()
    {
        enemy.Anim.SetBool("isAttacking", false);
    }

}
