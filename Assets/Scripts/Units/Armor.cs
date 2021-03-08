using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor : MonoBehaviour, IDamageable<float>
{
    public ArmorData armorData;

    public float durability;
    float currentDurability;

    void Start()
    {
        currentDurability = durability;
    }

    public void TakeDamage(float dmgVal)
    {
        durability += dmgVal;

        if(durability <= 0)
        {
            DestroyArmor();   
        }
    }

    void DestroyArmor()
    {
        // DESTROY THIS OBJECT AND CLEAR IT FROM EquipmentManager
        // DELETE ANY MODIFIERS TO CHARACTER
    }
}
