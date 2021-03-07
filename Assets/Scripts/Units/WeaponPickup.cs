using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    public Weapon weapon;
    public GameObject interactingEntity;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<EquipmentManager>() != null)
        {
            interactingEntity = other.gameObject;

            interactingEntity.GetComponent<EquipmentManager>().EquipWeapon(weapon);
            Object.Destroy(this.gameObject);
        }
    }


}
