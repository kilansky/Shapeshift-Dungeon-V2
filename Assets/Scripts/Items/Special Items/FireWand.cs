using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireWand : MonoBehaviour
{
    //Variable Initialization/Declaration
    public GameObject PlayerFireBall; //Sets the Firewand ItemEquipment so we can get the prefab for the actual attack 

    /// <summary>
    /// Function to get have the fire ball spawn and begin moving in the direction that the player is facing - AHL (3/29/21)
    /// Takes in the current player position, direction and rotation
    /// </summary>
    public void spawnFireBall(Vector3 playerPos, Vector3 playerDir, Quaternion playerRotation)
    {
        Vector3 spawnPos = playerPos + playerDir; //Sets the vector position to spawn the Fireball attack in front of the player
        spawnPos.y += 1f; //Adjusts the fireball to spawn in the correct position to the player

        Instantiate(PlayerFireBall, spawnPos, playerRotation); //Spawns the fireball attack
    }
}
