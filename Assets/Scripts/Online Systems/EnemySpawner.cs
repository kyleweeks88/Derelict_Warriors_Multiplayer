using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

// TEST
public class EnemySpawner : NetworkBehaviour
{
    float nextSpawnTime;
    bool spawnEnemy = true;

    [SerializeField] float spawnDelay = 2f;
    [SerializeField] Transform[] spawnPoints;
    [SerializeField] Enemy[] enemies;

    public override void OnStartServer()
    {
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
        Enemy enemyPrefab = ChooseEnemy();
        Enemy enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        NetworkServer.Spawn(enemy.gameObject);
    }


    Transform ChooseSpawnPoint()
    {
        int randomIndex = UnityEngine.Random.Range(0, spawnPoints.Length);
        var spawnPoint = spawnPoints[randomIndex];
        return spawnPoint;
    }

    Enemy ChooseEnemy()
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
