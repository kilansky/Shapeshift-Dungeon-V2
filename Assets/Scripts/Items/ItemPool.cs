using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPool : SingletonPattern<ItemPool>
{
    /// <summary>
    /// Inspector to let you adjust the size of the list and add to it each itemEquipment scriptable objects - AHL (3/2/21)
    /// </summary>
    [SerializeField]
    public List<ItemsEquipment> items = new List<ItemsEquipment>();

    [Header("Set Items")]
    public ItemsEquipment statPotion;
    public ItemsEquipment smallPotion;
    public ItemsEquipment gemBag;

    ItemsEquipment currentItem;

    /// <summary>
    /// Function to randomly select an object and display it on the pedestal
    /// AHL (3/2/21)
    /// </summary>
    [ContextMenu("Test Item Pool")]
    public ItemsEquipment randomItemSpawn()
    {
        if (items.Count == 0)
            return gemBag;

        //Debug.Log("Items in the pool: " + items.Count);
        int currIndex = Random.Range(0, items.Count);
        //Debug.Log("Index Selected: " + currIndex);

        ItemsEquipment currentItem = items[currIndex]; //Gets a random item of the item list and removes it but sets it to another ItemEquipment variable

        items.RemoveAt(currIndex);

        return currentItem;
    }

    public ItemsEquipment randomSpecialSpawn()
    {
        List<ItemsEquipment> specialItems = new List<ItemsEquipment>();
        foreach(ItemsEquipment item in items)
        {
            if(item.ItemSlot == 0)
            {
                specialItems.Add(item);
            }          
        }

        int currIndex = Random.Range(0, specialItems.Count);

        ItemsEquipment currentItem = specialItems[currIndex]; //Gets a random item of the item list and removes it but sets it to another ItemEquipment variable

        items.Remove(currentItem);
        return currentItem;
    }

    public ItemsEquipment GetItemNoGem()
    {
        List<ItemsEquipment> poolNoGems = new List<ItemsEquipment>();
        foreach(ItemsEquipment item in items)
        {
            if(item.ItemName != "Gem Bag")
            {
                poolNoGems.Add(item);
            }
        }

        if(poolNoGems.Count == 0)
        {
            Debug.LogWarning("There are not any non-GemBag items remaining in the pool! Retruning a potion");
            return smallPotion;
        }

        int currIndex = Random.Range(0, poolNoGems.Count);
        //Debug.Log("Index Selected: " + currIndex);

        ItemsEquipment currentItem = poolNoGems[currIndex]; //Gets a random item of the item list and removes it but sets it to another ItemEquipment variable

        items.Remove(currentItem);

        return currentItem;
    }
}
