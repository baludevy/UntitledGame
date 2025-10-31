using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance;

    private readonly Dictionary<GameObject, Queue<GameObject>> pools = new();

    private void Awake()
    {
        Instance = this;
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
            return obj;
        }

        return Instantiate(prefab, pos, rot);
    }

    public void Return(GameObject obj, GameObject prefab, float delay)
    {
        StartCoroutine(ReturnRoutine(obj, prefab, delay));
    }

    private IEnumerator ReturnRoutine(GameObject obj, GameObject prefab, float delay)
    {
        yield return new WaitForSeconds(delay);
        obj.SetActive(false);
        pools[prefab].Enqueue(obj);
    }
}