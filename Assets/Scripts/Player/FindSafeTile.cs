﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindSafeTile : SingletonPattern<FindSafeTile>
{
    /**
     * Script: FindSafeTile
     * Programmer: Justin Donato
     * Description: Finds and stores a position to move the player back to when called
     * Date Created: 2/22/2021
     * Date Last Edited: 2/22/2021
     **/

    [HideInInspector]public Vector3 safePos;

    private bool respawningPlayer = false;

    private void Start()
    {
        StartCoroutine(LocateSafePosition());
    }

    /// <summary>
    /// Every second, tests if there is a tile below. If there is a tile, determines if it is a safe tile. If it is safe, stores position as safe position
    /// </summary>
    /// <returns></returns>
    private IEnumerator LocateSafePosition()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit)) //Sends a raycast to look for an object below this one
        {
            if(hit.transform.gameObject.GetComponent<Tile>()) //If the raycast finds an object, this finds out if that object is a tile
            {
                if(hit.transform.gameObject.GetComponent<Tile>().tileType == Tile.tileTypes.pit || hit.transform.gameObject.GetComponent<Tile>().tileType == Tile.tileTypes.lava || hit.transform.gameObject.GetComponent<Tile>().tileType == Tile.tileTypes.spike)
                {
                    //Debug.Log("Not a safe tile!");                   
                }
                else if(transform.position.y > 2f)
                {                    
                    safePos = transform.position;
                    //Debug.Log(hit.transform.gameObject.GetComponent<Tile>().tileType);
                }
            }
            else if(hit.transform.gameObject.GetComponent<CenterTile>())
            {
                safePos = transform.position;
            }
        }

        yield return new WaitForSeconds(1f);
        StartCoroutine(LocateSafePosition());
    }

    public void MovePlayerToSafeLocation(float pitDamage)
    {
        StartCoroutine(RespawnPlayer(pitDamage));
    }

    /// <summary>
    /// I had to make this because unity hates me and moving the player can't just be as simple as transform.position :(
    /// </summary>
    /// <returns></returns>
    private IEnumerator RespawnPlayer(float pitDamage)
    {
        if(!respawningPlayer) //Make sure two pits don't both try to respawn the player
        {
            respawningPlayer = true;

            //Wait briefly to respawn for more dramatic effect
            yield return new WaitForSeconds(1f);

            //Deal Damage
            if (LevelManager.Instance.currFloor != 0)//don't deal damage on level 0
            {
                if (!PlayerHealth.Instance.isInvincible)
                    AnalyticsEvents.Instance.PlayerDamaged("Pit"); //Sends analytics event about damage source

                PlayerHealth.Instance.Damage(pitDamage, gameObject);
            }

            //Disable Character Controller in order to set player position
            GetComponent<CharacterController>().enabled = false;
            yield return new WaitForEndOfFrame();
            transform.position = safePos;
            yield return new WaitForEndOfFrame();

            //Re-enable Character Controller
            GetComponent<CharacterController>().enabled = true;
            respawningPlayer = false;
        }
    }

    public void TeleportPlayer(Vector3 pos)
    {
        StartCoroutine(MovePlayer(pos));
    }

    private IEnumerator MovePlayer(Vector3 pos)
    {
        GetComponent<CharacterController>().enabled = false;
        yield return new WaitForEndOfFrame();
        transform.position = pos;
        yield return new WaitForEndOfFrame();
        GetComponent<CharacterController>().enabled = true;
    }
}
