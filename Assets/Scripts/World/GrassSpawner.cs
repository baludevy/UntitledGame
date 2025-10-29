using UnityEngine;
using System.Collections.Generic;

public class GroundGrassSpawner : MonoBehaviour
{
    [Header("Grass Settings")] public Material grassMaterial;
    public ComputeShader cullCompute;
    public int grassCount = 100000;
    public float grassHeight = 1f;
    public float grassWidth = 0.5f;

    [Header("Culling Settings")] public float maxDrawDistance = 100f;
    public float cullBufferMargin = 2f;

    [Header("Spawn Settings")] public float groundOffset = 0.05f;

    private Mesh grassMesh;
    private ComputeBuffer positionBuffer;
    private ComputeBuffer argsBuffer;
    private ComputeBuffer visibleBuffer;
    private Bounds drawBounds;

    private int kernel;
    private uint[] args = new uint[5];
    private const int stride = sizeof(float) * 4;

    void Start()
    {
        CreateCrossedQuadMesh();
        InitGrass();
        InitBuffers();
    }

    void CreateCrossedQuadMesh()
    {
        grassMesh = new Mesh();

        Vector3[] vertices = new Vector3[8];
        vertices[0] = new Vector3(-grassWidth * 0.5f, 0, 0);
        vertices[1] = new Vector3(grassWidth * 0.5f, 0, 0);
        vertices[2] = new Vector3(-grassWidth * 0.5f, grassHeight, 0);
        vertices[3] = new Vector3(grassWidth * 0.5f, grassHeight, 0);
        vertices[4] = new Vector3(0, 0, -grassWidth * 0.5f);
        vertices[5] = new Vector3(0, 0, grassWidth * 0.5f);
        vertices[6] = new Vector3(0, grassHeight, -grassWidth * 0.5f);
        vertices[7] = new Vector3(0, grassHeight, grassWidth * 0.5f);

        int[] triangles = new int[12] { 0, 2, 1, 2, 3, 1, 4, 6, 5, 6, 7, 5 };

        Vector2[] uvs = new Vector2[8];
        for (int i = 0; i < 8; i++)
            uvs[i] = new Vector2((i & 1) == 0 ? 0 : 1, (i < 2 || i == 4 || i == 5) ? 0 : 1);

        grassMesh.vertices = vertices;
        grassMesh.triangles = triangles;
        grassMesh.uv = uvs;
        grassMesh.RecalculateNormals();
    }

    void InitGrass()
    {
        Vector4[] positions = new Vector4[grassCount];

        Renderer groundRenderer = GetComponent<Renderer>();
        Bounds groundBounds = groundRenderer
            ? groundRenderer.bounds
            : new Bounds(transform.position, transform.localScale * 10f);

        for (int i = 0; i < grassCount; i++)
        {
            Vector3 pos = new Vector3(
                Random.Range(groundBounds.min.x, groundBounds.max.x),
                groundBounds.max.y + groundOffset,
                Random.Range(groundBounds.min.z, groundBounds.max.z)
            );

            float rotY = Random.Range(0f, 360f);
            positions[i] = new Vector4(pos.x, pos.y, pos.z, rotY);
        }

        positionBuffer = new ComputeBuffer(grassCount, stride);
        positionBuffer.SetData(positions);

        drawBounds = groundBounds;
        drawBounds.Expand(maxDrawDistance * 2f + 20f);
    }

    void InitBuffers()
    {
        visibleBuffer = new ComputeBuffer(grassCount, stride, ComputeBufferType.Append);
        visibleBuffer.SetCounterValue(0);

        kernel = cullCompute.FindKernel("CSMain");
        cullCompute.SetBuffer(kernel, "allPositions", positionBuffer);
        cullCompute.SetBuffer(kernel, "visiblePositions", visibleBuffer);
        cullCompute.SetInt("_InstanceCount", grassCount);
        cullCompute.SetFloat("_MaxDistance", maxDrawDistance + cullBufferMargin);

        argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        args[0] = (uint)grassMesh.GetIndexCount(0);
        args[1] = 0;
        args[2] = (uint)grassMesh.GetIndexStart(0);
        args[3] = (uint)grassMesh.GetBaseVertex(0);
        args[4] = 0;
        argsBuffer.SetData(args);

        grassMaterial.enableInstancing = true;
        grassMaterial.SetShaderPassEnabled("ShadowCaster", false);
        grassMaterial.SetBuffer("_Positions", visibleBuffer);
    }

    void Update()
    {
        visibleBuffer.SetCounterValue(0);

        cullCompute.SetVector("_CameraPos", Camera.main.transform.position);
        cullCompute.SetFloat("_MaxDistance", maxDrawDistance + cullBufferMargin);
        cullCompute.Dispatch(kernel, Mathf.CeilToInt(grassCount / 64f), 1, 1);

        ComputeBuffer.CopyCount(visibleBuffer, argsBuffer, sizeof(uint));
        argsBuffer.GetData(args);
        if (args[1] == 0) args[1] = 1; // no empty frame flicker
        argsBuffer.SetData(args);

        Graphics.DrawMeshInstancedIndirect(grassMesh, 0, grassMaterial, drawBounds, argsBuffer);
    }

    void OnDestroy()
    {
        positionBuffer?.Release();
        visibleBuffer?.Release();
        argsBuffer?.Release();
        if (grassMesh) DestroyImmediate(grassMesh);
    }
}