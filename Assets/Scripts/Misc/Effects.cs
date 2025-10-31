using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Effects : MonoBehaviour
{
    public Color critColor = Color.yellow;
    public Color superEffectiveColor = Color.cyan;
    
    public static Effects Instance;

    public Material flashMaterial;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public static IEnumerator Flash(MeshRenderer[] renderers, Color color)
    {
        List<Material[]> originalMaterials = new();
        Instance.flashMaterial.color = color;

        foreach (MeshRenderer renderer in renderers)
        {
            originalMaterials.Add(renderer.sharedMaterials);
            renderer.sharedMaterial = Instance.flashMaterial;
        }

        yield return new WaitForSeconds(0.05f);

        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].sharedMaterials = originalMaterials[i];
        }
    }

    public static IEnumerator HitAnimation(Vector3 start, Transform targetTransform)
    {
        Vector3 target = start * 0.85f;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / 0.2f;
            float eased = t < 0.5f ? 4f * t * t * t : 1f - Mathf.Pow(-2f * t + 2f, 3f) / 2f;
            targetTransform.localScale = Vector3.Lerp(start, target, eased);
            yield return null;
        }

        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / 0.2f;
            float eased = t < 0.5f ? 4f * t * t * t : 1f - Mathf.Pow(-2f * t + 2f, 3f) / 2f;
            targetTransform.localScale = Vector3.Lerp(target, start, eased);
            yield return null;
        }

        targetTransform.localScale = start;
    }


    public static void SpawnEffects(Vector3 pos, Vector3 rot, float damage, bool crit, HitEffectiveness eff,
        bool hitEffect = true)
    {
        Color color = GetEffectColor(crit, eff);

        if (crit)
            PrefabManager.Instance.SpawnTextMarker(pos + new Vector3(Random.Range(-0.3f, 0.3f), -0.1f, 0f),
                Quaternion.LookRotation(rot), "CRIT!", color);

        PrefabManager.Instance.SpawnDamageMarker(pos, Quaternion.Euler(rot), damage, color);

        if (hitEffect)
            PrefabManager.Instance.SpawnSparkles(pos, Quaternion.Euler(rot), color);
    }

    public static Color GetEffectColor(bool crit, HitEffectiveness eff)
    {
        if (crit) return Effects.Instance.critColor;

        if (eff == HitEffectiveness.SuperEffective) return Effects.Instance.superEffectiveColor;

        return Color.white;
    }
}