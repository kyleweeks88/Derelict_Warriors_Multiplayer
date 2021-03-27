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
        // This takes in the passed mod and compares it to an existing mod in the List
        // then determines it's position by the StatModifier.order
        statModifiers.Sort(CompareModifierOrder);
    }

    public bool RemoveModifier(StatModifier mod)
    {
        isDirty = true;
        return statModifiers.Remove(mod);
    }

    // This will be used to compare the order and sort the modifiers correctly
    int CompareModifierOrder(StatModifier a, StatModifier b)
    {
        if (a.order < b.order)
            return -1;
        else if (a.order > b.order)
            return 1;

        return 0; // if (a.order == b.order)
    }

    float CalculateFinalValue()
    {
        float finalValue = baseValue;

        for (int i = 0; i < statModifiers.Count; i++)
        {
            StatModifier mod = statModifiers[i];

            if(mod.type == StatModType.Flat)
            {
                finalValue += mod.value;
            }
            else if(mod.type == StatModType.PercentMulti)
            {
                finalValue *= 1 + mod.value;
            }
        }

        // rounds 12.0001f to 12f
        return (float)Math.Round(finalValue, 4);
    }
}
