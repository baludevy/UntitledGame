using UnityEngine;

public class TestEnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private int enemyCount = 100;
    [SerializeField] private float spacing = 2f;

    private void Start()
    {
        int gridSize = Mathf.CeilToInt(Mathf.Sqrt(enemyCount));
        Vector3 startPos = transform.position;

        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {
                int index = x * gridSize + z;
                if (index >= enemyCount) return;

                Vector3 spawnPos = startPos + new Vector3(x * spacing, 0, z * spacing);
                Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
            }
        }
    }
}