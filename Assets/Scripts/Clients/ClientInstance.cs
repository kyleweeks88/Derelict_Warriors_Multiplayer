using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientInstance : NetworkBehaviour
{
    /// <summary>
    /// Singleton reference to the client instance. Referenced value will be
    /// for LocalPlayer
    /// </summary>
    public static ClientInstance Instance;

    /// <summary>
    /// Dispatched on the owning player when a character is spawned for them.
    /// </summary>
    public static Action<GameObject> OnOwnerCharacterSpawned;

    /// <summary>
    /// Prefab for the player.
    /// </summary>
    [Tooltip("Prefab for the player.")]
    [SerializeField] NetworkIdentity playerPrefab = null;

    /// <summary>
    /// Currently spawned character for the local player.
    /// </summary>
    private GameObject currentCharacter = null;


    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        Instance = this;
        CmdRequestSpawn();
    }

    /// <summary>
    /// Request a spawn for character.
    /// </summary>
    [Command]
    private void CmdRequestSpawn()
    {
        NetworkSpawnPlayer();
    }

    /// <summary>
    /// Spawns a character prefab for the player on the server
    /// </summary>
    [Server]
    private void NetworkSpawnPlayer()
    {
        Transform startPos = NetworkManager.singleton.GetStartPosition();
        GameObject playerObjectInstance = Instantiate(playerPrefab.gameObject, 
            startPos.position, startPos.rotation);
        NetworkServer.Spawn(playerObjectInstance, base.connectionToClient);
    }

    public void InvokeCharacterSpawned(GameObject go)
    {
        currentCharacter = go;
        SetName(PlayerNameInput.DisplayName);
        InitializeVitals();
        OnOwnerCharacterSpawned?.Invoke(go);
    }

    /// <summary>
    /// Sets the name by the Client the has authority and if there is a currentCharacter
    /// prefab spawned it will change the synchronized name on that palyer obj. 
    /// </summary>
    /// <param name="name"></param>
    public void SetName(string name)
    {
        if (currentCharacter != null)
        {
            PlayerName playerName = currentCharacter.GetComponent<PlayerName>();
            playerName.SetName(name);
        }
    }

    public void InitializeVitals()
    {
        if(currentCharacter != null)
        {
            HealthManager healthMgmt = currentCharacter.GetComponent<HealthManager>();
            healthMgmt.SetVital(healthMgmt.maxVital);

            StaminaManager staminaMgmt = currentCharacter.GetComponent<StaminaManager>();
            staminaMgmt.SetVital(staminaMgmt.maxVital);
        }
    }

    #region Function for retrieving your local ClientInstance
    /// <summary>
    /// Used to access the ClientInstance specific to the NetworkConnection that
    /// is passed in the params. 
    /// </summary>
    /// <param name="conn"></param>
    /// <returns></returns>
    public static ClientInstance ReturnClientInstance(NetworkConnection conn = null)
    {
        /* If server and connection isn't null.
         * When trying to access as server connection
         * will always contain a value. But if client it will be null. */
        if (NetworkServer.active && conn != null)
        {
            NetworkIdentity localPlayer;
            if (MyNetworkManager.LocalPlayers.TryGetValue(conn, out localPlayer))
                return localPlayer.GetComponent<ClientInstance>();
            else
                return null;
        }
        // If not server or connection is null, then client is.
        else
        {
            return Instance;
        }
    }
    #endregion
}
