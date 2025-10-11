using UnityEngine;
using System.IO;

public class IconCapture : MonoBehaviour
{
    public Camera cam;
    public RenderTexture rt;

    [ContextMenu("Capture")]
    void Capture()
    {
        bool prevSRGB = GL.sRGBWrite;
        GL.sRGBWrite = (QualitySettings.activeColorSpace == ColorSpace.Linear);

        cam.targetTexture = rt;
        cam.Render();

        RenderTexture prev = RenderTexture.active;
        RenderTexture.active = rt;

        Texture2D tex = new Texture2D(rt.width, rt.height, TextureFormat.RGBA32, false);
        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        tex.Apply();

        var bytes = tex.EncodeToPNG();
        File.WriteAllBytes(Application.dataPath + "/icon.png", bytes);

        RenderTexture.active = prev;
        cam.targetTexture = null;
        GL.sRGBWrite = prevSRGB;
    }
}