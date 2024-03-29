using System.Collections.Generic;
using UnityEngine;
using Invector.vCharacterController.AI;

public class EnemyRespawn : MonoBehaviour
{
    public List<GameObject> enemiesPrefabs;
    public List<Collider> spawnPoints;
    public float minRespawnTime, maxRespawnTime;
    public int enemiesCount;
    public VampirismController vampirismController;

    private bool isEnemyDead = true;
    private bool isFirstStart = true;
    private List<GameObject> enemiesOnScene = new List<GameObject>();

    private void Awake()
    {
        DisableSpawnPointMarkers();
    }

    private void Start()
    {
        CheckGameStarted();
    }

    private void DisableSpawnPointMarkers()
    {
        foreach (var spawnPoint in spawnPoints)
        {
            spawnPoint.GetComponent<MeshRenderer>().enabled = false;
        }
    }

    private void CheckGameStarted()
    {
        if (vampirismController.isGameStarted && isFirstStart)
        {
            SpawnInitialEnemies();
            isFirstStart = false;
        }
    }

    private void SpawnInitialEnemies()
    {
        for (int i = 0; i < enemiesCount; i++)
        {
            SpawnEnemy();
        }
    }

    private void Update()
    {
        CheckGameStarted();

        if (!isFirstStart && vampirismController.isGameStarted)
        {
            CheckDeadEnemies();
        }
    }

    private void CheckDeadEnemies()
    {
        for (int i = 0; i < enemiesOnScene.Count; i++)
        {
            var enemyAI = enemiesOnScene[i].GetComponent<vControlAIShooter>();
            if (enemyAI != null && enemyAI.isDead && isEnemyDead)
            {
                isEnemyDead = false;
                float respawnDelay = Random.Range(minRespawnTime, maxRespawnTime);
                Destroy(enemiesOnScene[i], respawnDelay);
                enemiesOnScene.RemoveAt(i);
                Invoke(nameof(SpawnEnemy), respawnDelay + 0.1f);
            }
        }
    }

    private void SpawnEnemy()
    {
        if (enemiesPrefabs.Count == 0 || spawnPoints.Count == 0)
            return;

        Collider spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
        if (!spawnPoint.enabled)
            return;

        Vector3 spawnPosition = GetRandomSpawnPosition(spawnPoint);
        enemiesOnScene.Add(Instantiate(enemiesPrefabs[Random.Range(0, enemiesPrefabs.Count)], spawnPosition, Quaternion.identity));
        isEnemyDead = true;
    }

    private Vector3 GetRandomSpawnPosition(Collider spawnPoint)
    {
        float x = Random.Range(-spawnPoint.bounds.extents.x, spawnPoint.bounds.extents.x) + spawnPoint.bounds.center.x;
        float y = spawnPoint.bounds.center.y;
        float z = Random.Range(-spawnPoint.bounds.extents.z, spawnPoint.bounds.extents.z) + spawnPoint.bounds.center.z;
        return new Vector3(x, y, z);
    }

    public void RemoveAllEnemies()
    {
        foreach (var enemy in enemiesOnScene)
        {
            Destroy(enemy);
        }
        enemiesOnScene.Clear();
    }
}
