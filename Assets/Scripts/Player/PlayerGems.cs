using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGems : SingletonPattern<PlayerGems>
{
    private int gemCount;
    public int GemCount { get { return gemCount; } }

    void Start()
    {
        gemCount = 0;
        HUDController.Instance.UpdateGemCount(GemCount);
    }

    //Increases gemCount by the given amt
    public void AddGems(int amt)
    {
        gemCount += amt;
        HUDController.Instance.UpdateGemCount(GemCount);
    }

    //Subtracts gemCount by the given amt
    public void SubtractGems(int amt)
    {
        gemCount -= amt;
        HUDController.Instance.UpdateGemCount(GemCount);
    }
}
