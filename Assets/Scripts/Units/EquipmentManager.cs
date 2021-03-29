using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class EquipmentManager : NetworkBehaviour
{
    [SerializeField] PlayerManager playerMgmt;

    public List<Weapon> equippedWeapons = new List<Weapon>();
    public Weapon currentlyEquippedWeapon;
    Weapon weaponToEquip;

    public int weaponLimit = 2;

    [SerializeField] Transform weaponEquipPos;
    StatModifier weaponDamageModifier;

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
        //if(equippedWeapons.Count >= weaponLimit)
        //{
        //    SwapWeapon(netId, equippedWeapons[0], _weaponToEquip);
        //}

        if (equippedWeapons[0] != null)
        {
            // UNEQUIP WEAPON LOGIC
            SwapWeapon(netId, equippedWeapons[0], _weaponToEquip);
        }
        else
        {
            Weapon newWeapon = Instantiate(_weaponToEquip, _weaponEquipPos);
            newWeapon.transform.SetParent(_weaponEquipPos);
            newWeapon.transform.localPosition = Vector3.zero;
            newWeapon.transform.localRotation = Quaternion.Euler(Vector3.zero);

            weaponDamageModifier = new StatModifier(newWeapon.weaponData.damage, newWeapon.type);
            playerMgmt.playerStats.attackDamage.AddModifer(weaponDamageModifier);

            AnimationManager am = netId.gameObject.GetComponent<AnimationManager>();
            am.SetAnimation(newWeapon.weaponData.animationSet);
            weaponToEquip = newWeapon;
            equippedWeapons[0] = newWeapon;
        }
    }

    public void UnequipWeapon(Weapon _weaponToUnequip)
    {
        // DROP CURRENT WEAPON??
        playerMgmt.playerStats.attackDamage.RemoveModifier(weaponDamageModifier);
        Destroy(_weaponToUnequip.gameObject);

        // CLEAR EQUIPPED WEAPON
        equippedWeapons[0] = null;
    }

    public void SwapWeapon(NetworkIdentity netId, Weapon _weaponToUnequip, Weapon _weaponToEquip)
    {
        UnequipWeapon(_weaponToUnequip);
        EquipWeapon(netId, _weaponToEquip, weaponEquipPos);
    }
}
