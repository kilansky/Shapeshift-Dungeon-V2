using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// What kind of stat modifier is it? - AHL (2/22/21)
/// </summary>
public enum statModifierType
{
    FlatValue = 100,
    PercentAddValue = 200,
    PercentMultValue = 300
}

/// <summary>
/// Choice of what player stat it adjusts? - AHL (2/23/21)
/// </summary>
public enum playerStatType
{
    None = 0,
    //HP = 1, //AHL (3/8/21) - This variable isn't needed but a placeholder for a stat for later
    MoveSpeed = 2,
    AttackDam = 3,
    AttackSpeed = 4, 
    ChargeAttackTime = 5,
    AttackKnockback = 6, //Not set up yet - check in with sky or joe for later
    DashInvulnerabilityTime = 7, //Not set up yet - Check in with sky for later (Priority)
    DashSpeed = 8, 
    DashRefreshSpeed = 9,
    DamageFromEnemies = 10,
    PotionHealing = 11, // additionalPotionHealing variable 
    SpecialItemRechargeTime = 12, 
}

[System.Serializable]
public class StatMods
{
    public float adjustableValue; //Value that will adjust the player stats
    public statModifierType statModifier; //Modification aspect that will be used to adjust the variable
    public playerStatType statType; //Type of Player stat that will be adjusted
}


/// <summary>
/// What kind of item will it be? - AHL (2/16/21)
/// Made public by Sky - (3/28/21)
/// </summary>
public enum ItemType
{
    Special = 0,
    Head = 1,
    Torso = 2,
    Foot = 3,
    Pocket = 4,
    Pickup = 5
}


[CreateAssetMenu(fileName = "New item/Equipment", menuName = "Item")]
public class ItemsEquipment : ScriptableObject
{
    //Variable Declaration/Initialization List
    public Sprite sprite; //Variable to hold the 2D sprite of the item image
    public GameObject prefab; //Variable to hold the prefab of the gameObject to be generated

    /// <summary>
    /// **AHL - NOTESSSSSS**
    /// Make the multiple stat into an array system so the item can have an adjustable amount
    /// </summary>

    //List of choices to be adjusted for the items - AHL (2/16/21)
    [SerializeField]
    private ItemType _itemSlotType; //Choice of what slot this item goes in
    [SerializeField]
    private string _itemName; //Name of the item
    public string itemDescription; //Text field in the inspector to add in item descriptions
    public StatMods[] statMods; //Array to hold all the modifiers for the item
    

    //Public variables for easy access
    public int ItemSlot { get { return (int)_itemSlotType; } }
    public string ItemName { get { return _itemName; } }
}