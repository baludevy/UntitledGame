using System;
using UnityEngine;

public class PrefabManager : MonoBehaviour
{
    public static PrefabManager Instance;

    public GameObject hitEffect;
    public GameObject damageMarker;
    public GameObject critMarker;
    public GameObject audioPrefab;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(this);
    }

    public void SpawnDamageMarker(Vector3 pos, Quaternion rot, float damage, Color color)
    {
        GameObject marker = Instantiate(damageMarker, pos, rot);

        marker.GetComponent<DamageMarker>().ShowDamage(damage, color);
    }

    public void SpawnSparkles(Vector3 pos, Quaternion rot, Color color)
    {
        ParticleSystem ps =
            Instantiate(hitEffect, pos, rot)
                .GetComponent<ParticleSystem>();

        ParticleSystem.MainModule main = ps.main;
        main.startColor = color;
        ps.transform.GetComponentInChildren<SpriteRenderer>().color = color;
    }

    public void SpawnCritMarker(Vector3 pos, Quaternion rot)
    {
        GameObject marker = Instantiate(critMarker, pos, rot);
    }
}