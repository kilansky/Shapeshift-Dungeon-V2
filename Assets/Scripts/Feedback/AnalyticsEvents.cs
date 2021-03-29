﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class AnalyticsEvents : SingletonPattern<AnalyticsEvents>
{
    //Sends an analytics event when an item is picked up by the player
    public void ItemTaken(string itemName)
    {
        AnalyticsResult analyticsResult = Analytics.CustomEvent("Item_Taken",
            new Dictionary<string, object> { {"Item_Name", itemName } });

        Debug.Log("ItemTaken analyticsResult: " + analyticsResult);
    }

    //Sends an analytics event when an item is dropped by the player
    public void ItemDropped(string itemNameDropped, string itemNameTaken)
    {
        AnalyticsResult analyticsResult = Analytics.CustomEvent("Item_Dropped",
            new Dictionary<string, object> { { "Item_Dropped", itemNameDropped },
            { "Item_Taken", itemNameTaken } });

        Debug.Log("ItemDropped analyticsResult: " + analyticsResult);
    }

    //Sends an analytics event when a stat upgrade is chosen
    public void StatUpgraded(string statType)
    {
        AnalyticsResult analyticsResult = Analytics.CustomEvent("Stat_Upgraded",
            new Dictionary<string, object> { { "Stat_Type", statType } });

        Debug.Log("StatUpgraded analyticsResult: " + analyticsResult);
    }

    //Sends an analytics event when an item is purchased from a shop
    public void ItemPurchased(string itemName)
    {
        int floorNum = LevelManager.Instance.currFloor;

        AnalyticsResult analyticsResult = Analytics.CustomEvent("Item_Purchased",
            new Dictionary<string, object> { { "Item_Name", itemName },
            { "Floor_Num", floorNum } });

        Debug.Log("ItemPurchased analyticsResult: " + analyticsResult);
    }

    //Sends an analytics event when a floor is started
    public void FloorStarted()
    {
        string mapName = LevelManager.Instance.currMapName;
        int floorNum = LevelManager.Instance.currFloor;
        int playerID = PlayerPrefs.GetInt("UserID");

        AnalyticsResult analyticsResult = Analytics.CustomEvent("Floor_Started",
            new Dictionary<string, object> { { "Map_Name", mapName },
            { "Floor_Num", floorNum }, { "Player_ID", playerID } });

        Debug.Log("FloorStarted analyticsResult: " + analyticsResult);
    }

    //Sends an analytics event when a floor is completed
    public void FloorCompleted()
    {
        string mapName = LevelManager.Instance.currMapName;
        int floorNum = LevelManager.Instance.currFloor;
        int playerID = PlayerPrefs.GetInt("UserID");

        AnalyticsResult analyticsResult = Analytics.CustomEvent("Floor_Completed",
            new Dictionary<string, object> { { "Map_Name", mapName },
            { "Floor_Num", floorNum }, { "Player_ID", playerID } });

        Debug.Log("FloorCompleted analyticsResult: " + analyticsResult);
    }

    //Sends an analytics event when a level is rated
    public void LevelRated(int levelRating)
    {
        string mapName = LevelManager.Instance.currMapName;

        AnalyticsResult analyticsResult = Analytics.CustomEvent("Level_Rated",
            new Dictionary<string, object> { { "Map_Name", mapName },
            { "Rating", levelRating } });

        Debug.Log("LevelRated analyticsResult: " + analyticsResult);
    }

    //Sends an analytics event when the player dies
    public void PlayerDied()
    {
        string mapName = LevelManager.Instance.currMapName;
        int floorNum = LevelManager.Instance.currFloor;
        int playerID = PlayerPrefs.GetInt("UserID");
        int potionCount = PlayerHealth.Instance.GetPotionCount();
        string runTime = RunTimer.Instance.TimerTextValue;

        PlayerController pc = PlayerController.Instance;
        int healthUpgrades = pc.StatMaxHealthCount;
        int attackUpgrades = pc.StatAttackCount;
        int speedUpgrades = pc.StatSpeedCount;

        AnalyticsResult analyticsResult = Analytics.CustomEvent("Player_Died",
            new Dictionary<string, object> { { "Player_ID", playerID },
            { "Floor_Num", floorNum }, { "Map_Name", mapName },
            { "Health_Potions", potionCount }, { "Health_Upgrades", healthUpgrades },
            { "Attack_Upgrades", attackUpgrades }, { "Speed_Upgrades", speedUpgrades },
            { "Run_Time", runTime } });

        Debug.Log("PlayerDied analyticsResult: " + analyticsResult);
    }

    //Sends an analytics event when the player dies about the items they had collected
    public void ItemsOnDeath()
    {
        PlayerController pc = PlayerController.Instance;
        string specialItem = (pc.SpecialSlot ? pc.SpecialSlot.ItemName : "---");
        string headItem = (pc.HeadSlot ? pc.HeadSlot.ItemName : "---");
        string torsoItem = (pc.TorsoSlot ? pc.TorsoSlot.ItemName : "---");
        string footItem = (pc.FootSlot ? pc.FootSlot.ItemName : "---");
        string pocket1Item = (pc.PocketSlot1 ? pc.PocketSlot1.ItemName : "---");
        string pocket2Item = (pc.PocketSlot2 ? pc.PocketSlot2.ItemName : "---");
        int floorNum = LevelManager.Instance.currFloor;
        int playerID = PlayerPrefs.GetInt("UserID");

        AnalyticsResult analyticsResult = Analytics.CustomEvent("Items_On_Death",
            new Dictionary<string, object> { { "Player_ID", playerID },
            { "Floor_Num", floorNum }, { "Special_Item", specialItem },
            { "Head_Item", headItem }, { "Torso_Item", torsoItem },
            { "Foot_Item", footItem }, { "Pocket1_Item", pocket1Item },
            { "Pocket2_Item", pocket2Item } });

        Debug.Log("ItemsOnDeath analyticsResult: " + analyticsResult);
    }
}
