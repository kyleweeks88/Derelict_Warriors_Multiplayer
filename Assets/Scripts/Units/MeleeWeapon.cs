using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MeleeWeapon : Weapon
{
    [Header("Melee Specs")]
    public MeleeWeaponData meleeData;
    public Transform impactOrigin;
    public Transform impactEnd;
    public float impactRadius;

    public void InstantiateHitVisuals(Vector3 hitPoint)
    {
        GameObject hitVis = Instantiate(weaponData.hitVisuals, hitPoint, Quaternion.identity);
    }

    /// SHOULD I CREATE THE IMPACT COLLIDER HERE ON THE WEAPON??? ///
    public void CheckCreateImpactCollider(CombatManager _combatMgmt)
    {
        // Generate a collider array that will act as the weapon's collision area
        Collider[] impactCollisions = null;

        impactCollisions = Physics.OverlapCapsule(
            impactOrigin.position,
            impactEnd.position,
            impactRadius, _combatMgmt.whatIsDamageable);


        // for each object the collider hits do this stuff...
        foreach (Collider hit in impactCollisions)
        {
            // Create equippedWeapon's hit visuals
            GameObject hitGfx = Instantiate(meleeData.hitVisuals,
                hit.ClosestPoint(impactEnd.position), Quaternion.identity);

            // If the collider hit has an NpcHealthManager component on it.
            if (hit.gameObject.GetComponent<NpcHealthManager>() != null)
            {
                _combatMgmt.CheckProcessAttack(hit.gameObject.GetComponent<NpcHealthManager>());
                _combatMgmt.impactActivated = false;
                ResetCharge();
            }

            // Create the impact collider on the server
            _combatMgmt.CmdCreateImpactCollider(
                impactOrigin.position,
                impactEnd.position,
                impactRadius);
        }
    }

    void OnDrawGizmos()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(impactOrigin.position, impactEnd.position);
    }
}
