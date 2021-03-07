using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    public Weapon currentlyEquippedWeapon;

    [SerializeField] Transform weaponEquipPos;

    public void EquipWeapon(Weapon weaponToEquip)
    {
        //Weapon newWeapon = weaponToEquip.GetComponent<Weapon>();
        if(currentlyEquippedWeapon != null)
        {
            // UNEQUIP WEAPON LOGIC
        }
        else
        {
            Weapon newWeapon = Instantiate(weaponToEquip, weaponEquipPos.position, weaponEquipPos.rotation);
            newWeapon.transform.SetParent(weaponEquipPos);
            currentlyEquippedWeapon = newWeapon;
        }
    }
}
