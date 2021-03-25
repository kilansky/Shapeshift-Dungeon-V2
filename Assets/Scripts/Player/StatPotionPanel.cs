using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatPotionPanel : MonoBehaviour
{
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

        //PlayerController.Instance.baseAttackDamage.BaseValue * 0.15f; //We assign a variable to hold the percent increase needed for the variable (25% of the current base value)
        float percentIncrease = 0.45f; //15% of the starting attack damage (3)
        PlayerController.Instance.baseAttackDamage.BaseValue += percentIncrease; //Increases the current base value by the percent increase
        PlayerController.Instance.StatAttackCount++;

        HUDController.Instance.HideStatPotionPanel();
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
}
