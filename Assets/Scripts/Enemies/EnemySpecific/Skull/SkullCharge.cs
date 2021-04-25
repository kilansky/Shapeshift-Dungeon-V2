using System.Collections;
using System.Collections.Generic;
using UnityEngine.VFX;
using UnityEngine;

public class SkullCharge : MonoBehaviour
{
    private EnemyBase enemy;
    private Animator myAnim;
    public GameObject sparkGO;
    
    [HideInInspector] public bool isAttacking = false;
    public float attackCoolDown = 2f;

    private List<float> startingSize = new List<float>();
    private float chargeTime = 1f;

    private VisualEffect sparks;

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
        enemy = transform.parent.GetComponent<EnemyBase>();
        myAnim = GetComponent<Animator>();
        sparks = sparkGO.GetComponent<VisualEffect>();
    }

    public void Attack(GameObject fireball, Transform firePoint, float chargeRate)
    {
        StartCoroutine(ChargeAttack(fireball, firePoint, chargeRate));
    }
    IEnumerator ChargeAttack(GameObject fireball, Transform firePoint, float chargeRate)
    {
        //attack state specific to floating skull

        //4-22-21
        //I replaced a lot of this function with code from the dispenser script to make vfx scaling work
        //-Justin

        GameObject bullet = Instantiate(fireball, firePoint.transform.position, firePoint.transform.rotation, transform);
        Vector3 originalScale = bullet.transform.localScale;
        float vfxPercent = 0;
        //canAttack = false;
        isAttacking = true;

        //set movespeed to 0
        bullet.GetComponent<Bullet>().moveSpeed = 0f;

        //To be honest, I forgot why I needed to do this. But it works for the dispenser
        bullet.transform.GetChild(0).gameObject.SetActive(false);

        yield return new WaitForSeconds(.1f);

        //set scale of bullet to .1 scale up, doesn't scale vfx
        bullet.transform.GetChild(0).gameObject.SetActive(true);
        bullet.transform.localScale = Vector3.one * .1f;

        //disable inner collider so it doesn't destroy while charging
        bullet.GetComponent<CapsuleCollider>().enabled = false;

        float counter = 0f; //Counter to keep track of time elapsed
        while (counter < chargeTime) //This while loop scales object over time
        {
            if (enemy.isStunned)
            {
                Destroy(bullet);
                myAnim.SetBool("isAttacking", false);
                yield break;
            }

            counter += Time.deltaTime;
            bullet.transform.localScale = Vector3.Lerp(bullet.transform.localScale, originalScale, counter / chargeTime);
            vfxPercent = Mathf.Lerp(1f, 100f, counter / chargeTime);
            //Debug.Log(vfxPercent);
            bullet.GetComponent<Bullet>().SetVFXScale(vfxPercent);
            yield return null;
        }
        /*
        while (currentScale < 1f)
        {
            //if the enemy get's attacked while charging
            //destroy the bullet, and exit the loop
            if (enemy.isStunned)
            {
                Destroy(bullet);
                myAnim.SetBool("isAttacking", false);
                yield break;
            }
            
            //scale bullet until it reaches max bullet size
            currentScale += chargeRate * Time.deltaTime;
            //bullet.transform.localScale = new Vector3(currentScale, currentScale, currentScale);
            bullet.GetComponent<Bullet>().SetVFXScale(currentScale * 100f);
            yield return new WaitForEndOfFrame();
        }
        */

        //WE DESTROY THE GD BULLET WE JUST WENT THROUGH CREATING
        Destroy(bullet);

        //WHY DOES THIS WORK
        bullet = Instantiate(fireball, firePoint.transform.position, transform.rotation);
        bullet.transform.localScale = originalScale;

        //Sets the bullets parent game object to this one to make the Damage Tracker acquire the skull as the correct game object for tracking
        bullet.GetComponent<Bullet>().parentObject = transform.root.gameObject;

        //bullet.transform.localEulerAngles = Vector3.right;
        bullet.GetComponent<Bullet>().canDamage = true;
        bullet.GetComponent<Bullet>().shotBy = "Skull";
        sparks.Play();

        yield return new WaitForSeconds(attackCoolDown);
        isAttacking = false;
    }

    public void AttackEnded()
    {
        myAnim.SetBool("isAttacking", false);
    }
}
