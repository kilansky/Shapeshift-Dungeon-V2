using System.Collections;
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
    }

    //Sends an analytics event when an item is dropped by the player
    public void ItemDropped(string itemNameDropped, string itemNameTaken)
    {
        AnalyticsResult analyticsResult = Analytics.CustomEvent("Item_Dropped",
            new Dictionary<string, object> { { "Item_Dropped", itemNameDropped },
            { "Item_Taken", itemNameTaken } });
    }

    //Sends an analytics event when a floor is completed
    public void FloorCompleted()
    {
        string mapName = LevelManager.Instance.currMapName;
        int floorNum = LevelManager.Instance.currFloor;
        int playerID = PlayerPrefs.GetInt("UserID");

        AnalyticsResult analyticsResult = Analytics.CustomEvent("Floor_Completed",
            new Dictionary<string, object> { { "Map_Name", mapName },
            { "Floor_#", floorNum }, { "Player_ID", playerID } });
    }

    //Sends an analytics event when a level is rated
    public void LevelRated(int levelRating)
    {
        string mapName = LevelManager.Instance.currMapName;

        AnalyticsResult analyticsResult = Analytics.CustomEvent("Level_Rated",
            new Dictionary<string, object> { { "Map_Name", mapName },
            { "Rating", levelRating } });
    }

    //Sends an analytics event when the player dies
    public void PlayerDied()
    {
        string mapName = LevelManager.Instance.currMapName;
        int floorNum = LevelManager.Instance.currFloor;
        int playerID = PlayerPrefs.GetInt("UserID");

        AnalyticsResult analyticsResult = Analytics.CustomEvent("Player_Died",
            new Dictionary<string, object> { { "Map_Name", mapName },
            { "Floor_#", floorNum }, { "Player_ID", playerID } });
    }
}
