using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class EquipmentManager : NetworkBehaviour
{
    public Weapon[] equippedWeapons;
    [SerializeField] PlayerManager playerMgmt;
    public Weapon currentlyEquippedWeapon;
    Weapon weaponToEquip;

    [SerializeField] Transform weaponEquipPos;

    public override void OnStartAuthority()
    {
        if(currentlyEquippedWeapon != null)
        {
            playerMgmt.animMgmt.SetAnimation(currentlyEquippedWeapon.weaponData.animationSet);
        }
    }

    [Client]
    public void CheckEquipWeapon(Weapon _weaponToEquip)
    {
        weaponToEquip = _weaponToEquip;
        EquipWeapon(weaponToEquip, weaponEquipPos);

        //if(!base.isServer)
        CmdEquipWeapon(weaponEquipPos);
    }

    [Command]
    void CmdEquipWeapon(Transform equipPos)
    {
        EquipWeapon(weaponToEquip, equipPos);
        RpcEquipWeapon(equipPos);
    }

    [ClientRpc]
    void RpcEquipWeapon(Transform equipPos)
    {
        if (base.hasAuthority) { return; }

        EquipWeapon(weaponToEquip, equipPos);

        //Weapon newWeapon = Instantiate(weaponToEquip, equipPos);
        //newWeapon.transform.SetParent(equipPos);
        //newWeapon.transform.localPosition = Vector3.zero;
        //newWeapon.transform.localRotation = Quaternion.Euler(Vector3.zero);
    }

    public void EquipWeapon(Weapon _weaponToEquip, Transform _weaponEquipPos)
    {
        if (!base.hasAuthority) { return; }

        if (currentlyEquippedWeapon != null)
        {
            // UNEQUIP WEAPON LOGIC
        }
        else
        {
            Weapon newWeapon = Instantiate(_weaponToEquip, _weaponEquipPos);
            newWeapon.transform.SetParent(_weaponEquipPos);
            newWeapon.transform.localPosition = Vector3.zero;
            newWeapon.transform.localRotation = Quaternion.Euler(Vector3.zero);

            playerMgmt.animMgmt.SetAnimation(newWeapon.weaponData.animationSet);
            currentlyEquippedWeapon = newWeapon;
        }
    }
}
