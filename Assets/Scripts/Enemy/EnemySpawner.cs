using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Collider spawnPlaneCollider;

    public static EnemySpawner Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartSpawn(float duration, int enemiesPerSession)
    {
        StartCoroutine(SpawnEnemies(duration, enemiesPerSession));
    }

    IEnumerator SpawnEnemies(float duration, int enemiesPerSession)
    {
        if (enemiesPerSession <= 0)
        {
            yield break;
        }
        
        float timeBetweenSpawns = duration / enemiesPerSession;

        for (int i = 0; i < enemiesPerSession; i++)
        {
            if (i > 0)
            {
                yield return new WaitForSeconds(timeBetweenSpawns);
            }
            
            Bounds bounds = spawnPlaneCollider.bounds;

            float randomX = Random.Range(bounds.min.x, bounds.max.x);
            float randomZ = Random.Range(bounds.min.z, bounds.max.z);
        
            Vector3 randomPosition = new Vector3(randomX, bounds.max.y + 0.1f, randomZ);

            if (randomPosition != Vector3.zero)
            {
                Instantiate(enemyPrefab, randomPosition, Quaternion.identity);
            }
        }
    }
    
    public void StopSpawn()
    {
        StopAllCoroutines();
    }
}