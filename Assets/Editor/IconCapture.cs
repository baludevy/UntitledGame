using UnityEngine;
using UnityEditor;
using System.IO;

public class IconCapture
{
    [MenuItem("Tools/Icon Capture")]
    private static void CaptureIcon()
    {
        Camera cam = Object.FindObjectOfType<Camera>();
        if (cam == null || cam.targetTexture == null) return;

        RenderTexture rt = cam.targetTexture;
        RenderTexture prev = RenderTexture.active;
        RenderTexture.active = rt;

        Texture2D tex = new Texture2D(rt.width, rt.height, TextureFormat.RGBA32, false, false);
        cam.Render();
        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);

        for (int y = 0; y < tex.height; y++)
        {
            for (int x = 0; x < tex.width; x++)
            {
                Color c = tex.GetPixel(x, y);
                c.r = Mathf.Pow(c.r, 1f / 2.2f);
                c.g = Mathf.Pow(c.g, 1f / 2.2f);
                c.b = Mathf.Pow(c.b, 1f / 2.2f);
                tex.SetPixel(x, y, c);
            }
        }

        tex.Apply();

        string path = Application.dataPath + "/icon.png";
        File.WriteAllBytes(path, tex.EncodeToPNG());

        RenderTexture.active = prev;
        Object.DestroyImmediate(tex);
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

        TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath("Assets/icon.png");
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Default;
            importer.alphaIsTransparency = true;
            importer.sRGBTexture = true;
            importer.SaveAndReimport();
        }
    }
}