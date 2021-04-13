using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPedestal : MonoBehaviour
{
    /*
     * Script: ItemPedestal
     * Programmer: Justin Donato
     * Description: Handles behaviors of pedestals
     * Date Created: 3/1/2021
     * Date Last Edited: 3/7/2021
     */

    public ItemsEquipment itemToDisplay;
    public GameObject colliderBox;
    public bool isRandom;
    public bool isRandomSpecial;

    [HideInInspector] public GameObject item;

    private void Start()
    {
        if (isRandomSpecial)
        {
            item = Instantiate(ItemPool.Instance.randomSpecialSpawn().prefab, transform.position + new Vector3(0, 7.2f, 0), transform.rotation, transform);
        }
    }

    /// <summary>
    /// Toggles the collider that prevents the player from falling into the hole created when pedestals are rising up
    /// </summary>
    /// <param name="state"></param>
    public void SetCollider(bool state)
    {
        colliderBox.SetActive(state);
    }

    /// <summary>
    /// Determines which item to spawn based on boolean and current floor
    /// </summary>
    public void SpawnItem()
    {       
        if(isRandom) //Checks if pedestal is set to spawn random item
        {
            if(LevelManager.Instance.currFloor >= 5) //Checks if the current level is passed floor 5
            {
                int rnd = Random.Range(0, 100); //If it is, rolls for percent chance on drops. 15% for small potion, 15% for gem pouch, and 70% chance for random item
                if(rnd <= 14)
                {
                    item = Instantiate(ItemPool.Instance.smallPotion.prefab, transform.position + new Vector3(0, 7.2f, 0), transform.rotation, transform);
                }
                else if(rnd > 14 && rnd <= 29)
                {
                    item = Instantiate(ItemPool.Instance.gemBag.prefab, transform.position + new Vector3(0, 7.2f, 0), transform.rotation, transform);
                }
                else
                    item = Instantiate(ItemPool.Instance.randomItemSpawn().prefab, transform.position + new Vector3(0, 7.2f, 0), transform.rotation, transform);
            }
            else //If it isn't passed floor 5 yet, spawns a random item
                item = Instantiate(ItemPool.Instance.randomItemSpawn().prefab, transform.position + new Vector3(0, 7.2f, 0), transform.rotation, transform);
        }
        else //If pedestal is not a random pedestal, it spawns a stat potion
        {
            item = Instantiate(ItemPool.Instance.statPotion.prefab, transform.position + new Vector3(0, 7.2f, 0), transform.rotation, transform);
        }
    }

    /// <summary>
    /// Starts coroutine. Called from Tile script
    /// </summary>
    public void StartColliderCycle()
    {
        StartCoroutine(PedestalColliderCycle());
    }

    /// <summary>
    /// Sets the states of the center tile and the collider while moving
    /// </summary>
    /// <returns></returns>
    private IEnumerator PedestalColliderCycle()
    {
        CenterTile.Instance.canTransition = false;
        GetComponent<ItemPedestal>().SetCollider(true);

        yield return new WaitForSeconds(2 * LevelManager.Instance.transitionTime);

        GetComponent<ItemPedestal>().SetCollider(false);

        CenterTile.Instance.canTransition = true;
        CenterTile.Instance.SetTextState(); //Enable the glow of the center tile number
    }
}
