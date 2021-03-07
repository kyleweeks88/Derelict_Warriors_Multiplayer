using UnityEngine;

[CreateAssetMenu(fileName = "ArmorData", menuName = "ItemData/ArmorData")]
public class ArmorData : ItemData
{
    public enum EquipSlot { Head, Chest, Legs, }
    public EquipSlot equipSlot;
}
