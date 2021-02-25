using System.Collections;
using System.Collections.Generic;
using Mirror;
using System.Linq;
using UnityEngine;

public class PlayerSpawnSystem : NetworkBehaviour
{
    [SerializeField] GameObject playerPrefab = null;

    // LIST IS STATIC SO EVERY INSTANCE OF THIS LIST SHARES THE SAME TRANSFORMS
    static List<Transform> spawnPoints = new List<Transform>();
    // USED TO INCREMENT BASED ON PLAYER'S THAT HAVE SPAWNED
    int nextIndex = 0;

    public static void AddSpawnPoint(Transform transform)
    {
        spawnPoints.Add(transform);

        // MAKES SURE THE ORDER IS CORRECT? 
        // ...RESEARCH THIS LINE OF CODE MORE
        spawnPoints = spawnPoints.OrderBy(x => x.GetSiblingIndex()).ToList();
    }


    public static void RemoveSpawnPoint(Transform transform) => spawnPoints.Remove(transform);


    // SUBSCRIBES TO OnServerReadied TO CALL SpawnPlayer WHEN THE SERVER IS READY
    public override void OnStartServer()
    {
        DontDestroyOnLoad(gameObject);
        playerPrefab = MyNetworkManager.singleton.playerPrefab;

        //MyNetworkManager.OnServerReadied += SpawnPlayer;
        Debug.Log("SpawnPlayer subscribed");
    }


    //[ServerCallback]
    //private void OnDestroy() => MyNetworkManager.OnServerReadied -= SpawnPlayer;


    [Server]
    // TAKES IN A conn PASSED BY THE OnServerReadied EVENT ON NetworkManager_Game
    public void SpawnPlayer(NetworkConnection conn)
    {
        //Transform spawnPoint = spawnPoints.ElementAtOrDefault(nextIndex);

        //if(spawnPoint == null)
        //{
        //    Debug.LogError($"Missing spawn point for player {nextIndex}");
        //    return;
        //}
        
        // SPAWN IN THE PLAYER BASED ON THE ASSIGNED PREFAB
        GameObject playerInstance = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        // WE PASS IN THE conn BECAUSE THE PLAYER OBJECT BEING SPAWNED IN BELONGS TO THE conn BEING PASSED INTO THIS METHOD
        NetworkServer.Spawn(playerInstance, conn);
        Debug.Log("Player spawned!");

        // INCREMENT THE INDEX FOR THE NEXT PLAYER 
        nextIndex++;
    }
}
