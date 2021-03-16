using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerStats : CharacterStats
{
    PlayerName playerName;

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();

        playerName = GetComponent<PlayerName>();
        base.charName = playerName.synchronizedName;
    }

    public override void Death()
    {
        base.Death();
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
        if (base.hasAuthority) { return; }

        Debug.Log(charName + " has died!");
        this.transform.position = Vector3.zero;
    }
}

