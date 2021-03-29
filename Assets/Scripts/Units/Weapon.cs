using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public StatModType type;
    public WeaponData weaponData;
    public StatModifier damageModifier;
    public float maxCharge = 2f;
    public float chargeRate = 5f;
    [HideInInspector] public float currentCharge = 1f;

    public void ResetCharge()
    {
        currentCharge = 1f;
    }
}

