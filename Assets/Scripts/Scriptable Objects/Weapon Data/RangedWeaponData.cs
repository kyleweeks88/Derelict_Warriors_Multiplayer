using UnityEngine;

[CreateAssetMenu(fileName = "RangedWeaponData", menuName = "ItemData/WeaponData/RangedWeaponData")]
public class RangedWeaponData : WeaponData
{
    [Header("Ranged Specific")]
    public GameObject projectile_Pf;
}
