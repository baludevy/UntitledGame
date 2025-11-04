using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public static EnemyController Instance;
    private readonly List<BaseEnemy> enemies = new();

    [SerializeField] private int batchSize = 25;
    private int currentBatch;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void RegisterEnemy(BaseEnemy enemy)
    {
        if (!enemies.Contains(enemy))
            enemies.Add(enemy);
    }

    public void UnregisterEnemy(BaseEnemy enemy)
    {
        if (enemies.Contains(enemy))
            enemies.Remove(enemy);
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i] != null)
                enemies[i].Tick();
        }

        TimeSliced();
    }

    private void TimeSliced()
    {
        if (enemies.Count == 0) return;
        int startIndex = currentBatch * batchSize;
        int endIndex = Mathf.Min(startIndex + batchSize, enemies.Count);

        for (int i = startIndex; i < endIndex; i++)
        {
            if (enemies[i] != null)
                enemies[i].DetectWall();
        }

        currentBatch++;
        if (startIndex >= enemies.Count - 1)
            currentBatch = 0;
    }
}