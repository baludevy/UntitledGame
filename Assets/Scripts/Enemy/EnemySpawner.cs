using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float minSpawnDistance = 10f;
    public float maxSpawnDistance = 20f;

    public static EnemySpawner Instance;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void StartSpawn(float duration, int enemiesPerSession)
    {
        StartCoroutine(SpawnEnemies(duration, enemiesPerSession));
    }

    IEnumerator SpawnEnemies(float duration, int enemiesPerSession)
    {
        if (enemiesPerSession <= 0) yield break;
        
        float timeBetweenSpawns = duration / enemiesPerSession;

        for (int i = 0; i < enemiesPerSession; i++)
        {
            if (i > 0) yield return new WaitForSeconds(timeBetweenSpawns);

            if (PlayerMovement.Instance == null) yield break;

            Vector3 center = PlayerMovement.Instance.transform.position;

            float angle = Random.Range(0f, 360f);
            float distance = Random.Range(minSpawnDistance, maxSpawnDistance);
            Vector3 offset = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0, Mathf.Sin(angle * Mathf.Deg2Rad)) * distance;

            Vector3 spawnPos = center + offset;
            spawnPos.y += 0.1f;

            Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        }
    }

    public void StopSpawn()
    {
        StopAllCoroutines();
    }
}