using UnityEngine;

public abstract class WeaponData : ItemData
{
    public enum WeaponType { Ranged, Melee }
    public WeaponType weaponType;

    public enum WieldStyle { OneHanded, TwoHanded, DualWield }
    public WieldStyle wieldStyle;

    public GameObject hitVisuals;
    public float damage;
}
