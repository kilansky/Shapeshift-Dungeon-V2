using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatPotionPanel : MonoBehaviour
{
    //Variable to keep track of the percent increase value to be displayed on the UI panel
    [HideInInspector] public float percentIncrease = 0;

    public void IncreaseMaxHealth()
    {
        Debug.Log("Increase Max Health");
        PlayerHealth.Instance.IncreaseMaxHealth(5);
        PlayerController.Instance.StatMaxHealthCount++;
        HUDController.Instance.HideStatPotionPanel();
    }

    public void IncreaseAttack()
    {
        Debug.Log("Increase Attack");

        PlayerController.Instance.StatAttackCount++; //Increases the statAttackCount by 1 as the attack option has been selected

        //Sets the percent increase to the Attack Base value * 15% (with a decreasing percentage by .1) * sqrt times choosen
        //0.151 is the percent as we start at 0 so when we take the first attack increase it will be the desired initial 15%
        percentIncrease = PlayerController.Instance.baseAttackDamage.BaseValue * (0.151f - (PlayerController.Instance.StatAttackCount / 10)) * (1f / Mathf.Sqrt(PlayerController.Instance.StatAttackCount));

        //Increases the current base value by the percent increase
        PlayerController.Instance.baseAttackDamage.BaseValue += percentIncrease;


        HUDController.Instance.HideStatPotionPanel(); //Hides the stat potion panel



        //(float)Math.Round(finalValue, 1); Rounds the decimal to 1 decimal place

        /*
        //PlayerController.Instance.baseAttackDamage.BaseValue * 0.15f; //We assign a variable to hold the percent increase needed for the variable (25% of the current base value)
        float percentIncrease = 0.45f; //15% of the starting attack damage (3)
        PlayerController.Instance.baseAttackDamage.BaseValue += percentIncrease; //Increases the current base value by the percent increase
        PlayerController.Instance.StatAttackCount++;

        */
    }

    public void IncreaseSpeed()
    {
        Debug.Log("Increase Speed");

        float percentIncrease = 0.6f; //5% of the starting move speed (12)
        //PlayerController.Instance.baseMoveSpeed.BaseValue * 0.05f; // We assign a variable to hold the percent increase needed for Move Speed (5% of the current base value)
        PlayerController.Instance.baseMoveSpeed.BaseValue += percentIncrease; //Increases the Move Speed base value by the percent increase

        percentIncrease = 1.4f; //5% of the starting dash speed (28)
        //percentIncrease = PlayerController.Instance.dashSpeed.BaseValue * 0.05f; // We assign a variable to hold the percent increase needed for Dash Speed (5% of the current base value)
        PlayerController.Instance.dashSpeed.BaseValue += percentIncrease; //Increases the Dash Speed base value by the percent increase
        PlayerController.Instance.StatSpeedCount++;

        HUDController.Instance.HideStatPotionPanel();
    }

    /// <summary>
    /// Function to get the math for the percent increase addition to be displayed to the player - AHL (3/26/21)
    /// **Possible funciton and would need adjustments to fit the UI panel**
    /// </summary>
    public void displayPercentIncrease()
    {
        //Attack Stat
        


        //Speed Stat

    }

}
