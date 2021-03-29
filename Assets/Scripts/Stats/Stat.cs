using System.Collections.Generic;
using System;
using System.Collections.ObjectModel;

[Serializable]
public class Stat
{
    public float baseValue;

    public virtual float value 
    {
        get 
        {
            if(isDirty || baseValue != lastBaseValue)
            {
                lastBaseValue = baseValue;
                currentValue = CalculateFinalValue();
                isDirty = false;
            }
            return currentValue;
        }
    }

    protected bool isDirty = true;
    protected float currentValue;
    protected float lastBaseValue = float.MinValue;

    protected readonly List<StatModifier> statModifiers;
    public readonly ReadOnlyCollection<StatModifier> StatModifiers;

    public Stat()
    {
        statModifiers = new List<StatModifier>();
        StatModifiers = statModifiers.AsReadOnly();
    }

    public Stat(float _baseValue) : this()
    {
        baseValue = _baseValue;
    }

    /// <summary>
    /// Add the passed StatModifier to affect the specified Stat.
    /// </summary>
    /// <param name="mod"></param>
    public virtual void AddModifer(StatModifier mod)
    {
//========>SHOULD I CHECK IF THE LIST ALREADY CONTAINS THE PASSED mod THOUGH???
//========>MIGHT CHANGE THIS LATER.
        if (!statModifiers.Contains(mod))
        {
            isDirty = true;
            statModifiers.Add(mod);
            // This takes in the passed mod and compares it to an existing mod in the List
            // then determines it's position by the StatModifier.order
            statModifiers.Sort(CompareModifierOrder);
        }
    }

    /// <summary>
    /// Removes the specified StatModifier from the affected Stat.
    /// </summary>
    /// <param name="mod"></param>
    /// <returns></returns>
    public virtual bool RemoveModifier(StatModifier mod)
    {
//========>SHOULD I CHECK IF THE LIST ALREADY CONTAINS THE PASSED mod THOUGH???
//========>MIGHT CHANGE THIS LATER.
        if (statModifiers.Contains(mod))
        {
            if (statModifiers.Remove(mod))
            {
                isDirty = true;
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Removes all StatModifiers affecting the Stat, passed by the _source parameter.
    /// </summary>
    /// <param name="_source"></param>
    /// <returns></returns>
    public virtual bool RemoveAllModifiersFromSource(object _source)
    {
        bool didRemove = false;

        // We loop through this list backwards to save the headache of shuffling objects
        // in list up or down in index as we remove the mods from the source.
        for (int i = statModifiers.Count - 1; i >= 0; i--)
        {
            // if the mod.source is equal to the _source passed in parameter...
            if(statModifiers[i].source == _source)
            {
                // isDirty tells us that the Stat has been modified and to recalculate.
                isDirty = true;
                // turn this bool true to signify that a mod.source matched the _source
                // and was removed appropriately.
                didRemove = true;
                // removes the mod at the correct index in the statModifiers list.
                statModifiers.RemoveAt(i);
            }
        }

        // return didRemove true or false based on what happened in the for loop.
        return didRemove;
    }

    // This will be used to compare the order and sort the modifiers correctly
    protected virtual int CompareModifierOrder(StatModifier a, StatModifier b)
    {
        if (a.order < b.order)
            return -1;
        else if (a.order > b.order)
            return 1;

        return 0; // if (a.order == b.order)
    }

    // This function will calculate all current affecting StatModifiers in
    // the statModifiers list and return the final value.
    // It is called when we attempt to get the value of this Stat -
    // but only if the Stat has been modified. Signified by the isDirty bool.
    protected virtual float CalculateFinalValue()
    {
        float finalValue = baseValue;
        float sumPercentAdd = 0;

        for (int i = 0; i < statModifiers.Count; i++)
        {
            StatModifier mod = statModifiers[i];

            if(mod.type == StatModType.Flat)
            {
                // flat types are added
                finalValue += mod.value;
            }
            else if(mod.type == StatModType.PercentAdd)
            {
                // We don't apply this mod value to the finalValue yet.
                // Instead we add it to the sumPercentAdd variable, to be added to
                // finalValue once we've calculated all other PercentAdd types.
                sumPercentAdd += mod.value;

                // until we reach the end of the list. OR until we reach a mod of a different type.
                if(i + 1 >= statModifiers.Count || statModifiers[i + 1].type != StatModType.PercentAdd)
                {
                    // Add the total calculated sumPercentAdd to finalValue
                    finalValue *= 1 + sumPercentAdd;
                    // and clear sumPercentAdd for further calculations
                    sumPercentAdd = 0;
                }
            }
            else if(mod.type == StatModType.PercentMulti)
            {
                // percent types are multiplied
                finalValue *= 1 + mod.value;
            }
        }

        // rounds 12.0001f to 12f
        return (float)Math.Round(finalValue, 4);
    }
}
