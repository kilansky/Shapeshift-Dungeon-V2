using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStartInfo : SingletonPattern<GameStartInfo>
{
    public int startingFloor;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
}