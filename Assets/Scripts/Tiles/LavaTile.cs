using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaTile : MonoBehaviour
{
    /**
     * Script: LavaTile
     * Programmer: Justin Donato
     * Description: Handles lava interaction
     * Date Created: 2/20/2021
     * Date Last Edited: 2/22/2021
     **/

    [SerializeField] private float damage = 1f;
    //[SerializeField] private float rate = 1f;
    [SerializeField] private float damageDelay = .2f;
    private bool lavaHit = false;

    /// <summary>
    /// Detects if something is on this lava tile. If its a player, deals damage. Will add logic for enemies later
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerStay(Collider other)
    {
        if(!lavaHit && other.GetComponent<PlayerController>() && !PlayerController.Instance.IsDashing)
            StartCoroutine(LavaCycle(other.gameObject));
    }

    private IEnumerator LavaCycle(GameObject target)
    {
        lavaHit = true;

        if (!PlayerHealth.Instance.isInvincible)
            AnalyticsEvents.Instance.PlayerDamaged("Lava"); //Sends analytics event about damage source

        PlayerHealth.Instance.Damage(damage, gameObject);
        PlayerController.Instance.transform.GetComponent<StatusEffects>().fireStatus(1f);
        yield return new WaitForSeconds(damageDelay);
        lavaHit = false;
    }
}