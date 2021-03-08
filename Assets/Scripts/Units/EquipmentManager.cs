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
            Weapon newWeapon = Instantiate(weaponToEquip, weaponEquipPos);
            newWeapon.transform.SetParent(weaponEquipPos);
            newWeapon.transform.localPosition = Vector3.zero;
            newWeapon.transform.localRotation = Quaternion.Euler(Vector3.zero);

            currentlyEquippedWeapon = newWeapon;
        }
    }
}
