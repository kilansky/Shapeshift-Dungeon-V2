using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnInfo : MonoBehaviour
{
    public FloorSpawnInfo spawnInfo;

    private void Start()
    {
        spawnInfo.CreateSpawnList(); //Generates the spawn list
    }
}
