﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pit : MonoBehaviour
{
    /**
     * Script: Pit
     * Programmer: Justin Donato
     * Description: Moves the player back to a safe spot when they fall into a pit
     * Date Created: 2/22/2021
     * Date Last Edited: 2/22/2021
     **/
    public float pitDamage = 5f;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            FindSafeTile.Instance.MovePlayerToSafeLocation();

            if(LevelManager.Instance.currFloor != 0)//don't deal damage on level 0
                PlayerHealth.Instance.Damage(pitDamage);
        }
    }
}
