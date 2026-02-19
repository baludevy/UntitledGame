using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyWaveSpawner : MonoBehaviour {
    [SerializeField] private List<GameObject> enemyPrefabs = new();

    [SerializeField] private int startWave = 1;
    [SerializeField] private int baseEnemiesPerWave = 10;
    [SerializeField] private int enemiesAddedPerWave = 5;
    [SerializeField] private float timeBetweenWaves = 5f;

    [SerializeField] private Transform areaCenter;
    [SerializeField] private float spawnRadius = 25f;
    [SerializeField] private float spawnY = 0f;

    [SerializeField] private float checkRadius = 0.8f;
    [SerializeField] private LayerMask obstructionMask;
    [SerializeField] private int maxAttemptsPerEnemy = 30;

    [SerializeField] private TMP_Text waveText;

    private int wave;
    private bool running;

    private void Start() {
        wave = Mathf.Max(1, startWave);
        StartSpawning();
    }

    public void StartSpawning() {
        if (running) return;
        running = true;
        StartCoroutine(WaveLoop());
    }

    public void StopSpawning() {
        running = false;
        StopAllCoroutines();
    }

    private IEnumerator WaveLoop() {
        while (running) {
            if (waveText != null) waveText.text = $"Wave {wave}";
            int count = Mathf.Max(0, baseEnemiesPerWave + (wave - 1) * enemiesAddedPerWave);
            yield return StartCoroutine(SpawnWave(count));
            wave++;
            if (timeBetweenWaves > 0f) yield return new WaitForSeconds(timeBetweenWaves);
            else yield return null;
        }
    }

    private IEnumerator SpawnWave(int count) {
        if (enemyPrefabs == null || enemyPrefabs.Count == 0) yield break;

        Transform center = areaCenter != null ? areaCenter : transform;

        for (int i = 0; i < count; i++) {
            if (!running) yield break;

            if (TryFindSpawnPoint(center.position, out Vector3 pos)) {
                var prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
                Instantiate(prefab, pos, Quaternion.identity);
            }

            yield return null;
        }
    }

    private bool TryFindSpawnPoint(Vector3 center, out Vector3 pos) {
        for (int attempt = 0; attempt < maxAttemptsPerEnemy; attempt++) {
            Vector2 r = Random.insideUnitCircle * spawnRadius;
            Vector3 candidate = new Vector3(center.x + r.x, spawnY, center.z + r.y);

            if (!Physics.CheckSphere(candidate, checkRadius, obstructionMask, QueryTriggerInteraction.Ignore)) {
                pos = candidate;
                return true;
            }
        }

        pos = default;
        return false;
    }
}