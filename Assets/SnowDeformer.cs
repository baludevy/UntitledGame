using UnityEngine;

[ExecuteAlways]
public class SnowDeformer : MonoBehaviour
{
    public float snowHeight = 0.2f;
    public float scale = 2f;
    public float threshold = 0.6f;
    public int seed = 0;
    MeshFilter mf;
    Vector3[] baseVerts;

    void Start()
    {
        mf = GetComponent<MeshFilter>();
        baseVerts = mf.sharedMesh.vertices;
        Apply();
    }

    void OnValidate()
    {
        if (!mf || baseVerts == null) return;
        Apply();
    }

    void Apply()
    {
        var m = Instantiate(mf.sharedMesh);
        Random.InitState(seed);
        var offset = Random.value * 1000f;
        var v = m.vertices;
        for (int i = 0; i < v.Length; i++)
        {
            var p = baseVerts[i];
            var n = Mathf.PerlinNoise(p.x * scale + offset, p.z * scale + offset);
            float lift = 0;
            if (n > threshold)
                lift = Mathf.SmoothStep(0, snowHeight, (n - threshold) / (1 - threshold));
            v[i].y = p.y + lift;
        }
        m.vertices = v;
        m.RecalculateNormals();
        mf.sharedMesh = m;
    }
}