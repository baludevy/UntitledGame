using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance;

    private readonly Dictionary<GameObject, Queue<GameObject>> pools = new();
    private readonly Dictionary<GameObject, float> lastUsed = new();
    [SerializeField] private float cleanupInterval = 30f;
    [SerializeField] private float idleLifetime = 60f;

    private void Awake()
    {
        Instance = this;
        StartCoroutine(CleanupRoutine());
    }

    public GameObject Get(GameObject prefab, Vector3 pos, Quaternion rot)
    {
        if (!pools.ContainsKey(prefab))
            pools[prefab] = new Queue<GameObject>();

        if (pools[prefab].Count > 0)
        {
            GameObject obj = pools[prefab].Dequeue();
            obj.transform.SetPositionAndRotation(pos, rot);
            obj.SetActive(true);
            lastUsed[obj] = Time.time;
            return obj;
        }

        GameObject newObj = Instantiate(prefab, pos, rot);
        lastUsed[newObj] = Time.time;
        return newObj;
    }

    public void Return(GameObject obj, GameObject prefab, float delay)
    {
        StartCoroutine(ReturnRoutine(obj, prefab, delay));
    }

    private IEnumerator ReturnRoutine(GameObject obj, GameObject prefab, float delay)
    {
        yield return new WaitForSeconds(delay);
        obj.SetActive(false);
        lastUsed[obj] = Time.time;
        pools[prefab].Enqueue(obj);
    }

    private IEnumerator CleanupRoutine()
    {
        WaitForSeconds wait = new(cleanupInterval);
        while (true)
        {
            yield return wait;
            float now = Time.time;
            foreach (var pool in pools.Values)
            {
                int count = pool.Count;
                for (int i = 0; i < count; i++)
                {
                    GameObject obj = pool.Peek();
                    if (obj == null || !lastUsed.ContainsKey(obj) || now - lastUsed[obj] > idleLifetime)
                    {
                        GameObject old = pool.Dequeue();
                        if (old != null) Destroy(old);
                        lastUsed.Remove(obj);
                    }
                    else
                    {
                        pool.Enqueue(pool.Dequeue());
                    }
                }
            }
        }
    }
}