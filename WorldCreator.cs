using UnityEngine;
using System.Collections.Generic;

public class WorldCreator : MonoBehaviour
{
    [System.Serializable]
    public class RoadPrefab
    {
        public GameObject prefab;
        public float spawnWeight;
    }

    public List<RoadPrefab> roadPrefabs;
    public float spawnDistance, despawnDistance, maxSpawnDistanceX, initialSpawnPositionX;
    private const float roadSpeed = 5f;
    public float speedIndex = 1;
    public Transform playerTransform;

    private List<GameObject> spawnedRoads = new List<GameObject>(); // Список созданных префабов дороги
    void Start()
    {
        SpawnInitialRoads();
    }
    void Update()
    {
        MoveRoads();
        CheckDespawnRoads();
    }
    private void FixedUpdate()
    {
        IncreaseSpeed();
    }
    void IncreaseSpeed()
    {
        if(speedIndex < 2)
        {
            speedIndex += 0.000075f;
        }
    }
    void SpawnInitialRoads()
    {
        float spawnPositionX = initialSpawnPositionX;
        while (spawnPositionX < despawnDistance)
        {
            GameObject randomRoadPrefab = GetRandomRoadPrefab();
            GameObject newRoad = Instantiate(randomRoadPrefab, new Vector3(spawnPositionX, 0f, 0f), Quaternion.identity);
            spawnedRoads.Add(newRoad);
            spawnPositionX += spawnDistance;
        }
    }
    void MoveRoads()
    {
        foreach (GameObject road in spawnedRoads)
        {
            road.transform.Translate(Vector3.left * roadSpeed * speedIndex * Time.deltaTime);
        }
        if (spawnedRoads.Count > 0 && spawnedRoads[spawnedRoads.Count - 1].transform.position.x - playerTransform.position.x < maxSpawnDistanceX)
        {
            SpawnNewRoad();
        }
    }
    void SpawnNewRoad()
    {
        GameObject randomRoadPrefab = GetRandomRoadPrefab();
        float newX = spawnedRoads[spawnedRoads.Count - 1].transform.position.x + spawnDistance;
        GameObject newRoad = Instantiate(randomRoadPrefab, new Vector3(newX, 0f, 0f), Quaternion.identity);
        spawnedRoads.Add(newRoad);
    }
    void CheckDespawnRoads()
    {
        if (spawnedRoads.Count > 0 && spawnedRoads[0].transform.position.x < -despawnDistance)
        {
            GameObject despawnedRoad = spawnedRoads[0];
            spawnedRoads.Remove(despawnedRoad);
            Destroy(despawnedRoad);
        }
    }
    GameObject GetRandomRoadPrefab()
    {
        float totalWeight = 0f;
        foreach (RoadPrefab roadPrefab in roadPrefabs)
        {
            totalWeight += roadPrefab.spawnWeight;
        }

        float randomValue = Random.Range(0f, totalWeight);
        float weightSum = 0f;

        foreach (RoadPrefab roadPrefab in roadPrefabs)
        {
            weightSum += roadPrefab.spawnWeight;
            if (randomValue <= weightSum)
            {
                return roadPrefab.prefab;
            }
        }

        return roadPrefabs[roadPrefabs.Count - 1].prefab;
    }
}
