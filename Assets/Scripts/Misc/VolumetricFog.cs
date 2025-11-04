using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class VolumetricFog : MonoBehaviour
{
    [SerializeField] Color fogColor = new Color(0.7f, 0.8f, 0.9f, 1f);
    [SerializeField] float density = 0.05f;
    [SerializeField] int steps = 32;
    [SerializeField] float startDistance = 16;
    [SerializeField] float fadeLength = 50;
    Material material;

    void OnEnable()
    {
        Shader shader = Shader.Find("Hidden/VolumetricFogPost");
        if (shader) material = new Material(shader);
        GetComponent<Camera>().depthTextureMode |= DepthTextureMode.Depth;
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (!material)
        {
            Graphics.Blit(src, dest);
            return;
        }
        material.SetColor("_Color", fogColor);
        material.SetFloat("_Density", density);
        material.SetInt("_Steps", steps);
        material.SetFloat("_StartDistance", startDistance);
        material.SetFloat("_FadeLength", fadeLength);
        Graphics.Blit(src, dest, material);
    }
}