using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Effects
{
    public static IEnumerator Flash(MeshRenderer[] renderers, Color color)
    {
        List<Material> originalMaterials = new();
        Material flashMaterial = new Material(Shader.Find("Unlit/Color"));
        flashMaterial.color = color;

        foreach (MeshRenderer renderer in renderers)
        {
            originalMaterials.Add(renderer.material);
            renderer.material = flashMaterial;
        }

        yield return new WaitForSeconds(0.05f);

        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material = originalMaterials[i];
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
}