using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class MineableObject : MonoBehaviour
{
    public string mineableName;
    public int health;
    public int minDropAmount;
    public int maxDropAmount;
    public GameObject dropPrefab;

    public void Mine(int damage)
    {
        health -= damage;
        StopAllCoroutines();
        StartCoroutine(HitAnimation());

        if (health <= 0)
        {
            Drop();
        }
    }

    IEnumerator HitAnimation()
    {
        Vector3 start = transform.localScale;
        Vector3 target = start * 0.85f;

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / 0.2f;
            float eased = t < 0.5f
                ? 4f * t * t * t
                : 1f - Mathf.Pow(-2f * t + 2f, 3f) / 2f;
            transform.localScale = Vector3.Lerp(start, target, eased);
            yield return null;
        }

        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / 0.2f;
            float eased = t < 0.5f
                ? 4f * t * t * t
                : 1f - Mathf.Pow(-2f * t + 2f, 3f) / 2f;
            transform.localScale = Vector3.Lerp(target, start, eased);
            yield return null;
        }
    }

    private void Drop()
    {
        DroppedItem dropped = Instantiate(dropPrefab, transform.position, Quaternion.identity)
            .GetComponent<DroppedItem>();
        dropped.Initialize(new ItemInstance(dropped.itemData, UnityEngine.Random.Range(minDropAmount, maxDropAmount)));
            
        Destroy(gameObject);
    }
}