using System;
using UnityEngine;

public class PrefabManager : MonoBehaviour
{
    public static PrefabManager Instance;

    public GameObject hitEffect;
    public GameObject damageMarker;
    public GameObject textMarker;
    public GameObject audioPrefab;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(this);
    }

    public void SpawnDamageMarker(Vector3 pos, Quaternion rot, float damage, Color color)
    {
        GameObject marker = ObjectPool.Instance.Get(damageMarker, pos, rot);
        
        HitMarker markerComp = marker.GetComponent<HitMarker>();
        markerComp.ShowDamage(damage, color);
        
        ObjectPool.Instance.Return(marker, damageMarker, 1.5f);
    }

    public void SpawnSparkles(Vector3 pos, Quaternion rot, Color color)
    {
        GameObject sparkle = ObjectPool.Instance.Get(hitEffect, pos, rot);
        ParticleSystem ps = sparkle.GetComponent<ParticleSystem>();
        ParticleSystem.MainModule main = ps.main;
        main.startColor = color;

        SpriteRenderer sprite = sparkle.GetComponentInChildren<SpriteRenderer>();
        if (sprite != null)
            sprite.color = color;

        ps.Play();
        ObjectPool.Instance.Return(sparkle, hitEffect, ps.main.duration + ps.main.startLifetime.constantMax);
    }

    public void SpawnTextMarker(Vector3 pos, Quaternion rot, string text, Color color)
    {
        GameObject marker = ObjectPool.Instance.Get(textMarker, pos, rot);
        
        HitMarker markerComp = marker.GetComponent<HitMarker>();
        markerComp.ShowText(text, color);
        
        ObjectPool.Instance.Return(marker, textMarker, 1.5f);
    }
}