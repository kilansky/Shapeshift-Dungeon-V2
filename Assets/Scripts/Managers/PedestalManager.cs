using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PedestalManager : SingletonPattern<PedestalManager>
{
    /*
     * Script: PedestalManager
     * Programmer: Justin Donato
     * Description: Handles the behaviors of individual tiles
     * Date Created: 3/4/2021
     * Date Last Edited: 3/8/2021
     */

    public int numberofRandomPedestals = 1;
    public int secondItemPrice = 3;

    private List<GameObject> pedestals = new List<GameObject>();

    [ContextMenu("Test Shop Loading")]
    public void LoadPedestals()
    {
        Transform[] allChildrenCurrLevel = LevelManager.Instance.activeLevel.GetComponentsInChildren<Transform>(); //Puts all tiles into an array
        foreach (Transform tile in allChildrenCurrLevel) //Cycles through all tiles in the newly created array
        {
            //if (tile.GetComponent<Tile>() && tile.GetComponent<Tile>().willBeShop) //If the object selected is a tile and has the shop boolean on
            if(tile.GetComponent<Tile>() && tile.GetComponentInChildren<RewardIndicator>())
            {
                GameObject pedestal = Instantiate(LevelManager.Instance.pedestalPrefab, tile.transform.position - new Vector3(0, 20, 0), new Quaternion(0, 0, 0, 0)); //Instantiate an item pedestal
                pedestals.Add(pedestal);
                tile.GetComponent<Tile>().DoTransition(LevelManager.Instance.transitionTime, 0f); //Runs the function to initiate a transition                        
            }
        }

        SpawnItems(); //Calls spawn item function to instanciate the items

        //StartCoroutine(ShopColliderCycle());
    }

    /// <summary>
    /// Triggers the collider for the pedestal to turn on and then turns it off after waiting for the transition to finish
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    private IEnumerator ShopColliderCycle()
    {
        CenterTile.Instance.canTransition = false;
        foreach (GameObject pedestal in pedestals)
            pedestal.GetComponent<ItemPedestal>().SetCollider(true);

        yield return new WaitForSeconds(2 * LevelManager.Instance.transitionTime);

        foreach (GameObject pedestal in pedestals)
            pedestal.GetComponent<ItemPedestal>().SetCollider(false);

        CenterTile.Instance.canTransition = true;
        //SetShopStates();
    }

    /// <summary>
    /// Determines if a pedestal will be random or set based on amount of random items required then spawns the items
    /// </summary>
    private void SpawnItems()
    {
        int randPedestals = 0;
        foreach(GameObject pedestal in pedestals) //This loop sets pedestals to random items until the number of random pedestals is equal to the amount given by the variable
        {
            if(randPedestals != numberofRandomPedestals)
            {
                pedestal.GetComponent<ItemPedestal>().isRandom = true;
                ++randPedestals;
            }
            else //Once required amount of pedestals is reached, sets pedestals to spawn set items
            {
                pedestal.GetComponent<ItemPedestal>().isRandom = false;
            }

            pedestal.GetComponent<ItemPedestal>().SpawnItem();
        }
    }

    /// <summary>
    /// Deletes the item on top of both pedestals
    /// </summary>
    public void DeactivatePedestals()
    {
        foreach (GameObject pedestal in pedestals)
        {
            if (pedestal.GetComponent<ItemPedestal>().item != null)
            {
                //Destroy(pedestal.GetComponent<ItemPedestal>().item);
                pedestal.GetComponent<ItemPedestal>().item.GetComponent<Item>().SetPrice(secondItemPrice);
            }
        }

        //ClearPedestals();
    }

    /// <summary>
    /// Clears the pedestals list
    /// </summary>
    public void ClearPedestals()
    {
        pedestals.Clear();
    }
}
