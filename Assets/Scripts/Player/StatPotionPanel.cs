using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

//Last Modified 4/24/21 by Sky - Partially Implemented Attack Speed Stat

public class StatPotionPanel : MonoBehaviour
{
    //Variable to keep track of the percent increase value to be displayed on the UI panel
    [HideInInspector] public float attackIncreaseValue = 0;
    [HideInInspector] public float attackSpeedIncreaseValue = 0;
    [HideInInspector] public float moveSpeedIncreaseValue = 0;
    [HideInInspector] public float dashIncreaseValue = 0;

    private float attackPercentIncrease = 0.15f; //Starting Attack % increase
    private float attackSpeedPercentIncrease = 0.10f; //Starting Attack % increase
    private float speedPercentIncrease = 0.075f; //Starting Speed % increase

    private float originalAttackBaseValue; //Variable to keep track of what the original attack of the player is when the game starts
    private float originalAttackSpeedBaseValue; //Variable to keep track of what the original attack speed of the player is when the game starts
    private float originalMoveSpeedBaseValue; //Variable to keep track of what the original move speed of the player is when the game starts
    private float originalDashSpeedBaseValue; //Variable to keep track of what the original dash speed of the player is when the game starts

    public TMP_Text attackText; //Variable to hold the attack text in the inspector for the stat potion panel
    public TMP_Text speedText; //Variable to hold the speed text in the inspector for the stat potion panel

    private PlayerController pc;

    private void Awake()
    {
        pc = PlayerController.Instance;

        originalAttackBaseValue = pc.baseAttackDamage.BaseValue; //Sets the original attack variable to the starting attack base value
        originalAttackSpeedBaseValue = pc.baseAttackSpeed.BaseValue; //Sets the original attack variable to the starting attack base value
        originalMoveSpeedBaseValue = pc.baseMoveSpeed.BaseValue; //Sets the original move speed variable to the starting move speed base value
        originalDashSpeedBaseValue = pc.dashSpeed.BaseValue; //Sets the original dash speed variable to the starting dash speed base value
    }

    private void Update()
    {
        //Attack Stat
        //Math that is done to increase the stat at a decreasing rate, currently set to 1 / a quad root of the amount of times attack has increased.
        float MathCalculation = 1f / Mathf.Pow((pc.StatAttackCount + 1), 1 / 4f);

        //Sets the increase value to the Attack Base value * attack % increase * math
        float IncreaseValue = originalAttackBaseValue * attackPercentIncrease * MathCalculation;

        //Sets an increase percent value to be used for the debug log later and for displaying proper information for the UI
        float IncreasePercent = (((originalAttackBaseValue + IncreaseValue) / originalAttackBaseValue) - 1) * 100;

        //Math rounding the increase percents to 1 decimal place to appear on the panel by adjusting the text
        attackText.text = "Attack Power +" + (float)Math.Round(IncreasePercent, 1) + "%";

        //Speed Stat
        //Math that is done to increase the stat at a decreasing rate, currently set to 1 / a quad root of the amount of times attack has increased.
        MathCalculation = 1f / Mathf.Pow((pc.StatSpeedCount + 1), 1 / 4f);

        //Sets the increase value to the Attack Base value * attack % increase * math
        IncreaseValue = originalAttackSpeedBaseValue * attackSpeedPercentIncrease * MathCalculation;

        //Sets an increase percent value to be used for the debug log later and for displaying proper information for the UI
        IncreasePercent = (((originalAttackSpeedBaseValue + IncreaseValue) / originalAttackSpeedBaseValue) - 1) * 100;

        //Math rounding the increase percents to 1 decimal place to appear on the panel by adjusting the text
        speedText.text = "Move & Attack" + "\n" + "Speed +" + (float)Math.Round(IncreasePercent, 1) + "%";
    }

    public void IncreaseMaxHealth()
    {
        PlayerHealth.Instance.IncreaseMaxHealth(5);
        pc.StatMaxHealthCount++;

        HUDController.Instance.HideStatPotionPanel();

        AnalyticsEvents.Instance.StatUpgraded("Health"); //Send Stat Upgraded Analytics Event
        AudioManager.Instance.Play("Potion");
    }

