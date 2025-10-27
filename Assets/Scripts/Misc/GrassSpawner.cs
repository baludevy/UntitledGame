using UnityEngine;
using System.Collections.Generic;

public class GroundGrassSpawner : MonoBehaviour
{
    [Header("Grass Settings")]
    public Material grassMaterial;
    public int grassCount = 100000;
    public float grassHeight = 1f;
    public float grassWidth = 0.5f;
    
    [Header("Culling Settings")]
    public float maxDrawDistance = 100f;
    
    [Header("Spawn Settings")]
    public float groundOffset = 0.05f;
    
    private Mesh grassMesh;
    private Matrix4x4[] allMatrices;
    private List<Matrix4x4> visibleMatrices;
    private MaterialPropertyBlock propertyBlock;
    private Bounds groundBounds;

    void Start()
    {
        CreateCrossedQuadMesh();
        SpawnGrass();
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
        
        int[] triangles = new int[12]
        {
            0, 2, 1, 2, 3, 1,
            4, 6, 5, 6, 7, 5 
        };
        
        Vector2[] uvs = new Vector2[8];
        for (int i = 0; i < 8; i++)
        {
            uvs[i] = new Vector2((i % 2 == 0) ? 0 : 1, (i < 2 || i == 4 || i == 5) ? 0 : 1);
        }
        
        grassMesh.vertices = vertices;
        grassMesh.triangles = triangles;
        grassMesh.uv = uvs;
        grassMesh.RecalculateNormals();
        
        visibleMatrices = new List<Matrix4x4>();
    }

    private void SpawnGrass()
    {
        allMatrices = new Matrix4x4[grassCount];
        propertyBlock = new MaterialPropertyBlock();
        
        Renderer groundRenderer = GetComponent<Renderer>();
        groundBounds = groundRenderer != null ? groundRenderer.bounds : new Bounds(transform.position, transform.localScale * 10f);
        
        for (int i = 0; i < grassCount; i++)
        {
            Vector3 position = new Vector3(
                Random.Range(groundBounds.min.x, groundBounds.max.x),
                groundBounds.max.y + groundOffset,
                Random.Range(groundBounds.min.z, groundBounds.max.z)
            );
            
            Quaternion rotation = Quaternion.Euler(0, Random.Range(0, 360f), 0);
            Vector3 scale = Vector3.one * Random.Range(0.8f, 1.2f);
            
            allMatrices[i] = Matrix4x4.TRS(position, rotation, scale);
        }
        
        Debug.Log($"Spawned {grassCount} grass instances");
    }

    private void Update()
    {
        if (grassMesh == null || allMatrices == null) return;
        
        Vector3 cameraPos = PlayerCamera.Instance.transform.position;
        
        visibleMatrices.Clear();
        
        for (int i = 0; i < grassCount; i++)
        {
            Vector3 grassPos = new Vector3(allMatrices[i].m03, allMatrices[i].m13, allMatrices[i].m23);
            float distance = Vector3.Distance(grassPos, cameraPos);
            
            if (distance <= maxDrawDistance)
            {
                visibleMatrices.Add(allMatrices[i]);
            }
        }
        
        if (visibleMatrices.Count > 0)
        {
            Graphics.DrawMeshInstanced(grassMesh, 0, grassMaterial, visibleMatrices.ToArray(), visibleMatrices.Count, propertyBlock);
        }
    }

    void OnDestroy()
    {
        if (grassMesh != null)
        {
            DestroyImmediate(grassMesh);
        }
    }
}