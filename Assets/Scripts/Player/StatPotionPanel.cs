using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatPotionPanel : MonoBehaviour
{
    //Variable to keep track of the percent increase value to be displayed on the UI panel
    [HideInInspector] public float increaseValue = 0;

    private float attackPercentIncrease = 0.15f;

    private float originalAttackBaseValue;

    private void Start()
    {
        originalAttackBaseValue = PlayerController.Instance.baseAttackDamage.BaseValue;
    }

    public void IncreaseMaxHealth()
    {
        PlayerHealth.Instance.IncreaseMaxHealth(5);
        PlayerController.Instance.StatMaxHealthCount++;

        HUDController.Instance.HideStatPotionPanel();

        AnalyticsEvents.Instance.StatUpgraded("Health"); //Send Stat Upgraded Analytics Event
    }

    public void IncreaseAttack()
    {
        Debug.Log("Increase Attack");

        float startingValue = PlayerController.Instance.baseAttackDamage.BaseValue;

        PlayerController.Instance.StatAttackCount++; //Increases the statAttackCount by 1 as the attack option has been selected

        float MathCalculation = 1f / Mathf.Pow(PlayerController.Instance.StatAttackCount,1/4f);

        //Sets the percent increase to the Attack Base value * 15% (with a decreasing percentage by .1) * sqrt times choosen
        //0.151 is the percent as we start at 0 so when we take the first attack increase it will be the desired initial 15%
        float increaseValue = originalAttackBaseValue * attackPercentIncrease * MathCalculation;

        //Increases the current base value by the percent increase
        PlayerController.Instance.baseAttackDamage.BaseValue += increaseValue;

        float increasePercent = ((originalAttackBaseValue + increaseValue) / originalAttackBaseValue) - 1;

        //float percentIncrease = (PlayerController.Instance.baseAttackDamage.BaseValue - startingValue); 

        HUDController.Instance.HideStatPotionPanel(); //Hides the stat potion panel

        AnalyticsEvents.Instance.StatUpgraded("Attack"); //Send Stat Upgraded Analytics Event

        Debug.Log("Starting Value: " + startingValue + "\n" 
            + "Percent Increase: " + attackPercentIncrease * 100 + "\n" 
            + "Math Calculation: " + MathCalculation + "\n" 
            + "Increase Value: " + increaseValue + "\n"
            + "Final Value: " + PlayerController.Instance.baseAttackDamage.BaseValue + "\n"
            + "Increase %: " + increasePercent * 100+ "%");

        attackPercentIncrease -= 0.001f;

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

        AnalyticsEvents.Instance.StatUpgraded("Speed"); //Send Stat Upgraded Analytics Event
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
