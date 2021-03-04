 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CombatManager : NetworkBehaviour
{
    public LayerMask whatIsDamageable;

    [Header("Component Reference")]
    [SerializeField] PlayerManager playerMgmt;
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

        inputMgmt.Controls.Player.Attack.performed += ctx => CheckAttack();
        inputMgmt.Controls.Combat.Shoot.performed += ctx => CheckRangedAttack();
    }


    [Client]
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

    // USING THIS TEMPORARILY FOR TESTING. WILL IMPLEMENT INTO CheckAttack SOMEHOW
    [Client]
    public void CheckRangedAttack()
    {
        if (!base.hasAuthority) { return; }
        if (!ShotTimeMet()) { return; }

        if (canRecieveAttackInput)
        {
            attackInputRecieved = true;
            canRecieveAttackInput = false;
            inCombat = true;
        }
        else
        {
            return;
        }

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
        // If player is locked into an "interacting" animation then don't let this happen.
        if(playerMgmt.isInteracting) { return; }


        // CHECK IF ARMED OR UNARMED //

        //if(canRecieveAttackInput)
        //{
        //    attackInputRecieved = true;
        //    canRecieveAttackInput = false;
        //    inCombat = true;
        //}
        //else
        //{
        //    return;
        //}

        inputMgmt.RecieveAttackInput();
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

    public void InvertAttackBool()
    {
        canRecieveAttackInput = !canRecieveAttackInput;

        //if(!canRecieveInput)
        //{
        //    canRecieveInput = true;
        //}
        //else
        //{
        //    canRecieveInput = false;
        //}
    }

    /// <summary>
    /// Called by an Animation Event from the player checks an int
    /// to determine the means of the attack
    /// </summary>
    /// <param name="handInt"></param>
    public void ActivateImpact(int handInt)
    {
        // IF UNARMED
        if (handInt == 0)
        {
            impactOrigin = leftHand.transform;
            impactEnd = leftHand.transform;
            impactRadius = .5f;
        }
        else if (handInt == 1)
        {
            // RIGHT HAND
        }
        else if (handInt == 2)
        {
            // LEFT OR RIGHT FOOT?
            // SINGLE FEET ATTACK POS?
        }
        else if (handInt == 3)
        {
            // IF WEAPON EQUIPPED
            // impactActivate = true;
        }

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
