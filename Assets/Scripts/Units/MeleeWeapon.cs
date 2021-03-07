using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : Weapon
{
    public Transform impactOrigin;
    public Transform impactEnd;
    public float impactRadius;

    public void InstantiateHitVisuals(Vector3 hitPoint)
    {
        GameObject hitVis = Instantiate(weaponData.hitVisuals, hitPoint, Quaternion.identity);
    }

    /// SHOULD I CREATE THE IMPACT COLLIDER HERE ON THE WEAPON??? ///

    void OnDrawGizmos()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(impactOrigin.position, impactEnd.position);
    }
}
