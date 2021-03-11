using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

// TEST
public class EnemySpawner : NetworkBehaviour
{
    float nextSpawnTime = 1f;
    bool spawnEnemy = true;

    [SerializeField] float spawnDelay = 2f;
    [SerializeField] Transform[] spawnPoints;
    [SerializeField] NpcStats[] enemies;

    public override void OnStartServer()
    {
        nextSpawnTime = spawnDelay;
        spawnEnemy = true;
        base.OnStartServer();
    }

    void Update()
    {
        if (isServer == false)
        return;

        if (ShouldSpawn() && spawnEnemy)
            Spawn();
    }

    void Spawn()
    {
        // TAKES THE CURRENT TIME AND ADDS THE SPAWN DELAY TIME
        nextSpawnTime = Time.time + spawnDelay;
        Transform spawnPoint = ChooseSpawnPoint();
        NpcStats enemyPrefab = ChooseEnemy();
        NpcStats enemyInstance = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        NetworkServer.Spawn(enemyInstance.gameObject);
    }


    Transform ChooseSpawnPoint()
    {
        int randomIndex = UnityEngine.Random.Range(0, spawnPoints.Length);
        var spawnPoint = spawnPoints[randomIndex];
        return spawnPoint;
    }

    NpcStats ChooseEnemy()
    {
        int randomIndex = UnityEngine.Random.Range(0, enemies.Length);
        var enemy = enemies[randomIndex];
        return enemy;
    }

    bool ShouldSpawn()
    {
        return Time.time >= nextSpawnTime;
    }
}
