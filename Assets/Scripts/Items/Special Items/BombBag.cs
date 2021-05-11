using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombBag : MonoBehaviour
{
    //Variable Initialization/Declaration
    public GameObject BombAttack; //Sets the Bomb Attack Game Object so we can get the prefab for the actual attack 

    /// <summary>
    /// Function to have the bomb attack spawn and start counting down for the explosion - AHL (3/9/21)
    /// Takes in the current player position and direction direction
    /// </summary>
    public void spawnBomb(Vector3 playerPos, Quaternion playerRotation)
    {
        Instantiate(BombAttack, playerPos, playerRotation); //Spawns the bomb attack at the current player location
        AudioManager.Instance.Play("Fuse");
    }
}
