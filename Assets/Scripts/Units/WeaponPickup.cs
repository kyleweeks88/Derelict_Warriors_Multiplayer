using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class WeaponPickup : NetworkBehaviour
{
    public GameObject interactingEntity;
    public Weapon weaponToPickup;

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player")
        {
            //PickupWeapon();
            interactingEntity = col.gameObject;
            HandleEntityInput(interactingEntity, true);
        }
        /// ELSE IF THE TAG IS AN NPC? ///
    }

    void OnTriggerExit(Collider col)
    {
        if (col.tag == "Player")
        {
            interactingEntity = col.gameObject;
            HandleEntityInput(interactingEntity, false);
        }
        /// ELSE IF THE TAG IS AN NPC? ///
    }

    void HandleEntityInput(GameObject colObj, bool boolVal)
    {
        /// DO I NEED THE CLIENT'S NETWORK CONNECTION??? ///
        //ClientInstance ci = ClientInstance.ReturnClientInstance(
        //    colObj.GetComponent<NetworkIdentity>().connectionToClient);
        ClientInstance ci = ClientInstance.ReturnClientInstance();

        if (ci != null)
        {
            InputManager playerInput = ci.GetComponent<InputManager>();
            if (playerInput != null)
            {
                if (boolVal)
                    playerInput.Event_OnInteract += PickupWeapon;

                if (!boolVal)
                    playerInput.Event_OnInteract -= PickupWeapon;
            }
        }
    }

    void PickupWeapon()
    {
        Debug.Log("LOCAL");
        CmdPickupWeapon();
    }

    [Command(ignoreAuthority = true)]
    public void CmdPickupWeapon()
    {
        EquipmentManager entityEquipment = interactingEntity.GetComponent<EquipmentManager>();
        if(entityEquipment != null)
        {
            entityEquipment.CheckEquipWeapon(weaponToPickup);
        }
        Debug.Log("SERVER");
        RpcPickupWeapon();
        Object.Destroy(this.gameObject);
    }

    [ClientRpc]
    void RpcPickupWeapon()
    {
        EquipmentManager entityEquipment = interactingEntity.GetComponent<EquipmentManager>();
        if (entityEquipment != null)
        {
            entityEquipment.CheckEquipWeapon(weaponToPickup);
        }
        Debug.Log("CLIENT RPC");
        Object.Destroy(this.gameObject);
    }
}
