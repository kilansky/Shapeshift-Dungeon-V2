using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LevelManager : SingletonPattern<LevelManager>
{
    /*
     * Script: LevelManager
     * Programmer: Justin Donato
     * Description: Handles the mechanics of the level such as transitioning from one level to the next
     * Date Created: 2/11/2021
     * Date Last Edited: 3/29/2021
     */

    public GameObject activeLevel;
    public GameObject forceLevelOne;
    public bool disableSpawning;
    public int currFloor = 0;

    [Header("Level Sets")]
    public List<GameObject> levelSet1;
    public List<GameObject> levelSet2;
    public List<GameObject> levelSet3;
    public List<GameObject> levelSet4;
    public List<GameObject> levelSet5;
    public List<GameObject> levelSet6;

    [Header("Shop stages")]
    public GameObject shop1;
    public GameObject shop2;
    public GameObject shop3;
    public GameObject shop4;
    public GameObject shop5;
    public GameObject shop6;

    [HideInInspector] public int levelsCompleted = 0;
    [HideInInspector] public string currMapName; //Used for level feedback forms

    [Header("Transition Variables")]
    public float transitionTime = 3f;
    [SerializeField] private float minStartTime = 0f;
    [SerializeField] private float maxStartTime = 2f;

    [Header("Prefabs")]
    public GameObject pedestalPrefab;
    //public GameObject invisibleCollider;

    [Header("Nav Mesh Area Baking")]
    public GameObject navMeshHazards;

    private bool isTransitioning = false;
    private bool isPlayingBossMusic = false;
    private int enemiesRemaining;

    /// <summary>
    /// Transitions from one level to the next
    /// </summary>
    public void TransitionLevel()
    {
        PedestalManager.Instance.ClearPedestals();

        if(isTransitioning)
        {
            Debug.LogError("Level is already transitioning!");
            return;
        }

        if(PlayerController.Instance.hasRedHerb == true) //If the player has the red herb then they heal 3 HP
        {
            PlayerHealth.Instance.Heal(3); //Heal 3 HP
        }

        isTransitioning = true;
        CenterTile.Instance.SetInvisibleWall(true);
        CameraController.Instance.ZoomOutLevelTransition();

        //LoadNextLevel(levelSet1); //Loads next level
        ++currFloor;
        if(forceLevelOne)
        {
            Debug.LogWarning("Force loading level: " + forceLevelOne.name);
            forceLevelOne.SetActive(true);
            MonsterSpawner.Instance.SetSpawnInfo(forceLevelOne.GetComponent<SpawnInfo>().spawnInfo);
            forceLevelOne = null;
        }
        else
            LoadNextLevel(SelectLevelList());

        CineShake.Instance.Shake(1.5f, 2* transitionTime + maxStartTime);
        AudioManager.Instance.Play("Rumble");
        Transform[] allChildrenCurrLevel = activeLevel.GetComponentsInChildren<Transform>(); //Puts all tiles into an array
        foreach(Transform tile in allChildrenCurrLevel) //Cycles through all tiles in the newly created array
        {
            if(tile.GetComponent<Tile>()) //If the object selected is a tile
            {
                tile.GetComponent<Tile>().DoTransition(transitionTime, Random.Range(minStartTime, maxStartTime)); //Runs the function to initiate a transition
            }
        }
        //++currFloor;
        ++levelsCompleted;
        MonsterSpawner.Instance.floorCleared = false;
        CenterTile.Instance.SetFloorText(currFloor);
        CenterTile.Instance.SetTextState(); //Disable the glow of the center tile number
        StartCoroutine(WaitForTransition());    
    }

    public void ChangeBossLevel(GameObject nextLevel)
    {
        if (isTransitioning)
        {
            Debug.LogError("Level is already transitioning!");
            return;
        }

        isTransitioning = true;

        nextLevel.SetActive(true);

        CineShake.Instance.Shake(1.5f, 2 * transitionTime + maxStartTime);
        AudioManager.Instance.Play("Rumble");

        Transform[] allChildrenCurrLevel = activeLevel.GetComponentsInChildren<Transform>(); //Puts all tiles into an array
        foreach (Transform tile in allChildrenCurrLevel) //Cycles through all tiles in the newly created array
        {
            if (tile.GetComponent<Tile>()) //If the object selected is a tile
            {
                tile.GetComponent<Tile>().DoTransition(transitionTime, Random.Range(minStartTime, maxStartTime)); //Runs the function to initiate a transition
            }
        }

        StartCoroutine(WaitForTransition());
    }

    /// <summary>
    /// Loads in the next level based on the list of levels available
    /// </summary>
    public void LoadNextLevel(List<GameObject> levelList)
    {
        if(levelList == null)
        {
            //Debug.Log("No level list detected. Probably loaded a shop");
            return;
        }

        if(levelList.Count == 0) //If the list of levels is empty, a log message is sent and the function returns
        {
            Debug.LogError("No levels to load!");
            return;
        }

        int rnd = Random.Range(0, levelList.Count); //Generates a random number to use as the index of the list of levels
        //Debug.Log("Index Selected: " + rnd + "     Total Levels in List: " + mainLevels.Count);
        levelList[rnd].SetActive(true); //Sets the selected level to active
        currMapName = levelList[rnd].name;
        //MonsterSpawner.Instance.currFloorInfo = levelList[rnd].GetComponent<SpawnInfo>().spawnInfo;
        MonsterSpawner.Instance.SetSpawnInfo(levelList[rnd].GetComponent<SpawnInfo>().spawnInfo);
        levelList.RemoveAt(rnd); //Removes selected level from list so it cannot be selected again in the future
    }

    /// <summary>
    /// Selects a level list to load based on what level the player is on. Every 5th floor loads a shop
    /// </summary>
    /// <returns></returns>
    private List<GameObject> SelectLevelList()
    {
        //Debug.Log("Current floor: " + currFloor);
        if(currFloor >= 0 && currFloor < 5)
        {
            return levelSet1;
        }
        else if(currFloor == 5)
        {
            shop1.SetActive(true);
            return null;
        }
        else if(currFloor >= 6 && currFloor < 10)
        {
            return levelSet2;
        }
        else if (currFloor == 10)
        {
            shop2.SetActive(true);
            return null;
        }
        else if (currFloor >= 11 && currFloor < 15)
        {
            return levelSet3;
        }
        else if (currFloor == 15)
        {
            shop3.SetActive(true);
            return null;
        }
        else if (currFloor >= 16 && currFloor < 20)
        {
            return levelSet4;
        }
        else if (currFloor == 20)
        {
            shop4.SetActive(true);
            return null;
        }
        else if (currFloor >= 21 && currFloor < 25)
        {
            return levelSet5;
        }
        else if (currFloor == 25)
        {
            shop5.SetActive(true);
            return null;
        }
        else if (currFloor >= 26 && currFloor < 30)
        {
            return levelSet2;
        }
        else if (currFloor == 30)
        {
            shop2.SetActive(true);
            return null;
        }
        else
        {
            Debug.LogError("Level out of range!");
            return null;
        }
    }  

    /// <summary>
    /// Waits until the transition is done to do more logic
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitForTransition()
    {
        if(!isPlayingBossMusic)
            MusicManager.Instance.FadeOut(0f, 1.5f * transitionTime + maxStartTime);

        yield return new WaitForSeconds(2 * transitionTime + maxStartTime);
        //Debug.Log("TransitionComplete!");
        isTransitioning = false; //Sets boolean back to false so transition can occur again
        CenterTile.Instance.SetInvisibleWall(false); //Sets invisible walls back to disabled
        if (!disableSpawning) //Checks if the debug bool is set
        {
            MonsterSpawner.Instance.BeginSpawingMonsters(); //Start spawning monsters
        }
        //Set camera zoom & shadows
        CameraController.Instance.ZoomInLevelTransition();
        CameraController.Instance.SetShadows();

        //Build Navigation Mesh
        GetComponent<NavMeshSurface>().BuildNavMesh();
        navMeshHazards.GetComponent<NavMeshSurface>().BuildNavMesh();

        //Activate hazards in the map
        ToggleHazards(true);

        if (currFloor % 5 == 0)
        {
            //Debug.Log("Starting Floor " + currFloor);
            //Debug.Log("Cleared Floor " + currFloor);
            AnalyticsEvents.Instance.FloorStarted();//Send Level Rated Analytics Event
            AnalyticsEvents.Instance.FloorCompleted(); //Send Floor Completed Analytics Event
            MusicManager.Instance.Shop();
        }
        else if (currFloor < 5)
            MusicManager.Instance.ExoticDangers();
        else if (currFloor >= 6 && currFloor <= 9)
            MusicManager.Instance.DungeonDecoration();
        else if (currFloor >= 11 && currFloor <= 19)
            MusicManager.Instance.DungeonDestruction();
        else if(!isPlayingBossMusic)
        {
            isPlayingBossMusic = true;
            MusicManager.Instance.DungeonDestruction();
        }

        if(GameObject.FindObjectOfType<MageBoss>())
        {
            GameObject.FindObjectOfType<MageBoss>().startBossFight = true;
            foreach (DangerTiles dangerousTile in GameObject.FindObjectsOfType<DangerTiles>())
                dangerousTile.ParentToTile();
        }

        //Debug.Log("Current map is: " + currMapName);
    }

    /// <summary>
    /// Allows the hazards on the map to be enabled or disabled
    /// </summary>
    /// <param name="enabled"></param>
    public void ToggleHazards(bool enabled)
    {
        foreach (Transform child in activeLevel.transform)
        {
            if (child.gameObject.GetComponent<LaserDispenser>())
            {
                child.gameObject.GetComponent<LaserDispenser>().ToggleLaser(enabled);
            }

            else if (child.gameObject.GetComponent<Dispenser>())
            {
                child.gameObject.GetComponent<Dispenser>().ToggleFiring(enabled);
            }

            else if (child.gameObject.GetComponent<SpikeTrap>())
            {
                child.gameObject.GetComponent<SpikeTrap>().ToggleSpike(enabled);
            }
        }
    }

    private void Start()
    {
        currFloor = PlayerPrefs.GetInt("startingLevel");
        MonsterSpawner.Instance.floorCleared = false;
        CenterTile.Instance.SetFloorText(currFloor); //Ensures the level display is set correctly on start
        CenterTile.Instance.SetTextState(); //Enables the glow of the center tile number
    }

    /// <summary>
    /// Runs the entire transition process for testing purposes
    /// </summary>
    [ContextMenu("Test Scene Loading")]
    private void DoLevelStuff()
    {
        TransitionLevel();
    }

    [ContextMenu("EnableHazards")]
    private void ForceStartHazards()
    {
        ToggleHazards(true);
    }
}
