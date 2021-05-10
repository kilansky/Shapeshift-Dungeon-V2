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
    [SerializeField] private float damageDelay = .1f;
    private bool lavaHit = false;
    private bool playerOnLava = false;

    /// <summary>
    /// Detects if something is on this lava tile. If its a player, deals damage. Will add logic for enemies later
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerStay(Collider other)
    {
        if(other.GetComponent<PlayerController>() && !PlayerController.Instance.IsDashing)
        {
            playerOnLava = true;

            if (!lavaHit)
                StartCoroutine(LavaCycle(other.gameObject));
        }
        else if (other.GetComponent<PlayerController>() && PlayerController.Instance.IsDashing)
            playerOnLava = false;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerController>())
            playerOnLava = false;
    }

    private IEnumerator LavaCycle(GameObject target)
    {
        lavaHit = true;
        yield return new WaitForSeconds(damageDelay);

        if(playerOnLava)
        {
            if (!PlayerHealth.Instance.isInvincible)
                AnalyticsEvents.Instance.PlayerDamaged("Lava"); //Sends analytics event about damage source

            PlayerHealth.Instance.Damage(damage, gameObject);
            PlayerController.Instance.transform.GetComponent<StatusEffects>().fireStatus(1f);
        }
        lavaHit = false;
    }
}