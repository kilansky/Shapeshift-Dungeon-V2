//Enum to keep track of flat and/or % modifiers to be added in
public enum StatModType
{
    Flat = 100,
    PercentAdd = 200,
    PercentMult = 300,
}

public class StatModifier
{
    //Variable Table of Context
    public readonly float Value; //This will contain the value of the statmodifier to be used
    public readonly StatModType Type; //Keeps track of the type of mod
    public int Order; //Variable to keep track of the order for modifiers to be applied
    public readonly object Source; //Variable to hold an object/type for where the modifier came from for debugging

    ///<summary>
    /// Alexander Lopez
    /// Updated: 2/9/21
    /// 
    /// Constructor to initialize the variables and keep track of mod types
    /// </summary>
    public StatModifier(float value, StatModType type, int order, object source)
    {
        Value = value;
        Type = type;
        Order = order; //Used to help create and adjust stat modifiers
        Source = source;
    }

    ///<summary>
    /// Alexander Lopez
    /// Updated: 2/9/21
    /// 
    /// Constructors for modifiers with/wihtout order and/or source
    /// </summary>
    public StatModifier(float value, StatModType type) : this(value, type, (int)type, null) { }
    public StatModifier(float value, StatModType type, int order) : this(value, type, order, null) { }
    public StatModifier(float value, StatModType type, object source) : this(value, type, (int)type, source) { }
}
