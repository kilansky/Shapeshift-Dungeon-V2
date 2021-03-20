using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowlingBall : MonoBehaviour
{
    //Variable Initialization/Declaration
    public GameObject bowlingBall; //Sets the bowling ball ItemEquipment so we can get the prefab for the actual attack 

    /// <summary>
    /// Function to get have the bowling ball spawn and begin moving in the direction that the player is facing - AHL (3/4/21)
    /// Takes in the current player position, direction and rotation
    /// </summary>
    public void spawnBowlingBall(Vector3 playerPos, Vector3 playerDir, Quaternion playerRotation)
    {
        Vector3 spawnPos = playerPos + playerDir * 2; //Sets the vector position to spawn the bowling ball attack slightly infront of the player

        Instantiate(bowlingBall, spawnPos, playerRotation); //Spawns the bowling ball attack
    }
}
