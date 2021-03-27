using System.Collections.Generic;
using System;

[Serializable]
public class Stat
{
    public float baseValue;
    public float maxValue;

    public float value 
    {
        get 
        {
            if(isDirty)
            {
                currentValue = CalculateFinalValue();
                isDirty = false;
            }
            return currentValue;
        }
    }

    bool isDirty = true;
    float currentValue;

    private readonly List<StatModifier> statModifiers;

    public Stat(float _baseValue, float _maxValue)
    {
        baseValue = _baseValue;
        maxValue = _maxValue;
        statModifiers = new List<StatModifier>();
    }

    public void AddModifer(StatModifier mod)
    {
        isDirty = true;
        statModifiers.Add(mod);
    }

    public bool RemoveModifier(StatModifier mod)
    {
        isDirty = true;
        return statModifiers.Remove(mod);
    }

    float CalculateFinalValue()
    {
        float finalValue = baseValue;

        for (int i = 0; i < statModifiers.Count; i++)
        {
            finalValue += statModifiers[i].value;
        }

        // rounds 12.0001f to 12f
        return (float)Math.Round(finalValue, 4);
    }
}
