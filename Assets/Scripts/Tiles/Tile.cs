using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    /*
     * Script: Tile
     * Programmer: Justin Donato
     * Description: Handles the behaviors of individual tiles
     * Date Created: 2/9/2021
     * Date Last Edited: 3/7/2021
     */

    public enum tileTypes
    {
        stone,
        dirt,
        sand,
        pit,
        lava,
        spike,
        item,
        dispenser,
        stairs,
        arcane,
        bridge,
        rail,
        laser,
        stoneGrass,
        dirtGrass,
        torch
    }
    public tileTypes tileType;

    [Header("Pointers")]
    public GameObject spawnerIndicator;
    public GameObject rewardIndicator;
    public LayerMask mask;

    public bool hasDecor = false;
    public bool forceSwap = false;

    private GameObject nextTile;

    private void Start()
    {
        //if (GetComponent<PropSpawner>())
            //GetComponent<PropSpawner>().SpawnProp();
    }

    /// <summary>
    /// Begins the process for replacing this tile with a new one if it needs to be replaced
    /// </summary>
    /// <param name="time">The time it takes to move down</param>
    /// <param name="delay"></param>
    public void DoTransition(float time, float delay)
    {
        if(NeedsReplacement(GetReplacementTile())) //This line determines if the current tile needs to be replaced with the tile of the next level
        {
            StartCoroutine(MoveDown(time, delay));
        }
        else
        {
            Destroy(nextTile); //Removes the unused tile
        }
    }

    /// <summary>
    /// Uses a raycast to find the gameobject directly below this one. If the object it hits has a tile script on it, it will return the tile. If it doesn't, returns null
    /// </summary>
    /// <returns>
    /// Returns game object with a tile script on it or null if there is no object with tile script on it
    /// </returns>
    public GameObject GetReplacementTile()
    {
        RaycastHit hit;

        if(Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity, mask)) //Sends a raycast to look for an object below this one
        {
            //Debug.Log("Raycast hit: " + hit.transform.name);
            if(hit.transform.gameObject.GetComponent<Tile>()) //If the raycast finds an object, this finds out if that object is a tile
            {
                //The tile gameobject is stored for later use and is returned back to the original script that called it
                nextTile = hit.transform.gameObject;
                return hit.transform.gameObject;
            }
            else //If object is not a tile, return null
            {
                return null;
            }
        }
        else //If raycast finds no object, return null
        {
            return null;
        }
    }

    /// <summary>
    /// Checks to see if the current tile needs to be replaced by comparing their tile type enums
    /// </summary>
    /// <param name="newTile"></param>
    /// <returns></returns>
    public bool NeedsReplacement(GameObject newTile)
    {
        if(newTile == null) //Initial check to make sure that some input was provided. Returns a log message and false if no input was detected
        {
            Debug.LogError("Null input detected under tile at " + transform.position);
            return false;
        }

        if (newTile.GetComponent<Tile>().tileType != tileType || (!hasDecor && newTile.GetComponent<Tile>().hasDecor) || newTile.GetComponent<Tile>().forceSwap || tileType == tileTypes.torch || tileType == tileTypes.arcane) //If the next tile is of a different type than this tile or has decor, do some stuff
        {            
            return true;
        }
        else //If tile is the same type
        {
            //There will be more logic here in the future for specific tiles that need more information
            //Example: a stair that is rotated a different way
            if (newTile.transform.position.y != transform.position.y - 20 //Tests if a tile of the same type is at a different height level than current tile
                || (newTile.transform.rotation != transform.rotation && !NoOrientation()) /*|| newTile.GetComponent<Tile>().willBeShop*/) //Tests if the tiles rotation is different than its replacement and also ignores some rotation differences in certain tiles
            {
                return true;
            }
            else if(NoOrientation()) //If the block doesn't require an orientation, it is also a spawnable block
            {
                spawnerIndicator.SetActive(CheckForSpawner(newTile)); //Sets this block's spawner to the same state as the next tile and then does no movement
                rewardIndicator.SetActive(CheckForReward(newTile)); //Sets this block's spawner to the same state as the next tile and then does no movement
                return false;
            }
            else
            {
                return false;
            }
        }
    }

    /// <summary>
    /// Tile moves down 20 units to meet the tile underneath it and triggers the new tile to move up when done. Deletes this tile when finished
    /// </summary>
    /// <param name="timeToMove"></param>
    /// <returns></returns>
    private IEnumerator MoveDown(float timeToMove, float delay)
    {
        yield return new WaitForSeconds(delay);

        RaycastHit hit;

        while (NoOrientation() && rewardIndicator.activeInHierarchy && Physics.BoxCast(transform.position, transform.localScale, transform.TransformDirection(Vector3.up), out hit)) //Sends a boxcast to look for an object above this one if it is flagged as a shop
        {
            if (hit.transform.CompareTag("Player")) //If boxcast finds player on top of tile, delay the transition
            {
                Debug.Log("Player Hit!");
                yield return null;
            }
            else //If player is not on top of tile, begin transition and enable the colliders on the pedestal
            {
                nextTile.GetComponent<ItemPedestal>().StartColliderCycle();
                break;
            }
        }

        Vector3 originalPos = transform.position;

        if (this.tileType == tileTypes.item && GetComponent<ItemPedestal>().item)
        {
            GameObject itemBase = GetComponent<ItemPedestal>().item.transform.GetChild(0).gameObject;
            foreach (Transform canvas in itemBase.transform)
            {
                canvas.gameObject.layer = 5;//Set UI layer on each child
            }
        }

        float counter = 0f; //Counter to keep track of time elapsed
        while (counter < timeToMove) //This while loop moves the object to new position over a set amount of time
        {
            counter += Time.deltaTime;
            float yPos = Mathf.Lerp(0, 20, counter / timeToMove);
            transform.position = new Vector3(originalPos.x, originalPos.y - yPos, originalPos.z);
            yield return null;
        }
        nextTile.transform.SetParent(LevelManager.Instance.activeLevel.transform); //Sets next tile to be a child of the parent of this tile

        if(nextTile.GetComponent<Tile>().tileType == tileTypes.arcane)
        {
            if(nextTile.GetComponent<ArcaneCircle>().groupScript.gameObject.transform.parent != LevelManager.Instance.activeLevel.transform)
            {
                nextTile.GetComponent<ArcaneCircle>().groupScript.gameObject.transform.SetParent(LevelManager.Instance.activeLevel.transform);
            }
        }

        nextTile.GetComponent<Tile>().StartMoving(timeToMove); //Triggers the next tile to begin moving up   
        
        if(tileType == tileTypes.arcane && GetComponent<ArcaneCircle>().groupScript != null)
        {
            GetComponent<ArcaneCircle>().groupScript.DestroyGroup(); //Destroys the arcane group object so it is not left over
        }

        Destroy(gameObject); //Destroys this object when its job is complete
    }

    /// <summary>
    /// Tile moves down 20 units up
    /// </summary>
    /// <param name="timeToMove"></param>
    /// <returns></returns>
    private IEnumerator MoveUp(float timeToMove)
    {
        Vector3 originalPos = transform.position;

        float counter = 0f; //Counter to keep track of time elapsed
        while (counter < timeToMove) //This while loop moves the object to new position over a set amount of time
        {
            counter += Time.deltaTime;
            float yPos = Mathf.Lerp(0, 20, counter / timeToMove);
            transform.position = new Vector3(originalPos.x, originalPos.y + yPos, originalPos.z);
            yield return null;
        }

        if(this.tileType == tileTypes.item && GetComponent<ItemPedestal>().item)
        {
            GameObject itemBase = GetComponent<ItemPedestal>().item.transform.GetChild(0).gameObject;
            foreach (Transform canvas in itemBase.transform)
            {
                canvas.gameObject.layer = 16;//Set world GUI layer on each child
            }
        }
    }

    /// <summary>
    /// This function serves to initiate the MoveUp coroutine. Is called by another tile object
    /// </summary>
    public void StartMoving(float moveSpeed)
    {
        StartCoroutine(MoveUp(moveSpeed));
    }

    /// <summary>
    /// Checks a tile to see if its spawner is enabled
    /// </summary>
    /// <param name="newTile">Tile to be checked</param>
    /// <returns></returns>
    private bool CheckForSpawner(GameObject newTile)
    {
        return newTile.GetComponent<Tile>().spawnerIndicator.activeInHierarchy;
    }

    private bool CheckForReward(GameObject newTile)
    {
        return newTile.GetComponent<Tile>().rewardIndicator.activeInHierarchy;
    }

    /// <summary>
    /// Checks if a tile does not require a specific orientation. This also functions to check if a tile can have a spawner on it
    /// </summary>
    /// <returns></returns>
    private bool NoOrientation()
    {
        return (tileType == tileTypes.stone || tileType == tileTypes.sand || tileType == tileTypes.dirt || tileType == tileTypes.stoneGrass || tileType == tileTypes.dirtGrass);
    }

    [ContextMenu("Test Transition")]
    private void TestTransition()
    {
        DoTransition(1, 1);
    }
}
