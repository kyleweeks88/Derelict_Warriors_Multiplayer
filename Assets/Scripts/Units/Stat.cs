using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stat
{
    CharacterStats myStats;
    public string statName = string.Empty;

    [SerializeField] float currentValue;
    float maxValue;

    //public Stat(float value)
    //{
    //    this.maxValue = value;
    //    currentValue = maxValue;
    //}

    public void InitializeStat(CharacterStats _stats, float value)
    {
        this.maxValue = value;
        currentValue = maxValue;

        myStats = _stats;
    }

    public float GetCurrentValue()
    {
        return currentValue;
    }

    public float GetMaxValue()
    {
        return maxValue;
    }

    public float ModifyStat(float modValue)
    {
        float newValue = currentValue += modValue;

        return newValue;
    }
}
