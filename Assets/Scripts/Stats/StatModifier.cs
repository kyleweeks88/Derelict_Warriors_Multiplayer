public enum StatModType
{
    Flat, // order = 0
    PercentAdd, // order = 1 etc...
    PercentMulti, 
}

public class StatModifier
{
    public readonly StatModType type;
    public readonly float value;
    public readonly int order;

    // This constructor requires the user to input all three parameters
    public StatModifier(float _value, StatModType _type, int _order)
    {
        value = _value;
        type = _type;
        order = _order;
    }

    // This constructor only requires the user to input _value and _type.
    // However this constructor will also automatically call the constructor above
    // passing in the _type, as the _order - determined by
    // the int value it holds in the enum StatModType.
    public StatModifier(float _value, StatModType _type) : this(_value, _type, (int)_type) { }
}
