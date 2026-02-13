using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public static EnemyController Instance;
    private readonly List<IEnemy> enemies = new();

    [SerializeField] private int batchSize = 25;
    private int currentBatch;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void RegisterEnemy(IEnemy enemy)
    {
        if (!enemies.Contains(enemy))
            enemies.Add(enemy);
    }

    public void UnregisterEnemy(IEnemy enemy)
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
    }
}