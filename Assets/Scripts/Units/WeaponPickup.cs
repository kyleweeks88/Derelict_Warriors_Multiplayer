using UnityEngine;
using Mirror;

public class WeaponPickup : NetworkBehaviour
{
    GameObject interactingEntity;
    // The weapon prefab for this pickup
    public Weapon weaponToPickup_Pf;

    void OnTriggerEnter(Collider col)
    {
        // Check if the colliding object has an EquipmentManager component
        EquipmentManager colEquipMgmt = col.gameObject.GetComponent<EquipmentManager>();
        if (colEquipMgmt != null)
        {
            interactingEntity = col.gameObject;
            HandleEntityInput(interactingEntity, true);
        }
    }

    void OnTriggerExit(Collider col)
    {
        // Check if the colliding object has an EquipmentManager component
        EquipmentManager colEquipMgmt = col.gameObject.GetComponent<EquipmentManager>();
        if (colEquipMgmt != null)
        {
            HandleEntityInput(interactingEntity, false);
        }
    }

    /// <summary>
    /// Subscribes or Unsubscribes this Weapon Pickup object to an event
    /// on the interacting entity's InputManager.
    /// </summary>
    /// <param name="colObj"></param>
    /// <param name="boolVal"></param>
    void HandleEntityInput(GameObject colObj, bool boolVal)
    {
        // Retrieve the Client Instance object relative to the entity
        ClientInstance ci = ClientInstance.ReturnClientInstance(
            colObj.GetComponent<NetworkIdentity>().connectionToClient);

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

    /// <summary>
    /// Retrieve the NetworkIdentity of the interacting entity and 
    /// call the Command function passing in that NetworkIdentity
    /// </summary>
    void PickupWeapon()
    {
        NetworkIdentity colNetId = interactingEntity.GetComponent<NetworkIdentity>();
        CmdPickupWeapon(colNetId);
    }

    /// <summary>
    /// On the server - Retrieve the EquipmentManager from the passed NetworkIdentity, attempt to equip the weapon prefab.
    /// Call the ClientRpc and destroy this Weapon Pickup object.
    /// </summary>
    /// <param name="_colNetId"></param>
    [Command(ignoreAuthority = true)]
    public void CmdPickupWeapon(NetworkIdentity _colNetId)
    {
        EquipmentManager entityEquipment = _colNetId.gameObject.GetComponent<EquipmentManager>();
        if(entityEquipment != null)
        {
            entityEquipment.CheckEquipWeapon(weaponToPickup_Pf);
        }

        RpcPickupWeapon(_colNetId);
        Object.Destroy(this.gameObject);
    }

    /// <summary>
    /// Tell all observing clients to Retrieve the EquipmentManager from the passed NetworkIdentity, attempt to equip the weapon prefab.
    /// and destroy this Weapon Pickup object.
    /// </summary>
    /// <param name="_colNetId"></param>
    [ClientRpc]
    void RpcPickupWeapon(NetworkIdentity _colNetId)
    {
        EquipmentManager entityEquipment = _colNetId.gameObject.GetComponent<EquipmentManager>();
        if (entityEquipment != null)
        {
            entityEquipment.CheckEquipWeapon(weaponToPickup_Pf);
        }

        Object.Destroy(this.gameObject);
    }
}
