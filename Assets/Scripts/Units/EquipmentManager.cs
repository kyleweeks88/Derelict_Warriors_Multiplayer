using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class EquipmentManager : NetworkBehaviour
{
    public Weapon currentlyEquippedWeapon;
    Weapon weaponToEquip;

    [SerializeField] Transform weaponEquipPos;

    public void EquipWeapon(Weapon _weaponToEquip, Transform _weaponEquipPos)
    {
        if(currentlyEquippedWeapon != null)
        {
            // UNEQUIP WEAPON LOGIC
        }
        else
        {
            Weapon newWeapon = Instantiate(_weaponToEquip, _weaponEquipPos);
            newWeapon.transform.SetParent(_weaponEquipPos);
            newWeapon.transform.localPosition = Vector3.zero;
            newWeapon.transform.localRotation = Quaternion.Euler(Vector3.zero);

            currentlyEquippedWeapon = newWeapon;
        }
    }

    public void CheckEquipWeapon(Weapon _weaponToEquip)
    {
        weaponToEquip = _weaponToEquip;
        EquipWeapon(weaponToEquip, weaponEquipPos);
        CmdEquipWeapon();
    }

    [Command]
    void CmdEquipWeapon()
    {
        //EquipWeapon(weaponToEquip);
        RpcEquipWeapon(weaponEquipPos);
    }

    [ClientRpc]
    void RpcEquipWeapon(Transform equipPos)
    {
        if (base.hasAuthority) { return; }

        Weapon newWeapon = Instantiate(weaponToEquip, equipPos);
        newWeapon.transform.SetParent(equipPos);
        newWeapon.transform.localPosition = Vector3.zero;
        newWeapon.transform.localRotation = Quaternion.Euler(Vector3.zero);
        //EquipWeapon(weaponToEquip);
    }
}
