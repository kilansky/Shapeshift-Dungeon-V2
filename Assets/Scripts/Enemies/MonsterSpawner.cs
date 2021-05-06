using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MonsterInfo
{
    public string name; //Just used to make inspector elements easier to read
    public GameObject monster; //Monster prefab
    [Range(1, 20)] public int spawnWeight; //Chance for monster to be selected
}

[System.Serializable]
public class FloorSpawnInfo
{
    public MonsterInfo[] monsters; //the list of potential monsters to spawn
    [Range(0, 100)] public int totalMonsters; //total # of monsters to spawn
    [Range(0, 30)] public int maxMonsters; //# of monsters that can be in the room at once
    public int gemsOnFloor;

    private List<GameObject> monsterRaffle = new List<GameObject>();

    /// <summary>
    /// Generates a list of monsters based on their spawn weights for random selection
    /// </summary>
    public void CreateSpawnList()
    {
        foreach (MonsterInfo monster in monsters)
        {
            for (int i = 0; i < monster.spawnWeight; i++)
            {
                monsterRaffle.Add(monster.monster);
            }
        }
    }

    /// <summary>
    /// Selects a monster from the raffle
    /// </summary>
    /// <returns></returns>
    public GameObject GetMonsterToSpawn()
    {
        return monsterRaffle[Random.Range(0, monsterRaffle.Count)];
    }
}

public class MonsterSpawner : SingletonPattern<MonsterSpawner>
{
    //public variables
    //public FloorSpawnInfo[] floorSpawnInfo = new FloorSpawnInfo[31];
    [Range(0,3)] public float timeBetweenSpawns = 0.5f; //How long to wait before allowing another monster to spawn
    [Range(0, 10)] public float disableSpawnerTime = 5f; //How long before a spawner can be used again
    [HideInInspector] public bool floorCleared = false; //Room is cleared of monsters
    [HideInInspector] public int gemMonstersToSpawn;
    [HideInInspector] public FloorSpawnInfo currFloorInfo;

    //private variables
    private List<SpawnPoint> monsterSpawnPoints = new List<SpawnPoint>();
    private List<Tile> dirtAndSandTiles = new List<Tile>();
    private Queue<SpawnPoint> disabledSpawnQueue = new Queue<SpawnPoint>();
    private int monstersInRoom = 0;
    private int monstersSpawned = 0;
    private int monstersKilled = 0;
    private int currFloor;
    private bool isSpawningMonsters;

    private int failCounter = 0;

    private void Start()
    {
        isSpawningMonsters = false;
        //BeginSpawingMonsters();
    }

