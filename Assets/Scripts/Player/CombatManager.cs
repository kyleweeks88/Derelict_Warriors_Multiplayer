﻿ using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CombatManager : NetworkBehaviour
{
    public string attackAnim = string.Empty;
    public LayerMask whatIsDamageable;

    [Header("Component Reference")]
    [SerializeField] CharacterStats myStats = null;
    [SerializeField] GameObject leftHand;
    [SerializeField] GameObject rightHand;
    [SerializeField] Animator myAnimator;
    [SerializeField] NetworkAnimator myNetworkAnimator = null;
    InputManager inputMgmt;
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


    public override void OnStartAuthority()
    {
        enabled = true;
        inputMgmt = GetComponent<InputManager>();
    }


    [ClientCallback]
    private void Update()
    {
        if (!hasAuthority) { return; }
        
        myAnimator.SetBool("inCombat", inCombat);

        if (impactActivated)
        {
            CheckCreateImpactCollider(impactOrigin.position,
                impactEnd.position, impactRadius, whatIsDamageable);
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

        // Plays the appropriate attack animation
        myNetworkAnimator.SetTrigger(attackAnim);

        inCombat = true;
        CmdAttack(transform.position);
    }

    /// <summary>
    /// Adjust's the players position to a clamped distance based on the server
    /// from where the player attacked to the actualy position on the server
    /// </summary>
    /// <param name="pos"></param>
    [Command]
    void CmdAttack(Vector3 pos)
    {
        float maxPosOffset = 1;
        if(Vector3.Distance(pos, transform.position) > maxPosOffset)
        {
            Vector3 posDir = pos - transform.position;
            pos = transform.position + (posDir * maxPosOffset);
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

        // You have no weapon equipped
        // Left hand
        if (impactID == 1)
        {
            impactOrigin = leftHand.transform;
            impactEnd = leftHand.transform;
            impactRadius = 0.5f;
        }
        // Right hand
        else if (impactID == 2)
        {
            impactOrigin = rightHand.transform;
            impactEnd = rightHand.transform;
            impactRadius = 0.5f;
        }
        //else if (impactID == 3)
        //{
        //    // LEFT OR RIGHT FOOT?
        //    // SINGLE FEET ATTACK POS?
        //}

        // You have a weapon equipped
        //if(impactID == 0)
        //{
        //    impactOrigin = weaponMgmt.currentWeapon.startPos;
        //    impactEnd = weaponMgmt.currentWeapon.endPos;
        //    impactRadius = weaponMgmt.currentWeapon.impactRadius;
        //}

        impactActivated = true;
    }

    void CheckCreateImpactCollider(Vector3 origin, Vector3 end, float radius, LayerMask whatIsDamageable)
    {
        Collider[] verifiedImpactCol = Physics.OverlapCapsule(origin, end, radius, whatIsDamageable);
        foreach (Collider hit in verifiedImpactCol)
        {
            if (hit.gameObject.GetComponentInParent<IHaveHealth>() != null)
            {
                // CHECK THE DISTANCE ON THE SERVER BETWEEN THIS CLIENT'S ATTACKER AND THE 
                // colPos OF THE HIT OBJECT TO VERIFY DISTANCE.
                float colLength = Vector3.Distance(origin, end);

                NetworkIdentity objIdentity = hit.gameObject.GetComponentInParent<NetworkIdentity>();
                // PASS THE OBJECT HIT INTO A SERVER CHECK AND COMMAND
                CmdCreateImpactCollider(objIdentity, colLength, radius);
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

    [Command]
    void CmdCreateImpactCollider(NetworkIdentity hitObj, float colLength, float colRadius)
    {
        // ALSO CHECK THE impactRadius AGAINST A CLAMPED RADIUS TO MAKE SURE CLIENT
        // ISN'T HACKING impactRadius SIZE.
        if (colRadius > 3f) { return; }

        // CHECK LENGTH OF CAPSULE COLLIDER FOR HACKING
        if(colLength > 5f) { return; }

        // CHECK DISTANCE FROM CLIENT TO HIT OBJECT
        float distToHitObject = Vector3.Distance(this.transform.position, hitObj.transform.position);
        if (distToHitObject > 5f) { return; }

        CheckProcessAttack(hitObj.gameObject); 
    }

    void CheckProcessAttack(GameObject target)
    {
        IHaveHealth entity = target.GetComponentInParent<IHaveHealth>();
        entity.TakeDamage(myStats.baseAttackDamage);
    }
    #endregion
}
