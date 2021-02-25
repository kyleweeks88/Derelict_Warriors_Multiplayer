using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class OwnerSpawnAnnouncer : NetworkBehaviour
{
    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        AnnounceSpawned();
    }

    private void AnnounceSpawned()
    {
        ClientInstance clientInstance = ClientInstance.ReturnClientInstance();
        clientInstance.InvokeCharacterSpawned(gameObject);
    }
}
