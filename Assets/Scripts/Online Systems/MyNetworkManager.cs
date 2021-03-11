using System.IO;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using System;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine;

public class MyNetworkManager : NetworkManager
{
    #region Public.
    /// <summary>
    /// All spawned local players, only kept on the server.
    /// </summary>
    public static Dictionary<NetworkConnection, NetworkIdentity> LocalPlayers = new Dictionary<NetworkConnection, NetworkIdentity>();
    #endregion

    [SerializeField] int minPlayers = 1;
    // THIS [Scene] TAG MAKES IT POSSIBLE TO DRAG A SCENE INTO FIELD TO CONVERT TO STRING
    [Scene] [SerializeField] string lobbyScene = string.Empty;

    public List<NetworkPlayerConnData> playerDataList { get; } = new List<NetworkPlayerConnData>();

    [SerializeField] private string notificationMessage = string.Empty;

    public static event Action OnClientConnected;
    public static event Action OnClientDisconnected;
    public static event Action OnServerStopped;
    public static event Action<NetworkPlayerConnData> OnPlayerAdded;
    

    public override void OnStartServer()
    {
        // ADDS ALL THE SPAWNABLE PREFABS TO THE NetworkManager THROUGH CODE SO 
        // WE DONT HAVE TO SET IT UP IN THE INSPECTOR.
        spawnPrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs").ToList();
    }


    public override void OnStartClient()
    {
        var spawnablePrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs");

        foreach (var prefab in spawnablePrefabs)
        {
            ClientScene.RegisterPrefab(prefab);
        }
    }


    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);

        OnClientConnected?.Invoke();
    }


    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);

        OnClientDisconnected?.Invoke();
    }


    public override void OnServerConnect(NetworkConnection conn)
    {
        if (numPlayers >= maxConnections)
        {
            conn.Disconnect();
            return;
        }

        // ONLY ALLOW NEW PLAYERS TO JOIN FROM THE MENU SCENE
        if (SceneManager.GetActiveScene().path != lobbyScene)
        {
            conn.Disconnect();
            return;
        }
    }


    public override void OnStopServer()
    {
        OnServerStopped?.Invoke();

        playerDataList.Clear();
        LocalPlayers.Clear();
    }


    public override void ServerChangeScene(string newSceneName)
    {
        // FROM LOBBY TO GAME
        if (SceneManager.GetActiveScene().path == lobbyScene && Path.GetFileName(newSceneName).StartsWith("Scene_ServerTest"))
        {
            // DO LOGIC THAT NEEDS TO HAPPEN WHEN LEAVING THE LOBBY SCENE
        }

        base.ServerChangeScene(newSceneName);
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        GameObject player = Instantiate(playerPrefab);

        LocalPlayers[conn] = player.GetComponent<NetworkIdentity>();
        NetworkServer.AddPlayerForConnection(conn, player);
    }


    public override void OnServerDisconnect(NetworkConnection conn)
    {
        // IF THE PLAYER'S CONNECTION ISN'T MISSING A networkIdentity
        // (WHICH IT NEVER SHOULD BE MISSING RIGHT??)
        if (conn.identity != null)
        {
            var player = conn.identity.GetComponent<NetworkPlayerConnData>();

            playerDataList.Remove(player);
        }

        base.OnServerDisconnect(conn);

        LocalPlayers.Remove(conn);
        base.OnServerDisconnect(conn);
    }


    public void StartGame()
    {
        if (SceneManager.GetActiveScene().path == lobbyScene)
        {
            // CHECK IF THE PLAYERS IN THE LOBBY ARE ALL READY???

            ServerChangeScene("Scene_ServerTest");
        }
    }


    [ContextMenu("Send Notification")]
    private void SendNotification()
    {
        NetworkServer.SendToAll(new Notification { content = notificationMessage });
    }
}
