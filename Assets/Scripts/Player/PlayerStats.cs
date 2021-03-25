using System.Collections.Generic;
using System;
using System.Collections.ObjectModel;

[Serializable]
public class PlayerStats
{
    //Variable Table of Context
    public float BaseValue; //Value to keep track of the base value that the player will use for their adjustments
    protected readonly List<StatModifier> statModifiers; //List that will hold all the modifiers that will affect the player
    public readonly ReadOnlyCollection<StatModifier> StatModifiers;
    protected bool recalculate = true; //Variable to keep track of if the value needs to be recalculated or not
    protected float _value; //Variable to hold the most recent calculation that occurred to be used to set the final value for player stats
    protected float lastBaseValue = float.MinValue;

    ///<summary>
    /// Alexander Lopez
    /// Updated: 2/10/21
    /// 
    /// Variable to hold the calculated final value to be used to adjusst player stats based on if there has been a recent calulation to the modifiers/value
    /// Calculations only occur if any changes have been made
    /// </summary>
    public virtual float Value
    {
        get
        {
            if(recalculate || BaseValue != lastBaseValue)
            {
                lastBaseValue = BaseValue;
                _value = CalculateFinalValue();
                recalculate = false;
            }

            return _value;
        }
    }

    ///<summary>
    /// Alexander Lopez
    /// Updated: 2/10/21
    /// 
    /// Parameterless constructor to initialize statModifier lists
    /// </summary>
    public PlayerStats()
    {
        statModifiers = new List<StatModifier>();
        StatModifiers = statModifiers.AsReadOnly();
    }

    ///<summary>
    /// Alexander Lopez
    /// Updated: 2/10/21
    /// 
    /// Constructor to initalize the base value
    /// </summary>
    public PlayerStats(float baseValue) : this()
    {
        BaseValue = baseValue;
    }

    ///<summary>
    /// Alexander Lopez
    /// Updated: 2/10/21
    /// 
    /// Function to add modifiers to the statModifiers list and sets recalculate so the final value can be adjusted
    /// Sorts the modifiers by order type to make it easier to calculate later
    /// </summary>
    public virtual void AddModifiers(StatModifier mod)
    {
        recalculate = true;
        statModifiers.Add(mod);
        statModifiers.Sort(CompareModifierOrder);
    }

    ///<summary>
    /// Alexander Lopez
    /// Updated: 2/9/21
    /// 
    /// Comparison function to sort the stat modifiers by Order
    /// </summary>
    protected virtual int CompareModifierOrder(StatModifier a, StatModifier b)
    {
        if (a.Order < b.Order)
            return -1;
        else if (a.Order > b.Order)
            return 1;
        return 0; //a.Order == b.Order
    }

    ///<summary>
    /// Alexander Lopez
    /// Updated: 2/10/21
    /// 
    /// Function to remove modifiers from the statModifiers list and sets recalculate so the final value can be adjusted
    /// **AHL - If recalculate isn't working then put return false after the if and return true in the if.
    /// </summary>
    public virtual bool RemoveModifiers(StatModifier mod)
    {
        if(statModifiers.Remove(mod))
        {
            recalculate = true;
        }

        return recalculate;
    }

    ///<summary>
    /// Alexander Lopez
    /// Updated: 2/10/21
    /// 
    /// Function to remove modifiers from a specific source
    /// </summary>
    public virtual bool RemoveAllModifiersFromSource(object source)
    {
        bool didRemove = false;
        
        //For loop to go through all modifiers until it finds the right source
        for(int i = statModifiers.Count - 1; i >= 0; i--) //Best to start from the back of the list to remove in the specific order
        {
            if (statModifiers[i].Source == source)
            {
                recalculate = true;
                didRemove = true;
                statModifiers.RemoveAt(i);
            }
        }

        return didRemove;
    }

    ///<summary>
    /// Alexander Lopez
    /// Updated: 2/10/21
    /// 
    /// Function to calculate the final value needed to adjust character stats
    /// </summary>
    protected virtual float CalculateFinalValue()
    {
        float finalValue = BaseValue; //Set final value to base as it is the starting point
        float sumPercentAdd = 0; //Variable for percents that are added together

        //For-loop to go through the list of statModifiers and add them to the final value
        for(int i = 0; i < statModifiers.Count; i++)
        {
            StatModifier mod = statModifiers[i];

            //If the modifier is a flat type
            if(mod.Type == StatModType.Flat)
            {
                finalValue += statModifiers[i].Value;
            }

            //If the modifier is %add type
            if(mod.Type == StatModType.PercentAdd)
            {
                sumPercentAdd += mod.Value;

                //Iterate through the list and add modifiers of the same type until we reach a different type or end of the list
                if(i + 1 >= statModifiers.Count || statModifiers[i+1].Type != StatModType.PercentAdd)
                {
                    finalValue *= 1 + sumPercentAdd;
                    sumPercentAdd = 0;
                }
            }
            
            //If the modifier is a %mult type
            else if(mod.Type == StatModType.PercentMult)
            {
                //Uses a decimal number as the value and adds 1 (100%) to it to adjust the final value number
                finalValue *= 1 + mod.Value;
            }
        }

        //Return the finalValue rounded to avoid stat calculation errors
        //**4 points after the decimal**
        return (float)Math.Round(finalValue, 4);
    }
}
