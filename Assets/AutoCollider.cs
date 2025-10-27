using UnityEngine;

[ExecuteInEditMode]
public class AutoCollider : MonoBehaviour
{
    private void OnValidate()
    {
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        if (boxCollider == null) boxCollider = gameObject.AddComponent<BoxCollider>();

        Renderer[] childRenderers = GetComponentsInChildren<Renderer>();
        if (childRenderers.Length == 0) return;

        Bounds combinedBounds = childRenderers[0].bounds;
        for (int i = 1; i < childRenderers.Length; i++)
        {
            combinedBounds.Encapsulate(childRenderers[i].bounds);
        }

        boxCollider.center = combinedBounds.center - transform.position;
        boxCollider.size = combinedBounds.size;
    }
}