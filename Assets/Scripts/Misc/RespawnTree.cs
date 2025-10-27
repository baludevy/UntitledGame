using System;
using UnityEngine;

public class RespawnTree : MonoBehaviour
{
    private Vector3 position;
    public GameObject treePrefab;
    private bool isSpawning;

    private void Start()
    {
        position = transform.GetChild(1).position;
    }

    private void Update()
    {
        if (transform.childCount == 1 && !isSpawning)
        {
            isSpawning = true;
            Invoke(nameof(SpawnTree), 0.2f);
        }
    }

    private void SpawnTree()
    {
        Instantiate(treePrefab, position, Quaternion.identity, transform);
        isSpawning = false;
    }
}