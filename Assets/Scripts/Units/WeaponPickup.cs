using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class WeaponPickup : NetworkBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
            PickupWeapon();
    }

    [Client]
    void PickupWeapon()
    {
        CmdPickupWeapon();
        Object.Destroy(this.gameObject);
    }

    [Command]
    void CmdPickupWeapon()
    {
        RpcPickupWeapon();
        Object.Destroy(this.gameObject);
    }

    [ClientRpc]
    void RpcPickupWeapon()
    {
        Object.Destroy(this.gameObject);
    }
}
