using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTracker : SingletonPattern<DamageTracker>
{
    //Variable List
    [HideInInspector] public float spikeDamage = 0, lavaDamage = 0, skullDamage = 0, bombDamage = 0, pitfallDamage = 0; //The different damage types to track the amount of damage taken on the player
    
    //Enums

    /// <summary>
    /// When the player dies then we display the total amount of damage the player has taken - AHL (3/23/21)
    /// </summary>
    public void displayDamage()
    {
        print("These are the damage sources and amount of damage that the player has taken throughout the test:");
        print("The Amount of Spike damage the player took was: " + spikeDamage);
        print("The Amount of Lava damage the player took was: " + lavaDamage);
        print("The Amount of Skull damage the player took was: " + skullDamage);
        print("The Amount of Bomb damage the player took was: " + bombDamage);
        print("The Amount of Pit Fall damage the player took was: " + pitfallDamage);
    }
}
