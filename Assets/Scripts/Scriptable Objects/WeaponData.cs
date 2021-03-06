using UnityEngine;

public abstract class WeaponData : ItemData
{
    public enum WieldStyle { OneHanded, TwoHanded }
    public WieldStyle wieldStyle;
    public float damage;
}
