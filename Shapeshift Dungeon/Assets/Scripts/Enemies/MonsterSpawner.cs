﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FloorSpawnInfo
{
    public GameObject[] monsters; //the list of potential monsters to spawn
    [Range(0, 100)] public int totalMonsters; //total # of monsters to spawn
    [Range(0, 30)] public int maxMonsters; //# of monsters that can be in the room at once
}

public class MonsterSpawner : SingletonPattern<MonsterSpawner>
{
    public FloorSpawnInfo[] floorSpawnInfo = new FloorSpawnInfo[31];
    [Range(0,3)] public float timeBetweenSpawns = 0.5f; //How long to wait before allowing another monster to spawn
    [Range(0, 10)] public float disableSpawnerTime = 5f; //How long before a spawner can be used again
    [HideInInspector] public bool floorCleared = true; //Room is cleared of monsters

    //private GameObject[] monsterSpawnPoints;
    private List<SpawnPoint> monsterSpawnPoints = new List<SpawnPoint>();
    private Queue<SpawnPoint> disabledSpawnQueue = new Queue<SpawnPoint>();
    private int monstersInRoom = 0;
    private int monstersSpawned = 0;
    private int monstersKilled = 0;
    private int currFloor;

    private void Start()
    {
        currFloor = 1;
        BeginSpawingMonsters();
    }

    public void BeginSpawingMonsters()
    {
        currFloor = LevelManager.Instance.currFloor;

        if (LevelManager.Instance.currFloor % 5 != 0)//Check if current floor is not a shop
        {
            //Clear the spawn point list of any previously set spawn points
            monsterSpawnPoints.Clear();

            //Set up spawn point list based on current room tiles
            foreach (SpawnPoint spawnPoint in GameObject.FindObjectsOfType<SpawnPoint>())
                monsterSpawnPoints.Add(spawnPoint);

            SpawnMonsters();
        }
        else //if floor is a shop, don't spawn monster, instantly clear the floor
        {
            floorCleared = true;
            CenterTile.Instance.SetTextState();
        }
    }

    //Called when monsters are spawned above/below the map and killed instantly
    public void MonsterKilledPrematurly()
    {
        if (!floorCleared)
        {
            monstersInRoom--;
            monstersSpawned--;
            SpawnMonsters();
            Debug.Log("Attempted to spawn a monster to replace one that died from killbox");
        }
    }

    //Spawns a single monster, waits, and is called again recursively until all monsters have been killed
    private void SpawnMonsters()
    {
        //Only spawn if the # of monsters in the room is less than maxMonsters
        if (monstersInRoom < floorSpawnInfo[currFloor].maxMonsters && monsterSpawnPoints.Count > 0)
        {
            //Get a random spawn point index
            int randSpawnPoint = Random.Range(0, monsterSpawnPoints.Count);

            //Get a random monster to spawn
            int randMonster = Random.Range(0, floorSpawnInfo[currFloor].monsters.Length);
            GameObject monsterToSpawn = floorSpawnInfo[currFloor].monsters[randMonster];

            //Spawn the monster and disable the spawn point temporarily
            monsterSpawnPoints[randSpawnPoint].SpawnMonster(monsterToSpawn);
            monstersInRoom++;
            monstersSpawned++;
            StartCoroutine(DisableSpawner(randSpawnPoint));
        }

        //Recursively attempt to spawn until the total # of monsters to spawn have been killed
        if (monstersSpawned < floorSpawnInfo[currFloor].totalMonsters)
            StartCoroutine(WaitToSpawnAgain());
    }

    //Call this from Enemy class whenever a monster is killed
    public void MonsterKilled()
    {
        monstersKilled += 1;
        monstersInRoom -= 1;

        if(monstersKilled == floorSpawnInfo[currFloor].totalMonsters)
        {
            monstersKilled = 0;
            monstersSpawned = 0;
            Debug.Log("All Monsters Killed, Display Rewards");
            //ROOM CLEARED, TRIGGER ROOM REWARDS <----------------------------------- !!
            floorCleared = true;
            
            //HUDController.Instance.ShowLevelReviewPanel(); //Show the level review panel
            //LevelManager.Instance.LoadShops();
            PedestalManager.Instance.LoadPedestals();
        }
    }

    //Waits briefly to check to spawn again rather than spawning/checking every possible frame
    IEnumerator WaitToSpawnAgain()
    {
        yield return new WaitForSeconds(timeBetweenSpawns);
        SpawnMonsters();
    }

    //Prevents the given spawner from spawning monsters for disableSpawnTime seconds
    IEnumerator DisableSpawner(int spawnerIndex)
    {       
        disabledSpawnQueue.Enqueue(monsterSpawnPoints[spawnerIndex]);
        monsterSpawnPoints.Remove(monsterSpawnPoints[spawnerIndex]);

        yield return new WaitForSeconds(disableSpawnerTime);

        monsterSpawnPoints.Add(disabledSpawnQueue.Dequeue());
    }
}