    public void IncreaseAttack()
    {
        //Debug.Log("Increase Attack");

        //Keeps track of what the attack starting value was for the debug Log later on
        float startingValue = pc.baseAttackDamage.BaseValue;

        //Increases the statAttackCount by 1 as the attack option has been selected
        pc.StatAttackCount++; 

        //Math that is done to increase the stat at a decreasing rate, currently set to 1 / a quad root of the amount of times attack has increased.
        float MathCalculation = 1f / Mathf.Pow(pc.StatAttackCount,1/4f); 

        //Sets the increase value to the Attack Base value * attack % increase * math
        attackIncreaseValue = originalAttackBaseValue * attackPercentIncrease * MathCalculation;

        //Increases the current base value by the increase value
        pc.baseAttackDamage.BaseValue += attackIncreaseValue;

        //Sets an increase percent value to be used for the debug log later and for displaying proper information for the UI
        float increasePercent = ((originalAttackBaseValue + attackIncreaseValue) / originalAttackBaseValue) - 1;

        HUDController.Instance.HideStatPotionPanel(); //Hides the stat potion panel

        AnalyticsEvents.Instance.StatUpgraded("Attack"); //Send Stat Upgraded Analytics Event

        //Debug Log to show all the math as check for Skys math table equations
        /*Debug.Log("Starting Value: " + startingValue + "\n" 
            + "Percent Increase: " + attackPercentIncrease * 100 + "\n" 
            + "Math Calculation: " + MathCalculation + "\n" 
            + "Increase Value: " + attackIncreaseValue + "\n"
            + "Final Value: " + pc.baseAttackDamage.BaseValue + "\n"
            + "Increase %: " + increasePercent * 100 + "%");
            */

        //Decrease the % increase by 0.1%
        attackPercentIncrease -= 0.001f;
        AudioManager.Instance.Play("Potion");
    }

    public void IncreaseSpeed()
    {
        //Debug.Log("Increase Speed");

        //Keeps track of what the move speed and dash speed starting value was for the debug Log later on
        float moveSpeedStartingValue = pc.baseMoveSpeed.BaseValue;
        float attackSpeedStartingValue = pc.baseAttackSpeed.BaseValue;
        float dashStartingValue = pc.dashSpeed.BaseValue;

        //Increases the statSpeedCount by 1 as the speed option has been selected
        pc.StatSpeedCount++;

        //Math that is done to increase the stat at a decreasing rate, currently set to 1 / a quad root of the amount of times speed has increased.
        float MathCalculation = 1f / Mathf.Pow(pc.StatSpeedCount, 1 / 4f);

        //Sets the increase values to the move and dashspeed Base value * speed % increase * math
        moveSpeedIncreaseValue = originalMoveSpeedBaseValue * speedPercentIncrease * MathCalculation;
        attackSpeedIncreaseValue = originalAttackSpeedBaseValue * attackSpeedPercentIncrease * MathCalculation;
        dashIncreaseValue = originalDashSpeedBaseValue * speedPercentIncrease * MathCalculation;

        //Increases the current base values by their representative increase values
        pc.baseMoveSpeed.BaseValue += moveSpeedIncreaseValue;
        pc.baseAttackSpeed.BaseValue += attackSpeedIncreaseValue;
        pc.dashSpeed.BaseValue += dashIncreaseValue;
        pc.SetAttackSpeed();

        //Sets the increase percent values to be used for the debug log later and for displaying proper information for the UI
        float moveSpeedIncreasePercent = ((originalMoveSpeedBaseValue + moveSpeedIncreaseValue) / originalMoveSpeedBaseValue) - 1;
        float attackSpeedIncreasePercent = ((originalAttackSpeedBaseValue + attackSpeedIncreaseValue) / originalAttackSpeedBaseValue) - 1;
        float dashIncreasePercent = ((originalDashSpeedBaseValue + dashIncreaseValue) / originalDashSpeedBaseValue) - 1;

        //Debug Log to show all the math as check for Skys math table equations
        /*
        Debug.Log("Move Speed Math\n"
            + "Starting Value: " + moveSpeedStartingValue + "\n"
            + "Percent Increase: " + speedPercentIncrease * 100 + "\n"
            + "Math Calculation: " + MathCalculation + "\n"
            + "Increase Value: " + moveSpeedIncreaseValue + "\n"
            + "Final Value: " + pc.baseMoveSpeed.BaseValue + "\n"
            + "Increase %: " + moveSpeedIncreasePercent * 100 + "%");

        Debug.Log("Attack Speed Math\n"
            + "Starting Value: " + attackSpeedStartingValue + "\n"
            + "Percent Increase: " + speedPercentIncrease * 100 + "\n"
            + "Math Calculation: " + MathCalculation + "\n"
            + "Increase Value: " + attackSpeedIncreaseValue + "\n"
            + "Final Value: " + pc.baseAttackSpeed.BaseValue + "\n"
            + "Increase %: " + attackSpeedIncreasePercent * 100 + "%");

        Debug.Log("Dash Speed Math\n"
            + "Starting Value: " + dashStartingValue + "\n"
            + "Percent Increase: " + speedPercentIncrease * 100 + "\n"
            + "Math Calculation: " + MathCalculation + "\n"
            + "Increase Value: " + dashIncreaseValue + "\n"
            + "Final Value: " + pc.dashSpeed.BaseValue + "\n"
            + "Increase %: " + dashIncreasePercent * 100 + "%");
            */

        //Decrease the % increase by 0.1%
        speedPercentIncrease -= 0.001f;

        HUDController.Instance.HideStatPotionPanel(); //Hides the stat potion panel

        AnalyticsEvents.Instance.StatUpgraded("Speed"); //Send Stat Upgraded Analytics Event
        AudioManager.Instance.Play("Potion");
    }
}
