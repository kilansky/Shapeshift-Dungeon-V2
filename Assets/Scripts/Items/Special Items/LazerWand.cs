﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LazerWand : MonoBehaviour
{
    //Variable Initialization/Declaration
    public GameObject PlayerLazerAttack; //Sets Lazer Wand ItemEquipment so we can get the prefab for the actual attack 

    /// <summary>
    /// Function to get the Lazer Attack to spawn in the direction that the player is facing - AHL (4/13/21)
    /// Takes in the current player position, direction and rotation
    /// </summary>
    public void spawnLazer(Vector3 playerPos, Vector3 playerDir, Quaternion playerRotation)
    {
        Vector3 spawnPos = playerPos + playerDir; //Sets the vector position to spawn the Lazer attack in front of the player
        spawnPos.y += 1f; //Adjusts the Lazer to spawn in the correct position to the player

        GameObject playerLazer = Instantiate(PlayerLazerAttack, spawnPos, playerRotation); //Spawns the Lazer attack
        playerLazer.GetComponent<Laser>().parentObject = PlayerController.Instance.gameObject;
    }
}
