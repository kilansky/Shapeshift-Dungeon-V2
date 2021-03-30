﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTracker : SingletonPattern<DamageTracker>
{
    //Variable List
    private float spikeDamage = 0, lavaDamage = 0, skullDamage = 0, crystalDamage = 0, bombDamage = 0, pitfallDamage = 0, fireStatusDamage = 0; //The different damage types to track the amount of damage taken on the player

    /*
    //Enums
    /// <summary>
    /// Choice of what damage source the player took - AHL (3/30/21)
    /// </summary>
    public enum damageSourceTypes
    {
        SpikeDamageSource = 0,
        LavaDamageSource = 1,
        PitfallDamageSource = 2,
        SkullDamageSource = 3,
        CrystalDamageSource = 4,
        BombDamageSource = 5,
        FireStatusDamageSource = 6,
    }*/

    /// <summary>
    /// Updates the damage float values with the amount of damage that the player took based on the game object source - AHL (3/30/21)
    /// </summary>
    public void updateDamage(float damageValue, GameObject damageSource)
    {
        //Long chain of ifs to adjust the daamge variables listed above
        
        //Spike Trap
        if(damageSource.GetComponent<SpikeTrap>())
        {
            spikeDamage += damageValue;
        }

        //Pitfalls
        else if(damageSource.GetComponent<Pit>())
        {
            pitfallDamage += damageValue;
        }
    }

    /// <summary>
    /// When the player dies then we display the total amount of damage the player has taken - AHL (3/30/21)
    /// </summary>
    public void displayDamage()
    {
        print("These are the damage sources and amount of damage that the player has taken throughout the test:\n" +
            "The Amount of Spike damage the player took was: " + spikeDamage + 
            "\nThe Amount of Lava damage the player took was: " + lavaDamage +
            "\nThe Amount of Pit Fall damage the player took was: " + pitfallDamage +
            "\nThe Amount of Skull damage the player took was: " + skullDamage +
            "\nThe Amount of Crystal damage the player took was: " + crystalDamage +
            "\nThe Amount of Bomb damage the player took was: " + bombDamage + 
            "\nThe Amount of Fire Status damage the player took was: " + fireStatusDamage);
    }
}