    public void BeginSpawingMonsters()
    {
        //Debug.Log("Spawning monsters!");
        currFloor = LevelManager.Instance.currFloor;

        gemMonstersToSpawn = currFloorInfo.gemsOnFloor;

        if (LevelManager.Instance.currFloor % 5 != 0)//Check if current floor is not a shop
        {
            //Debug.Log("Starting Floor " + currFloor);
            AnalyticsEvents.Instance.FloorStarted();//Send Floor Started Analytics Event

            //Clear the spawn point list of any previously set spawn points
            monsterSpawnPoints.Clear();
            dirtAndSandTiles.Clear();

            //Set up spawn point list based on current room tiles
            foreach (SpawnPoint spawnPoint in GameObject.FindObjectsOfType<SpawnPoint>())
            {
                monsterSpawnPoints.Add(spawnPoint);
                spawnPoint.onAtStart = true;
            }

            foreach (Tile tile in GameObject.FindObjectsOfType<Tile>())
            {
                //Check if the tile is safe to stand on
                if (tile.tileType == Tile.tileTypes.dirt || tile.tileType == Tile.tileTypes.sand)
                {
                    dirtAndSandTiles.Add(tile);
                }
            }

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
        monstersInRoom--;

        if (!floorCleared)//if the floor has not been cleared yet, attempt to spawn an additional monster
        {
            monstersSpawned--;
            if (!isSpawningMonsters)
                SpawnMonsters();
        }
        else//if the floor has already been cleared, just kill the last monster and pretend things are normal
            monstersKilled++;
    }

    //Spawns a single monster, waits, and is called again recursively until all monsters have been killed
    private void SpawnMonsters()
    {
        isSpawningMonsters = true;

        //Only spawn if the # of monsters in the room is less than maxMonsters
        if (monstersInRoom < currFloorInfo.maxMonsters && monsterSpawnPoints.Count > 0)
        {
            //Get a random spawn point index
            int randSpawnPoint = Random.Range(0, monsterSpawnPoints.Count);

            //Get a random monster to spawn
            //int randMonster = Random.Range(0, currFloorInfo.monsters.Length);
            GameObject monsterToSpawn = currFloorInfo.GetMonsterToSpawn();

            if(monsterToSpawn.GetComponent<Worm>())
            {
                int randomTile = Random.Range(0, dirtAndSandTiles.Count);
                dirtAndSandTiles[randomTile].spawnerIndicator.SetActive(true);
                dirtAndSandTiles[randomTile].spawnerIndicator.GetComponent<SpawnPoint>().SpawnMonster(monsterToSpawn, CheckForGem());       
                if(!dirtAndSandTiles[randomTile].spawnerIndicator.GetComponent<SpawnPoint>().onAtStart)
                {
                    StartCoroutine(DisableWormSpawner(dirtAndSandTiles[randomTile].spawnerIndicator));
                }
                monstersInRoom++;
                monstersSpawned++;
            }
            else
            {
                //Spawn the monster and disable the spawn point temporarily
                monsterSpawnPoints[randSpawnPoint].SpawnMonster(monsterToSpawn, CheckForGem());
                monstersInRoom++;
                monstersSpawned++;
                StartCoroutine(DisableSpawner(randSpawnPoint));
            }           
        }

        //Recursively attempt to spawn until the total # of monsters to spawn have been killed
        if (monstersSpawned < currFloorInfo.totalMonsters)
            StartCoroutine(WaitToSpawnAgain());
        else
            isSpawningMonsters = false;
    }

    /// <summary>
    /// Checks if a spawned enemy will be gem monster based off of how many monsters have already spawned and how many gem monsters need to spawn still
    /// </summary>
    /// <returns></returns>
    private bool CheckForGem()
    {
        //Debug.Log("This floor will have " + floorSpawnInfo[currFloor].totalMonsters + " monsters. " + monstersSpawned + " have spawned");


        if (gemMonstersToSpawn == 0) //If there are no more gem monsters to spawn, simply return false
            return false;
        else
        {
            //Uses a roll check to see if a monster will be spawned as a gem monster
            int random = Random.Range(1, currFloorInfo.totalMonsters - monstersSpawned);

            //A successful roll lands when the random number rolls less than or equal to the gem monsters remaining amount
            //Rolls from 1 (always at least 1 enemy to spawn if enemy is being spawned) to enemies left to spawn
            if (random <= gemMonstersToSpawn)
            {
                //Debug.Log("Successful gem roll of " + random + " out of " + (floorSpawnInfo[currFloor].totalMonsters - monstersSpawned));
                --gemMonstersToSpawn;
                return true;
            }
            else
            {
                //Debug.Log("Unsuccessful gem roll of " + random + " out of " + (floorSpawnInfo[currFloor].totalMonsters - monstersSpawned));
                return false;
            }
        }
    }

    //Call this from Enemy class whenever a monster is killed
    public void MonsterKilled()
    {
        monstersKilled += 1;
        monstersInRoom -= 1;

        if(monstersKilled == currFloorInfo.totalMonsters)
        {
            monstersKilled = 0;
            monstersSpawned = 0;

            floorCleared = true;
            PedestalManager.Instance.LoadPedestals(); //Activate the item pedestals
            LevelManager.Instance.ToggleHazards(false); //Disabled level hazards

            //Debug.Log("Cleared Floor " + currFloor);
            AnalyticsEvents.Instance.FloorCompleted(); //Send Floor Completed Analytics Event

            AudioManager.Instance.Play("BigBell");
            CineShake.Instance.Shake(1f, 2f);
            MusicManager.Instance.FloorCleared();
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

    IEnumerator DisableWormSpawner(GameObject spawner)
    {
        yield return new WaitForSeconds(5f);
        spawner.SetActive(false);
    }

    public void SetSpawnInfo(FloorSpawnInfo info)
    {
        currFloorInfo = info;
        //Debug.Log("Received new spawn info: Contains");
        foreach(MonsterInfo monsterInfo in currFloorInfo.monsters)
        {
            //Debug.Log(monsterInfo.monster);
        }
    }
}
