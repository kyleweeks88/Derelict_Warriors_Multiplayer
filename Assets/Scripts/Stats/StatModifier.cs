public enum StatModType
{
    Flat,
    PercentMulti
}

public class StatModifier
{
    public readonly float value;
    public readonly StatModType type;

    public StatModifier(float _value, StatModType _type)
    {
        value = _value;
        type = _type;
    }
}
