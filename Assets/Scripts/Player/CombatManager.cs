﻿ using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CombatManager : NetworkBehaviour
{
    #region Setup
    [SerializeField] EquipmentManager equipmentManager;
    [SerializeField] GameObject hitFX = null;
    public string attackAnim = string.Empty;
    public LayerMask whatIsDamageable;

    [Header("Component Reference")]
    [SerializeField] CharacterStats myStats = null;
    [SerializeField] GameObject leftHand;
    [SerializeField] GameObject rightHand;
    [SerializeField] Animator myAnimator;
    [SerializeField] NetworkAnimator myNetworkAnimator = null;
    PlayerManager playerMgmt;
    Transform impactOrigin;
    Transform impactEnd;

    [Tooltip("Determines how long the player stays in combat mode")]
    public float combatTimer = 10f;
    [HideInInspector] public float currentCombatTimer;
    [HideInInspector] public bool inCombat;

    [HideInInspector] public bool canRecieveAttackInput;
    [HideInInspector] public bool attackInputRecieved;
    [SerializeField] float impactRadius = 1f;
    public bool impactActivated;

    [Header("RANGED TESTING")]
    [SerializeField] Transform projectileSpawn;
    [SerializeField] GameObject projectile = null;
    float nextShotTime = 0f;
    [SerializeField] float msBetweenShots = 0f;
    #endregion

    public override void OnStartAuthority()
    {
        enabled = true;
        playerMgmt = GetComponent<PlayerManager>();
    }

    [ClientCallback]
    private void Update()
    {
        if (!hasAuthority) { return; }
        
        myAnimator.SetBool("inCombat", inCombat);

        if(attackAnim != null)
            myAnimator.SetBool(attackAnim, playerMgmt.inputMgmt.attackInputHeld);

        CheckMeleeAttack();

        if(playerMgmt.inputMgmt.attackInputHeld)
        {
            ChargingAttack();
        }
    }

    #region Ranged
    bool ShotTimeMet(bool resetTime = true)
    {
        bool result = (Time.time >= nextShotTime);

        if (resetTime)
            nextShotTime = Time.time + msBetweenShots / 1000f;

        return result;
    }

    // THIS WILL BE CALLED FROM AN ANIMATION EVENT????
    [Client]
    public void CheckRangedAttack()
    {
        if (!base.hasAuthority) { return; }
        if (!ShotTimeMet()) { return; }

        // Spawn projectile locally w/auth
        SpawnProjectile(projectileSpawn.position, projectileSpawn.rotation);
        // Ask the server to check your pos, and spawn a projectile for the server
        CmdRangedAttack(projectileSpawn.position, projectileSpawn.rotation);
    }

    void SpawnProjectile(Vector3 pos, Quaternion rot)
    {
        GameObject newProjectile = Instantiate(projectile,
            pos,
            rot);
    }

    [Command]
    void CmdRangedAttack(Vector3 pos, Quaternion rot)
    {
        if (!ShotTimeMet()) { return; }

        float maxPosOffset = 1;
        if (Vector3.Distance(pos, projectileSpawn.position) > maxPosOffset)
        {
            Vector3 posDir = pos - projectileSpawn.position;
            pos = projectileSpawn.position + (posDir * maxPosOffset);
        }

        // This is for the client/host to spawn a projectile
        //if (base.isClient)
        //    SpawnProjectile();

        // Tells observing clients to also spawn the projectile
        RpcRangedAttack(pos, rot);
    }  

    [ClientRpc]
    void RpcRangedAttack(Vector3 pos, Quaternion rot)
    {
        //if (base.isServer) { return; }
        if(base.hasAuthority){return;}
        if (!ShotTimeMet()) { return; }

        float maxPosOffset = 1;
        if (Vector3.Distance(pos, projectileSpawn.position) > maxPosOffset)
        {
            Vector3 posDir = pos - projectileSpawn.position;
            pos = projectileSpawn.position + (posDir * maxPosOffset);
        }

        SpawnProjectile(pos, rot);
    }
    #endregion

    #region Melee
    /// <summary>
    /// Called by the player's attack input
    /// </summary>
    public void CheckAttack()
    {
        //string animString = string.Empty;
        // RUN LOGIC TO DETERMINE THE MEANS OF THE ATTACK //
        // CHECK IF ARMED OR UNARMED //
        // CHECK IF STUNNED //
        // OTHER IMPORTANT STUFF //

        // If unarmed: animString = "unarmedAttack"

        // If 1H melee weapon: animString = "meleeAttack_1H"

        // If weaponMgmt.currentWeapon.Type == RangedWeapon: animString = "rangedAttack"

        // Etc...
        if(equipmentManager.currentlyEquippedWeapon != null)
        {
            Weapon newWeapon = equipmentManager.currentlyEquippedWeapon;

            if(newWeapon.weaponData.weaponType == WeaponData.WeaponType.Melee)
            {
                if (newWeapon.weaponData.wieldStyle == WeaponData.WieldStyle.OneHanded)
                {
                    attackAnim = "1H_meleeAttackHold";
                }
                else if (newWeapon.weaponData.wieldStyle == WeaponData.WieldStyle.TwoHanded)
                {
                    attackAnim = "2H_meleeAttackHold";
                }
            }
            
            if(newWeapon.weaponData.weaponType == WeaponData.WeaponType.Ranged)
            {
                if(newWeapon.weaponData.wieldStyle == WeaponData.WieldStyle.OneHanded)
                {
                    attackAnim = "1H_rangedAttackHold";
                }
                else if (newWeapon.weaponData.wieldStyle == WeaponData.WieldStyle.TwoHanded)
                {
                    attackAnim = "2H_rangedAttackHold";
                }
            }
        }
        else
        {
            attackAnim = "unarmedAttack";
        }

        // Plays the appropriate attack animation

        if(playerMgmt.inputMgmt.attackInputHeld)
            myAnimator.SetBool(attackAnim, true);

        inCombat = true;
        currentCombatTimer = combatTimer;
    }

    public void ChargingAttack()
    {
        Debug.Log("CHARGING!");
    }

    void CheckMeleeAttack()
    {
        if (impactActivated)
        {
            MeleeWeapon equippedWeapon = equipmentManager.currentlyEquippedWeapon as MeleeWeapon;
            if (equippedWeapon != null)
            {
                CheckCreateImpactCollider(equippedWeapon);
            }
        }
    }

    /// <summary>
    /// Called by an Animation Event from the player checks an int
    /// to determine the means of the attack
    /// </summary>
    /// <param name="handInt"></param>
    public void ActivateImpact(int impactID)
    {
        // Only activate the impact if you have auth over this object
        if(!base.hasAuthority) { return; }

        //// You have no weapon equipped
        //// Left hand
        //if (impactID == 1)
        //{
        //    impactOrigin = leftHand.transform;
        //    impactEnd = leftHand.transform;
        //    impactRadius = 0.5f;
        //}
        //// Right hand
        //else if (impactID == 2)
        //{
        //    impactOrigin = rightHand.transform;
        //    impactEnd = rightHand.transform;
        //    impactRadius = 0.5f;
        //}
        ////else if (impactID == 3)
        ////{
        ////    // LEFT OR RIGHT FOOT?
        ////    // SINGLE FEET ATTACK POS?
        ////}

        impactActivated = true;
    }

    void CheckCreateImpactCollider(MeleeWeapon equippedWeapon)
    {
        // Generate a collider array that will act as the weapon's collision area
        Collider[] impactCollisions = null;

        if (equippedWeapon != null)
        {
            impactCollisions = Physics.OverlapCapsule(
                equippedWeapon.impactOrigin.position,
                equippedWeapon.impactEnd.position,
                equippedWeapon.impactRadius, whatIsDamageable);
        }
        else
        {
            // UNARMED IMPACT LOGIC
        }

        // for each object the collider hits do this stuff...
        foreach (Collider hit in impactCollisions)
        {
            // Create equippedWeapon's hit visuals
            GameObject hitGfx = Instantiate(equippedWeapon.weaponData.hitVisuals, 
                hit.ClosestPoint(equippedWeapon.impactEnd.position), Quaternion.identity);

            // If the collider hit has an IHaveHealth component on it.
            if (hit.gameObject.GetComponentInParent<IHaveHealth>() != null)
            {
                // Grab the NetworkIdentity of the hit object to pass into the Cmd
                NetworkIdentity objIdentity = hit.gameObject.GetComponentInParent<NetworkIdentity>();
                CmdCreateImpactCollider(objIdentity,
                    equippedWeapon.impactOrigin.position, 
                    equippedWeapon.impactEnd.position, 
                    equippedWeapon.impactRadius);

                impactActivated = false;
            }
        }
    }

    [Command]
    void CmdCreateImpactCollider(NetworkIdentity hitObj, Vector3 origin, Vector3 end, float colRadius)
    {
        // ALSO CHECK THE impactRadius AGAINST A CLAMPED RADIUS TO MAKE SURE CLIENT
        // ISN'T HACKING impactRadius SIZE.
        //if (colRadius > 3f) { return; }

        // CHECK LENGTH OF CAPSULE COLLIDER FOR HACKING
        //float colLength = Vector3.Distance(origin, end);
        //if (colLength > 5f) { return; }

        // CHECK DISTANCE FROM CLIENT TO HIT OBJECT
        //float distToHitObject = Vector3.Distance(this.transform.position, hitObj.transform.position);
        //if (distToHitObject > 5f) { return; }

        Collider[] verifiedImpactCol = Physics.OverlapCapsule(origin, end, colRadius, whatIsDamageable);
        foreach (Collider hit in verifiedImpactCol)
        {
            if (hit.gameObject.GetComponentInParent<IHaveHealth>() != null)
            {
                // PASS THE OBJECT HIT INTO A SERVER CHECK AND COMMAND
                CheckProcessAttack(hitObj.gameObject);
                impactActivated = false;
            }
        }

        RpcCreateImpactCollider(hitObj, origin, end, colRadius);
    }

    [ClientRpc]
    void RpcCreateImpactCollider(NetworkIdentity hitObj, Vector3 origin, Vector3 end, float colRadius)
    {
        if (hasAuthority) { return; }

        Collider[] verifiedImpactCol = Physics.OverlapCapsule(origin, end, colRadius, whatIsDamageable);
        foreach (Collider hit in verifiedImpactCol)
        {
            /// CREATE THE WEAPONS HIT VISUALS HERE ///

            if (hit.gameObject.GetComponentInParent<IHaveHealth>() != null)
            {
                // PASS THE OBJECT HIT INTO A SERVER CHECK AND COMMAND
                impactActivated = false;
                return;
            }
            else
            {
                impactActivated = false;
                return;
            }
        }
    }

    [Server]
    void CheckProcessAttack(GameObject target)
    {
        IHaveHealth entity = target.GetComponentInParent<IHaveHealth>();
        entity.TakeDamage(myStats.baseAttackDamage * equipmentManager.currentlyEquippedWeapon.weaponData.damage);
    }
    #endregion
}
