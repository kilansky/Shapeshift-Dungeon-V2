using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class AnalyticsEvents : MonoBehaviour
{
    //Sends an analytics event when an item is picked up by the player
    public void ItemTaken(string ItemName)
    {
        AnalyticsResult analyticsResult = Analytics.CustomEvent("test");
    }
}
