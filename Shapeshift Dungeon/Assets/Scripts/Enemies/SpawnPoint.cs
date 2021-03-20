using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public GameObject spawnVFX;
    public Vector3 offset;

    //Spawns a monster on a tile (called from MonsterSpawner)
    public void SpawnMonster(GameObject monsterToSpawn)
    {
        Instantiate(monsterToSpawn, transform.position + offset, Quaternion.identity);

        if(spawnVFX != null)
            Instantiate(spawnVFX, transform.position, Quaternion.identity);
    }
}
