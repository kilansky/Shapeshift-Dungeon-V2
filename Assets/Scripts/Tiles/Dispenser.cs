using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dispenser : MonoBehaviour
{
    /**
     * Script: Dispenser
     * Programmer: Justin Donato
     * Description: Behavior of the projectile dispenser tile
     * Date Created: 3/9/2021
     * Date Last Edited: 3/9/2021
     **/

    [Header("Dispenser Variables")]
    public GameObject projectile;
    
    [Header("Attacking Variables")]
    public float rateOfFire = 3f;
    public float chargeTime = 1f;
    public float delayBeforeFiring = 0.5f;

    [HideInInspector] public bool canFire = false;

    private Transform spawner;

    private void Start()
    {
        spawner = transform.GetChild(0);
        if(chargeTime + delayBeforeFiring > rateOfFire)
        {
            Debug.LogError("Charge time is greater than the rate of fire!");
            return;
        }
        //StartCoroutine(AttackCycle());
    }

    public void BeginFiring()
    {
        StartCoroutine(AttackCycle());
    }

    private IEnumerator AttackCycle()
    {
        /*
        if(!canFire)
        {
            //StartCoroutine(AttackCycle());
            yield break;
        }
        */

        //Spawns in projectile and sets its scale to 0 and slowly begins to scale up to proper size over chargeTime seconds
        GameObject bullet = Instantiate(projectile, spawner.position, transform.rotation, transform);
        Vector3 originalScale = bullet.transform.localScale;
        float originalSpeed = bullet.GetComponent<Bullet>().moveSpeed;
        float vfxPercent = 0;
       
        //bullet.GetComponent<Bullet>().SetVFXScale(25f);
        bullet.GetComponent<Bullet>().moveSpeed = 0f;

        bullet.transform.GetChild(0).gameObject.SetActive(false);
        yield return new WaitForSeconds(.1f);
        bullet.transform.GetChild(0).gameObject.SetActive(true);
        bullet.transform.localScale = Vector3.one * .01f;

        float counter = 0f; //Counter to keep track of time elapsed
        while (counter < chargeTime) //This while loop scales object over time
        {
            counter += Time.deltaTime;
            bullet.transform.localScale = Vector3.Lerp(bullet.transform.localScale, originalScale, counter / chargeTime);
            vfxPercent = Mathf.Lerp(1f, 100f, counter / chargeTime);
            //Debug.Log(vfxPercent);
            bullet.GetComponent<Bullet>().SetVFXScale(vfxPercent);
            yield return null;
        }

        //Waits at max scale size for a bit before moving
        yield return new WaitForSeconds(delayBeforeFiring);
        //Fires Projectile after waiting
        bullet.GetComponent<Bullet>().moveSpeed = originalSpeed;
        bullet.GetComponent<Bullet>().canDamage = true;

        yield return new WaitForSeconds(rateOfFire - delayBeforeFiring - chargeTime);
        StartCoroutine(AttackCycle());

        //bullet.GetComponent<Bullet>().SetVFXScale(100f);
    }
}
