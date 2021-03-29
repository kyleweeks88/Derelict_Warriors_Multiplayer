public enum StatModType
{
    // turning these index to 100, 200 etc. makes it easier to 
    // customize the way we implement the "order" they are calculated.
    Flat = 100,
    PercentAdd = 200,
    PercentMulti = 300, 
}

public class StatModifier
{
    // type can determine the order in which this modifier is processed.
    // also how it will be calculated when modifying the affected Stat.
    public readonly StatModType type;
    // value is amount to modify the affected Stat by
    public readonly float value;
    // order is used to determine the order, this modifier will be
    // processed by the affected Stat
    public readonly int order;
    // source can hold any type as a source. ex: Class, int, float, GameObject...
    // Useful for being able to tell where each modifier is sent from.
    public readonly object source;

    // This main constructor requires all three parameters
    public StatModifier(float _value, StatModType _type, int _order, object _source)
    {
        value = _value;
        type = _type;
        order = _order;
        source = _source;
    }

    // This constructor only requires the user to input _value and _type.
    // However this constructor will also automatically call the constructor above
    // passing in the _type, as the _order - determined by
    // the int value it holds in the enum StatModType.
    public StatModifier(float _value, StatModType _type) : this(_value, _type, (int)_type, null) { }

    // this constructor allows input of a custom _order and leaves _source as null.
    public StatModifier(float _value, StatModType _type, int _order) : this(_value, _type, _order, null) { }

    // this constructor takes in a _source and leaves _order as the default value of it's enum index
    public StatModifier(float _value, StatModType _type, object _source) : this(_value, _type, (int)_type, _source) { }
}
