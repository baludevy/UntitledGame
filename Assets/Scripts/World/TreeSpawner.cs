using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class TreeSpawner : MonoBehaviour
{
    public GameObject[] treePrefabs;
    public int treeCount = 100;
    
    public Collider spawnPlaneCollider;

    public static TreeSpawner Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        SpawnTrees();
    }

    public void SpawnTrees()
    {
        if (treePrefabs == null || treePrefabs.Length == 0 || spawnPlaneCollider == null)
            return;

        Bounds bounds = spawnPlaneCollider.bounds;

        for (int i = 0; i < treeCount; i++)
        {
            float randomX = Random.Range(bounds.min.x, bounds.max.x);
            float randomZ = Random.Range(bounds.min.z, bounds.max.z);
            Vector3 randomPosition = new Vector3(randomX, bounds.max.y, randomZ);

            GameObject prefab = treePrefabs[Random.Range(0, treePrefabs.Length)];
            Instantiate(prefab, randomPosition, Quaternion.identity);
        }
    }
}