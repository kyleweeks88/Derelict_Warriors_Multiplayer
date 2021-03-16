 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CombatManager : NetworkBehaviour
{
    #region Setup
    public LayerMask whatIsDamageable;

    [Header("Component Ref")]
    [SerializeField] PlayerManager playerMgmt;

    float combatTimer = 10f;
    float currentCombatTimer;
    [HideInInspector] public bool inCombat;
    [HideInInspector] public bool impactActivated;
    [HideInInspector] public string attackAnim;

    [Header("RANGED TESTING")] 
    [SerializeField] Transform projectileSpawn;
    // THIS PROJECTILE NEEDS TO BE DETERMINED BY THE CURRENT WEAPON \\
    // IT SHOULD NOT BE ON THIS SCRIPT!!!
    [SerializeField] Projectile projectile = null; //<<<========== FIX THIS!!!
    float nextShotTime = 0f;
    [SerializeField] float msBetweenShots = 0f;
    #endregion


    [ClientCallback]
    private void Update()
    {
        if (!hasAuthority) { return; }

        CheckMeleeAttack();

        if(playerMgmt.inputMgmt.attackInputHeld)
        {
            ChargeMeleeAttack();
        }

        if(playerMgmt.inputMgmt.rangedAttackHeld)
        {
            ChargeRangedAttack();
        }
    }

    public void HandleCombatTimer()
    {
        if (!base.hasAuthority) { return; }

        currentCombatTimer -= Time.deltaTime;

        if (currentCombatTimer <= 0)
        {
            currentCombatTimer = combatTimer;
            inCombat = false;
        }
    }

    #region Ranged

    // Called by the player's input manager
    public void RangedAttackPerformed()
    {
        attackAnim = "rangedAttackHold";
    }

    public virtual void ChargeRangedAttack()
    {
        // CHECK IF CHARACTER HAS A RANGED WEAPON \\

        // ACCESS THAT RANGED WEAPON SOMEHOW \\

        // INCREASE THAT RANGED WEAPON'S DAMAGE AND/OR PROJECTILE VELOCITY \\

        Debug.Log("CHARGING RANGED ATTACK!");
    }

    // CALLED BY AN ANIMATION EVENT 
    [Client]
    public void CheckRangedAttack()
    {
        if (!base.hasAuthority) { return; }
        // Ask the server to check your pos, and spawn a projectile for the server
        CmdRangedAttack(projectileSpawn.position, projectileSpawn.rotation, playerMgmt.myCamera.transform.forward);
    }

    [Command]
    void CmdRangedAttack(Vector3 pos, Quaternion rot, Vector3 dir)
    {
        float maxPosOffset = 1;
        if (Vector3.Distance(pos, projectileSpawn.position) > maxPosOffset)
        {
            Vector3 posDir = pos - projectileSpawn.position;
            pos = projectileSpawn.position + (posDir * maxPosOffset);
        }

        SpawnProjectile(pos, rot, dir);
    }  

    [Server]
    void SpawnProjectile(Vector3 pos, Quaternion rot, Vector3 dir)
    {
        Projectile newProjectile = Instantiate(projectile,
            pos,
            rot);
        newProjectile.SetSpeed(20f, dir);

        NetworkServer.Spawn(newProjectile.gameObject);
    }
    #endregion

    #region Melee
    /// <summary>
    /// Called by the player's attack input
    /// </summary>
    public void AttackPerformed()
    {
        // If you have a weapon equipped
        if (playerMgmt.equipmentMgmt.currentlyEquippedWeapon != null)
        {
            // If current weapon is a melee type...
            if (playerMgmt.equipmentMgmt.currentlyEquippedWeapon.weaponData.weaponType == WeaponData.WeaponType.Melee)
            {
                MeleeWeapon myWeapon = playerMgmt.equipmentMgmt.currentlyEquippedWeapon as MeleeWeapon;
                if((playerMgmt.staminaMgmt.GetCurrentVital() - myWeapon.meleeData.staminaCost) > 0)
                {
                    if(myWeapon.meleeData.isChargeable)
                    {
                        // Determines correct animation to play
                        attackAnim = "meleeAttackHold";
                        playerMgmt.animMgmt.HandleMeleeAttackAnimation(true);
                    }
                    else
                    {
                        attackAnim = "meleeAttackTrigger";
                    }
                }
                
            }
        }
        // If you have no equipped weapon, you're unarmed
        else
        {
            attackAnim = "meleeAttackHold";
            playerMgmt.animMgmt.HandleMeleeAttackAnimation(true);
            // EVENTUALLY THIS WILL TRIGGER DIFFERENT UNARMED COMBOS \\\
        }

        inCombat = true;
        currentCombatTimer = combatTimer;
    }

    public virtual void ChargeMeleeAttack()
    {
        // If you have a weapon equipped...
        if (playerMgmt.equipmentMgmt.currentlyEquippedWeapon != null)
        {
            // If the current weapon is chargeable...
            if (playerMgmt.equipmentMgmt.currentlyEquippedWeapon.weaponData.isChargeable)
            {
                // If that weapon's current charge is less than it's max charge,
                // increase the currenty charge by time * charge rate until it reaches it's max charge.
                if (playerMgmt.equipmentMgmt.currentlyEquippedWeapon.currentCharge <=
                    playerMgmt.equipmentMgmt.currentlyEquippedWeapon.maxCharge)
                {
                    playerMgmt.equipmentMgmt.currentlyEquippedWeapon.currentCharge +=
                        Time.deltaTime * playerMgmt.equipmentMgmt.currentlyEquippedWeapon.chargeRate / 100f;
                }
            }
            else
            {
                return;
            }
        }
        else
        {
            // UNARMED CHARGING LOGIC
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

        MeleeWeapon myWeapon = playerMgmt.equipmentMgmt.currentlyEquippedWeapon as MeleeWeapon;
        playerMgmt.staminaMgmt.TakeDamage(myWeapon.meleeData.staminaCost);

        impactActivated = true;
    }

    void CheckMeleeAttack()
    {
        if (impactActivated)
        {
            MeleeWeapon equippedWeapon = playerMgmt.equipmentMgmt.currentlyEquippedWeapon as MeleeWeapon;
            if (equippedWeapon != null)
            {
                equippedWeapon.CheckCreateImpactCollider(this);
            }
        }
    }

    //void CheckCreateImpactCollider(MeleeWeapon equippedWeapon)
    //{
    //    // Generate a collider array that will act as the weapon's collision area
    //    Collider[] impactCollisions = null;

    //    if (equippedWeapon != null)
    //    {
    //        impactCollisions = Physics.OverlapCapsule(
    //            equippedWeapon.impactOrigin.position,
    //            equippedWeapon.impactEnd.position,
    //            equippedWeapon.impactRadius, whatIsDamageable);
    //    }
    //    else
    //    {
    //        // UNARMED IMPACT LOGIC
    //    }

    //    // for each object the collider hits do this stuff...
    //    foreach (Collider hit in impactCollisions)
    //    {
    //        // Create equippedWeapon's hit visuals
    //        GameObject hitGfx = Instantiate(equippedWeapon.weaponData.hitVisuals, 
    //            hit.ClosestPoint(equippedWeapon.impactEnd.position), Quaternion.identity);

    //        // If the collider hit has an NpcHealthManager component on it.
    //        if (hit.gameObject.GetComponent<NpcHealthManager>() != null)
    //        {
    //            CheckProcessAttack(hit.gameObject.GetComponent<NpcHealthManager>());
    //            impactActivated = false;
    //            //chargeMultiplier = 0f;
    //            playerMgmt.equipmentMgmt.currentlyEquippedWeapon.ResetCharge();
    //        }

    //        // Create the impact collider on the server
    //        CmdCreateImpactCollider(
    //            equippedWeapon.impactOrigin.position,
    //            equippedWeapon.impactEnd.position,
    //            equippedWeapon.impactRadius);
    //    }
    //}

    [Command]
    public void CmdCreateImpactCollider(Vector3 origin, Vector3 end, float colRadius)
    {
        // DO SOME SERVER VERIFICATION RIGHT HERE \\

        Collider[] verifiedImpactCol = Physics.OverlapCapsule(origin, end, colRadius, whatIsDamageable);
        foreach (Collider hit in verifiedImpactCol)
        {
            if (hit.gameObject.GetComponent<NpcHealthManager>() != null)
            {
                // Process the attack and damage on the server
                //CheckProcessAttack(hit.gameObject.GetComponent<NpcHealthManager>());
                impactActivated = false;
            }

            RpcCreateImpactCollider(origin, end, colRadius);
        }
    }

    [ClientRpc]
    void RpcCreateImpactCollider(Vector3 origin, Vector3 end, float colRadius)
    {
        if (hasAuthority) { return; }

        Collider[] verifiedImpactCol = Physics.OverlapCapsule(origin, end, colRadius, whatIsDamageable);
        foreach (Collider hit in verifiedImpactCol)
        {
            if (hit.gameObject.GetComponent<NpcHealthManager>() != null)
            {
                // PASS THE OBJECT HIT INTO A SERVER CHECK AND COMMAND
                //CheckProcessAttack(hit.gameObject.GetComponent<NpcHealthManager>());
                impactActivated = false;
            }
        }
    }

    public void CheckProcessAttack(NpcHealthManager target)
    {
        float dmgVal = (playerMgmt.equipmentMgmt.currentlyEquippedWeapon.weaponData.damage 
            * playerMgmt.equipmentMgmt.currentlyEquippedWeapon.currentCharge) + playerMgmt.playerStats.baseAttackDamage;

        NetworkIdentity targetNetId = target.gameObject.GetComponent<NetworkIdentity>();

        CmdProcessAttack(targetNetId, dmgVal);
    }

    [Command]
    void CmdProcessAttack(NetworkIdentity _targetNetId, float dmgVal)
    {
        NpcHealthManager entity = _targetNetId.gameObject.GetComponent<NpcHealthManager>();
        entity.TakeDamage(dmgVal);
        //RpcProcessAttack(_targetNetId, dmgVal);
    }
    #endregion
}
