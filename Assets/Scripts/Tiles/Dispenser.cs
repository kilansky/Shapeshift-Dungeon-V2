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
     * Date Last Edited: 4/10/2021 by Sky
     **/

    [Header("Dispenser Variables")]
    [SerializeField] private GameObject projectile;
    [SerializeField] private Transform firePoint;
    
    [Header("Attacking Variables")]
    [SerializeField] private float rateOfFire = 3f;
    [SerializeField] private float chargeTime = 1f;
    [SerializeField] private float delayBeforeFiring = 0.5f;

    private bool canFire = false;

    private void Start()
    {
        if(chargeTime + delayBeforeFiring > rateOfFire)
        {
            Debug.LogError("Charge time is greater than the rate of fire!");
            return;
        }
        //StartCoroutine(AttackCycle());
    }

    public void ToggleFiring(bool enabled)
    {
        canFire = enabled;
        if(enabled)
            StartCoroutine(AttackCycle());
    }

    private IEnumerator AttackCycle()
    {
        //Spawns in projectile and sets its scale to 0 and slowly begins to scale up to proper size over chargeTime seconds
        GameObject bullet = Instantiate(projectile, firePoint.position, transform.rotation, transform);
        Vector3 originalScale = bullet.transform.localScale;
        float originalSpeed = bullet.GetComponent<Bullet>().moveSpeed;
        float vfxPercent = 0;

        //Sets the bullets parent game object to this one to make the Damage Tracker acquire the skull as the correct game object for tracking
        bullet.GetComponent<Bullet>().parentObject = transform.root.gameObject;

        //bullet.GetComponent<Bullet>().SetVFXScale(25f);
        bullet.GetComponent<Bullet>().moveSpeed = 0f;

        bullet.transform.GetChild(0).gameObject.SetActive(false);
        yield return new WaitForSeconds(.1f);

        if(bullet)
        {
            bullet.transform.GetChild(0).gameObject.SetActive(true);
            bullet.transform.localScale = Vector3.one * .01f;
        }

        float counter = 0f; //Counter to keep track of time elapsed
        while (bullet && counter < chargeTime) //This while loop scales object over time
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

        if (bullet)
        {
            //Fires Projectile after waiting
            bullet.GetComponent<Bullet>().moveSpeed = originalSpeed;
            bullet.GetComponent<Bullet>().canDamage = true;
            //bullet.GetComponent<Bullet>().shotBy = "Dispenser";

            yield return new WaitForSeconds(rateOfFire - delayBeforeFiring - chargeTime);
        }

        if(canFire)
            StartCoroutine(AttackCycle());
    }
}
