using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public GameObject spawnVFX;
    public Vector3 offset;

    //Spawns a monster on a tile (called from MonsterSpawner)
    public void SpawnMonster(GameObject monsterToSpawn/*, bool isGemMonster*/)
    {
        GameObject monster = Instantiate(monsterToSpawn, transform.position + offset, Quaternion.identity);

        /*
        if(isGemMonster)
        {
            //Set monster to gem monster
            //Debug.Log("Spawned a game monster!");
        }
        */
       

        if(spawnVFX != null)
            Instantiate(spawnVFX, transform.position, Quaternion.identity);
    }
}
