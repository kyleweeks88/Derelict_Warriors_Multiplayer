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

    public override void OnStartClient()
    {
        if(currentlyEquippedWeapon != null)
        {
            playerMgmt.animMgmt.SetAnimation(currentlyEquippedWeapon.weaponData.animationSet);
        }
    }

    public void CheckEquipWeapon(Weapon _weaponToEquip)
    {
        NetworkIdentity netId = this.GetComponent<NetworkIdentity>();
        weaponToEquip = _weaponToEquip;
        EquipWeapon(netId, weaponToEquip, weaponEquipPos);

        if(!base.isServer)
            CmdEquipWeapon(netId, weaponEquipPos);
    }

    [Command]
    void CmdEquipWeapon(NetworkIdentity netId, Transform equipPos)
    {        
        EquipWeapon(netId, weaponToEquip, equipPos);
        RpcEquipWeapon(netId, equipPos);
    }

    [ClientRpc]
    void RpcEquipWeapon(NetworkIdentity netId, Transform equipPos)
    {
        if (base.hasAuthority) { return; }

        EquipWeapon(netId, weaponToEquip, equipPos);
    }

    public void EquipWeapon(NetworkIdentity netId, Weapon _weaponToEquip, Transform _weaponEquipPos)
    {
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

            AnimationManager am = netId.gameObject.GetComponent<AnimationManager>();
            am.SetAnimation(newWeapon.weaponData.animationSet);
            weaponToEquip = newWeapon;
            currentlyEquippedWeapon = newWeapon;
        }
    }
}
