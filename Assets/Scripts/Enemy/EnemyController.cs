using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public static EnemyController Instance;
    private readonly List<BaseEnemy> enemies = new();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
        }
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
    }
}