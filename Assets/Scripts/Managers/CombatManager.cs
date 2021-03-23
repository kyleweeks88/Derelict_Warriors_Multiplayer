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

    FloatVariable health;
    FloatVariable stamina;

    public bool canRecieveAttackInput;
    public bool attackInputRecieved;
    public bool attackInputHeld;
    public bool rangedAttackHeld;
    #endregion

    public override void OnStartAuthority()
    {
        canRecieveAttackInput = true;

        playerMgmt.inputMgmt.attackEventStarted += AttackPerformed;
        playerMgmt.inputMgmt.attackEventCancelled += AttackReleased;
        playerMgmt.inputMgmt.rangedAttackEventStarted += RangedAttackPerformed;
        playerMgmt.inputMgmt.rangedAttackEventCancelled += RangedAttackReleased;

        stamina = playerMgmt.vitalsMgmt.stamina;
        health = playerMgmt.vitalsMgmt.health;
    }

    [ClientCallback]
    private void Update()
    {
        if (!hasAuthority) { return; }

        CheckMeleeAttack();

        if(attackInputHeld)
        {
            ChargeMeleeAttack();
        }

        if(rangedAttackHeld)
        {
            ChargeRangedAttack();
        }
    }

    /// <summary>
    /// Handles the duration that the entity will be in combat and playing it's combat idle animtion.
    /// Turns the bool "inCombat" to false when timer reaches zero.
    /// </summary>
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
    public void RangedAttackPerformed()
    {
        // if player is locked into an "interacting" state then don't let this happen.
        if (playerMgmt.isInteracting) { return; }

        if (!canRecieveAttackInput) { return; }

        if (canRecieveAttackInput)
        {
            rangedAttackHeld = true;
            attackAnim = "rangedAttackHold";
            playerMgmt.animMgmt.HandleRangedAttackAnimation(rangedAttackHeld);
        }
    }

    public void RangedAttackReleased()
    {
        rangedAttackHeld = false;
        playerMgmt.animMgmt.HandleRangedAttackAnimation(rangedAttackHeld);
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
    /// Called by the player's attack input.
    /// </summary>
    public void AttackPerformed()
    {
        // If the player is interacting with a contextual object, exit.
        if (playerMgmt.isInteracting) { return; }
        // If the player is unable to recieve attack input, exit.
        if (!canRecieveAttackInput) { return; }

        if (canRecieveAttackInput)
        {
            attackInputHeld = true;

            // If you have a weapon equipped
            if (playerMgmt.equipmentMgmt.currentlyEquippedWeapon != null)
            {
                // If current weapon is a melee type...
                if (playerMgmt.equipmentMgmt.currentlyEquippedWeapon.weaponData.weaponType == WeaponData.WeaponType.Melee)
                {
                    MeleeWeapon myWeapon = playerMgmt.equipmentMgmt.currentlyEquippedWeapon as MeleeWeapon;
                    // Checks if the entity has enough stamina to do an attack...
                    if ((stamina.GetCurrentValue() - myWeapon.meleeData.staminaCost) > 0)
                    {
                        // If the weapon is a chargeable weapon...
                        if (myWeapon.meleeData.isChargeable)
                        {
                            attackAnim = "meleeAttackHold";
                            playerMgmt.animMgmt.HandleMeleeAttackAnimation(attackInputHeld);
                        }
                        // If the weapon is a rapid attack weapon...
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
                playerMgmt.animMgmt.HandleMeleeAttackAnimation(attackInputHeld);
                // EVENTUALLY THIS WILL TRIGGER DIFFERENT UNARMED COMBOS \\\
            }

            inCombat = true;
            currentCombatTimer = combatTimer;
        }
    }

    void AttackReleased()
    {
        attackInputHeld = false;
        playerMgmt.animMgmt.HandleMeleeAttackAnimation(attackInputHeld);
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
        playerMgmt.vitalsMgmt.TakeDamage(stamina, myWeapon.meleeData.staminaCost);

        impactActivated = true;
    }

    /// <summary>
    /// Waits for the impactActivated bool to be triggered by an Animation Event. Grabs the entity's
    /// currently equipped weapon and creates an impact collider based on the weapons specs.
    /// </summary>
    void CheckMeleeAttack()
    {
        if (impactActivated)
        {
            MeleeWeapon equippedWeapon = playerMgmt.equipmentMgmt.currentlyEquippedWeapon as MeleeWeapon;
            if (equippedWeapon != null)
            {
                // Creates the collider on the weapon, the weapon then calls the Cmd
                equippedWeapon.CheckCreateImpactCollider(this);
            }
        }
    }

    // The player's equipped weapon calls this Cmd
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

    private void OnDisable()
    {
        playerMgmt.inputMgmt.attackEventStarted -= AttackPerformed;
        playerMgmt.inputMgmt.attackEventCancelled -= AttackReleased;
        playerMgmt.inputMgmt.rangedAttackEventStarted -= RangedAttackPerformed;
        playerMgmt.inputMgmt.rangedAttackEventCancelled -= RangedAttackReleased;
    }
}
