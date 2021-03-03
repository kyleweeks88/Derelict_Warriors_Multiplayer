using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;

// I think this script will handle character leveling, dying, all the attributes/stats
// and the way they modify or mitigate incoming/outgoing damage.
public class CharacterStats : NetworkBehaviour, IKillable
{
    [Header("Settings")]
    public string charName;
    public float baseAttackDamage;

    #region Death!!!
    [Client]
    public virtual void Death()
    {
        if(!base.hasAuthority){return;}
        
        Debug.Log(charName + " has died!");
        this.transform.position = Vector3.zero;
        CmdDeath();
    }

    [Command]
    public virtual void CmdDeath()
    {
        Debug.Log(charName + " has died!");
        this.transform.position = Vector3.zero;
        RpcDeath();
    }

    [ClientRpc]
    void RpcDeath()
    {
        if(base.hasAuthority){return;}

        Debug.Log(charName + " has died!");
        this.transform.position = Vector3.zero;
    }
    #endregion
}
