using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTracker : SingletonPattern<DamageTracker>
{
    //Variable List
    private float spikeDamage = 0, lavaDamage = 0, dispenserTrapDamage = 0, skullDamage = 0, crystalDamage = 0, bombDamage = 0, pitfallDamage = 0, fireStatusDamage = 0; //The different damage types to track the amount of damage taken on the player

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
    /// Updates the damage float values with the amount of damage that the player took based on the game object source - AHL (4/4/21)
    /// </summary>
    public void updateDamage(float damageValue, GameObject damageSource)
    {
        //Long chain of ifs to adjust the daamge variables listed above
        
        //Spike Trap
        if(damageSource.GetComponent<SpikeTrap>())
        {
            spikeDamage += damageValue;
        }

        //Lava
        else if (damageSource.GetComponent<LavaTile>())
        {
            lavaDamage += damageValue;
        }

        //Dispenser
        else if (damageSource.GetComponent<Dispenser>())
        {
            dispenserTrapDamage += damageValue;
        }

        //Skull
        else if (damageSource.GetComponent<FloatingSkull>())
        {
            skullDamage += damageValue;

            //If the player is wearing the Flame Crown then damage the enemy for 3 seconds
            if (PlayerController.Instance.HeadSlot != null && PlayerController.Instance.HeadSlot.ItemName == "Flame Crown")
                damageSource.GetComponent<StatusEffects>().fireStatus(3);
        }

        //Crystal
        else if (damageSource.GetComponent<FloatingCrystal>())
        {
            crystalDamage += damageValue;

            //If the player is wearing the Flame Crown then damage the enemy for 3 seconds
            if (PlayerController.Instance.HeadSlot != null && PlayerController.Instance.HeadSlot.ItemName == "Flame Crown")
                damageSource.GetComponent<StatusEffects>().fireStatus(3);
        }

        //Bomb
        else if (damageSource.GetComponent<BombAttack>())
        {
            bombDamage += damageValue;
        }

        //Pitfalls
        else if(damageSource.GetComponent<Pit>())
        {
            pitfallDamage += damageValue;
        }

        //Fire Status Effect
        else if (damageSource.GetComponent<PlayerController>())
        {
            fireStatusDamage += damageValue;
        }
    }

    /// <summary>
    /// When the player dies then we display the total amount of damage the player has taken - AHL (4/4/21)
    /// </summary>
    public void displayDamage()
    {
        print("These are the damage sources and amount of damage that the player has taken throughout the test:\n" +
            "The Amount of Spike damage the player took was: " + spikeDamage + 
            "\nThe Amount of Lava damage the player took was: " + lavaDamage +
            "\nThe Amount of Dispenser damage the player took was: " + dispenserTrapDamage +
            "\nThe Amount of Pit Fall damage the player took was: " + pitfallDamage +
            "\nThe Amount of Skull damage the player took was: " + skullDamage +
            "\nThe Amount of Crystal damage the player took was: " + crystalDamage +
            "\nThe Amount of Bomb damage the player took was: " + bombDamage + 
            "\nThe Amount of Fire Status damage the player took was: " + fireStatusDamage);
    }
}
