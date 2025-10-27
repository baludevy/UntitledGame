using System;
using UnityEngine;

public class PrefabManager : MonoBehaviour
{
    public static PrefabManager Instance;

    public GameObject hitEffect;
    public GameObject damageMarker;
    public GameObject audioPrefab;
    public GameObject droppedResourcePrefab;

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

    public void SpawnDamageMarker(Vector3 pos, Quaternion rot, float damage, bool crit)
    {
        GameObject marker = Instantiate(damageMarker, pos, rot);

        marker.GetComponent<DamageMarker>().ShowDamage(damage, crit);
    }

    public void SpawnSparkles(Vector3 pos, Quaternion rot, bool crit)
    {
        ParticleSystem ps =
            Instantiate(hitEffect, pos, rot)
                .GetComponent<ParticleSystem>();

        if (crit)
        {
            ParticleSystem.MainModule main = ps.main;
            main.startColor = Color.yellow;
            ps.transform.GetComponentInChildren<SpriteRenderer>().color = Color.yellow;
        }
    }
}