using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LazerWand : MonoBehaviour
{
    //Variable Initialization/Declaration
    private GameObject PlayerLazerAttack; //Sets Lazer Wand ItemEquipment so we can get the prefab for the actual attack 
    public List<GameObject> Lazers; //List to hold all the possible lazers to be spawned

    /// <summary>
    /// Function to get the Lazer Attack to spawn in the direction that the player is facing - AHL (4/13/21)
    /// Takes in the current player position, direction and rotation
    /// </summary>
    public void spawnLazer(Vector3 playerPos, Vector3 playerDir, Quaternion playerRotation)
    {
        PlayerLazerAttack = Lazers[Random.Range(0, Lazers.Count)].gameObject;

        Vector3 spawnPos = playerPos + playerDir; //Sets the vector position to spawn the Lazer attack in front of the player
        spawnPos.y += 2f; //Adjusts the Lazer to spawn in the correct position to the player

        GameObject playerLazer = Instantiate(PlayerLazerAttack, spawnPos, playerRotation); //Spawns the Lazer attack
        playerLazer.GetComponent<Laser>().parentObject = PlayerController.Instance.gameObject;
    }
}
