using UnityEngine;

[ExecuteInEditMode]
public class CameraEdgeOutline : MonoBehaviour
{
    public Shader shader;
    Material material;

    void Start()
    {
        if (shader == null) enabled = false;
        else material = new Material(shader);
        GetComponent<Camera>().depthTextureMode = DepthTextureMode.DepthNormals;
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (material == null)
        {
            Graphics.Blit(source, destination);
            return;
        }

        Graphics.Blit(source, destination, material);
    }
}