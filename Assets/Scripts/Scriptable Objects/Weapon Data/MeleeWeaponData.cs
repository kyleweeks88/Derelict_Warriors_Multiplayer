using UnityEngine;

[CreateAssetMenu(fileName = "MeleeWeaponData", menuName = "ItemData/WeaponData/MeleeWeaponData")]
public class MeleeWeaponData : WeaponData
{
    public enum DamageType { Blunt, Slash, Pierce }

    [Header("Melee Settings")]
    public DamageType damageType;
    public float staminaCost;
}
