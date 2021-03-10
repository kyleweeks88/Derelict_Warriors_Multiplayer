using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Enemy : CharacterStats
{
    public override void Death()
    {
        base.Death();
        ServerDeath();
    }

    [Server]
    public virtual void ServerDeath()
    {
        Debug.Log(charName + " has died!");
        RpcDeath();
        Object.Destroy(this.gameObject);
    }

    [ClientRpc]
    void RpcDeath()
    {
        if (base.hasAuthority) { return; }

        Debug.Log(charName + " has died!");
        Object.Destroy(this.gameObject);
    }
}
