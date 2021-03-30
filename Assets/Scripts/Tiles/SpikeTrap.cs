using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    /**
     * Script: SpikeTrap
     * Programmer: Justin Donato
     * Description: Handles spike trap interactions with players
     * Date Created: 2/19/2021
     * Date Last Edited: 2/20/2021
     **/

    [SerializeField] private GameObject spikeModel;
    [Header("Spike Variables")]
    [SerializeField] private float damage = 8f;
    [SerializeField] private float initialRiseTime = .2f;
    [SerializeField] private float idleTime = .5f;
    [SerializeField] private float attackTime = .1f;
    [SerializeField] private float lowerTime = 1f;

    private bool isTriggered = false;
    private bool isEnabled = true;
    [SerializeField]private List<GameObject> entitiesOnSpike;

    private IEnumerator SpikeCycle()
    {
        if (!isEnabled) //If spike trap is disabled, exit coroutine
            yield break;

        //Raise spikes up a little as a warning
        isTriggered = true;
        float counter = 0f; //Counter to keep track of time elapsed
        Vector3 originalPos = spikeModel.transform.position;
        while (counter < initialRiseTime) //This while loop moves the object to new position over a set amount of time
        {
            counter += Time.deltaTime;
            float yPos = Mathf.Lerp(0, .4f, counter / initialRiseTime);
            spikeModel.transform.position = new Vector3(originalPos.x, originalPos.y + yPos, originalPos.z);
            yield return null;
        }

        //Wait for some amount of time
        yield return new WaitForSeconds(idleTime);

        //Quickly raise spikes all the way and deal damage to entity if entity is still on top of spikes
        counter = 0f;
        originalPos = spikeModel.transform.position;
        while (counter < attackTime) //This while loop moves the object to new position over a set amount of time
        {
            counter += Time.deltaTime;
            float yPos = Mathf.Lerp(0, 1.05f, counter / attackTime);
            spikeModel.transform.position = new Vector3(originalPos.x, originalPos.y + yPos, originalPos.z);
            yield return null;
        }
        DealDamage();

        print("Spikes dealt damage and are now going back down");

        //Wait for some amount of time
        yield return new WaitForSeconds(idleTime);


        //Lower Spikes
        counter = 0f;
        originalPos = spikeModel.transform.position;
        while (counter < lowerTime) //This while loop moves the object to new position over a set amount of time
        {
            counter += Time.deltaTime;
            float yPos = Mathf.Lerp(0, -1.45f, counter / lowerTime);
            spikeModel.transform.position = new Vector3(originalPos.x, originalPos.y + yPos, originalPos.z);
            yield return null;
        }
        isTriggered = false;

        if(entitiesOnSpike.Count > 0) //If there are still entities on the spike, trigger the spike again
        {
            StartCoroutine(SpikeCycle());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            entitiesOnSpike.Add(other.gameObject); //Adds entity to the list of entities on the spike
            if(!isTriggered) //If the spike is not mid trigger, triggers the spike
            {
                StartCoroutine(SpikeCycle());
            }          
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(entitiesOnSpike.Contains(other.gameObject))
        {
            entitiesOnSpike.Remove(other.gameObject); //Removes entities from the list of entities on the spike if they leave the collider
        }
    }

    /// <summary>
    /// Deals damage to all entities on the spike
    /// </summary>
    private void DealDamage()
    {
        foreach (GameObject entity in entitiesOnSpike)
        {
            if(entity.tag == "Player")
            {
                if (!PlayerHealth.Instance.isInvincible)
                    AnalyticsEvents.Instance.PlayerDamaged("Spikes"); //Sends analytics event about damage source

                PlayerHealth.Instance.Damage(damage,gameObject);
            }
        }
    }

    public void ToggleSpike(bool enabled)
    {
        Debug.Log("Spike traps have been toggled to " + enabled);
        isEnabled = enabled;
    }
}
