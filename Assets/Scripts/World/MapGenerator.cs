using System;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

[System.Serializable]
public class TreeSpawn
{
    public GameObject prefab;
    [Range(0f, 1f)] public float baseChance = 0.2f;
    [Range(0f, 1f)] public float heightInfluence = 0.5f;
}

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class MapGenerator : MonoBehaviour
{
    public int size = 128;
    public float heightScale = 12f;
    public float noiseScale = 0.06f;
    public int seed;
    public bool randomizeSeed;

    public float falloffStrength = 0.65f;
    public float falloffSharpness = 3f;

    public Material terrainMaterial;
    public Material waterMaterial;
    public float waterLevel = 2f;
    public Vector2 uvScale = new Vector2(1f, 1f);

    public GrassComputeScript grassSystem;
    public SO_GrassSettings grassSettings;
    public float grassDensity = 0.35f;

    public TreeSpawn[] treePrefabs;
    public float treeDensity = 0.1f;
    public Transform treeParent;

    private Mesh mesh;
    private GameObject water;
    private List<GameObject> spawnedTrees = new List<GameObject>();
    private NavMeshSurface surface;

    private void Awake()
    {
        surface = GetComponent<NavMeshSurface>();
    }

    private void Start()
    {
        if (randomizeSeed)
        {
            seed = Random.Range(0, 1000000);
        }

        Generate();
        surface.BuildNavMesh();
        PlacePlayer();
    }

    private void Generate()
    {
        ClearTrees();

        MeshFilter meshFilter = GetComponent<MeshFilter>();
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        MeshCollider meshCollider = GetComponent<MeshCollider>();

        if (mesh != null)
        {
            mesh.Clear();
        }
        else
        {
            mesh = new Mesh();
        }

        mesh.indexFormat = IndexFormat.UInt32;

        int vertexCount = (size + 1) * (size + 1);
        Vector3[] vertices = new Vector3[vertexCount];
        Vector2[] uvArray = new Vector2[vertexCount];
        int[] triangles = new int[size * size * 6];

        float offsetX = seed * 0.1f;
        float offsetZ = seed * 0.1f;

        int vertexIndex = 0;
        for (int z = 0; z <= size; z++)
        {
            for (int x = 0; x <= size; x++, vertexIndex++)
            {
                float nx = (x * noiseScale) + offsetX;
                float nz = (z * noiseScale) + offsetZ;
                float height = Mathf.PerlinNoise(nx, nz) * heightScale;
                float falloffValue = Falloff(x, z);
                height *= Mathf.Clamp01(1f - falloffValue);
                vertices[vertexIndex] = new Vector3(x, height, z);
                uvArray[vertexIndex] = new Vector2((x / (float)size) * uvScale.x, (z / (float)size) * uvScale.y);
            }
        }

        int triangleIndex = 0;
        int v = 0;
        for (int z = 0; z < size; z++, v++)
        {
            for (int x = 0; x < size; x++, v++)
            {
                triangles[triangleIndex + 0] = v;
                triangles[triangleIndex + 1] = v + size + 1;
                triangles[triangleIndex + 2] = v + 1;
                triangles[triangleIndex + 3] = v + 1;
                triangles[triangleIndex + 4] = v + size + 1;
                triangles[triangleIndex + 5] = v + size + 2;
                triangleIndex += 6;
            }
        }

        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0, true);
        mesh.SetUVs(0, new List<Vector2>(uvArray));
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        meshFilter.sharedMesh = mesh;
        meshCollider.sharedMesh = mesh;
        if (terrainMaterial != null)
        {
            meshRenderer.sharedMaterial = terrainMaterial;
        }

        MakeGrass(vertices, mesh.normals);
        SpawnTrees(vertices);
    }

    private float Falloff(int x, int z)
    {
        float edgeX = Mathf.Min(x, size - x);
        float edgeZ = Mathf.Min(z, size - z);
        float edgeDistance = Mathf.Min(edgeX, edgeZ);
        float normalized = 1f - Mathf.Clamp01(edgeDistance / (size * 0.5f));
        return Mathf.Pow(normalized, falloffSharpness) * falloffStrength;
    }

    private void MakeWater()
    {
        if (waterMaterial == null)
        {
            return;
        }

        if (water != null)
        {
            DestroyImmediate(water);
        }

        water = GameObject.CreatePrimitive(PrimitiveType.Plane);
        water.name = "Water";
        water.transform.SetParent(transform, false);
        water.transform.position = transform.position + new Vector3(size * 0.5f, waterLevel, size * 0.5f);
        water.transform.localScale = new Vector3(size / 10f, 1f, size / 10f);

        MeshRenderer meshRenderer = water.GetComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = waterMaterial;
        DestroyImmediate(water.GetComponent<MeshCollider>());
    }

    private void MakeGrass(Vector3[] vertices, Vector3[] normals)
    {
        if (grassSystem == null || grassSettings == null)
        {
            return;
        }

        grassSystem.currentPresets = grassSettings;

        List<GrassData> grassDataList = new List<GrassData>(vertices.Length);
        Vector3 upVector = Vector3.up;
        Color topColor = grassSettings.topTint;
        Vector2 lengthRange = new Vector2(grassSettings.MinHeight, grassSettings.MaxHeight);

        for (int i = 0; i < vertices.Length; i++)
        {
            if (vertices[i].y < waterLevel)
            {
                continue;
            }

            if (Random.value > grassDensity)
            {
                continue;
            }

            GrassData grassData = new GrassData
            {
                position = transform.TransformPoint(vertices[i]),
                normal = normals[i].sqrMagnitude > 0f ? normals[i] : upVector,
                length = new Vector2(
                    Random.Range(lengthRange.x, lengthRange.y),
                    Random.Range(lengthRange.x, lengthRange.y)),
                color = new Vector3(topColor.r, topColor.g, topColor.b)
            };

            grassDataList.Add(grassData);
        }

        grassSystem.SetGrassPaintedDataList = grassDataList;
        grassSystem.Reset();
    }

    private void SpawnTrees(Vector3[] vertices)
    {
        if (treePrefabs == null || treePrefabs.Length == 0)
        {
            return;
        }

        Random.InitState(seed + 1337);

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 vertex = vertices[i];

            if (vertex.y < waterLevel)
            {
                continue;
            }

            float roll = Random.value;
            if (roll > treeDensity)
            {
                continue;
            }

            for (int j = 0; j < treePrefabs.Length; j++)
            {
                TreeSpawn treeSpawn = treePrefabs[j];
                if (treeSpawn.prefab == null)
                {
                    continue;
                }

                float heightFactor = vertex.y / heightScale;
                float chance = Mathf.Clamp01(treeSpawn.baseChance + (heightFactor - 0.5f) * treeSpawn.heightInfluence);

                float treeRoll = Random.value;
                if (treeRoll <= chance)
                {
                    Vector3 position = transform.TransformPoint(vertex);
                    GameObject treeObject = Instantiate(treeSpawn.prefab, position,
                        Quaternion.Euler(0, Random.Range(0f, 360f), 0),
                        treeParent ? treeParent : transform);

                    NavMeshObstacle obstacle = treeObject.AddComponent<NavMeshObstacle>();
                    obstacle.carving = true;
                    obstacle.shape = NavMeshObstacleShape.Capsule;
                    obstacle.radius = 1f;
                    obstacle.height = 5f;

                    spawnedTrees.Add(treeObject);
                    break;
                }
            }
        }
    }


    private void ClearTrees()
    {
        for (int i = 0; i < spawnedTrees.Count; i++)
        {
            if (spawnedTrees[i] != null)
            {
                DestroyImmediate(spawnedTrees[i]);
            }
        }

        spawnedTrees.Clear();
    }

    private void PlacePlayer()
    {
        Transform player = PlayerMovement.Instance.transform;

        float centerX = size * 0.5f;
        float centerZ = size * 0.5f;

        Ray ray = new Ray(new Vector3(centerX, heightScale * 2f, centerZ), Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, heightScale * 4f))
        {
            Vector3 position = hit.point;
            position.y += 2;
            player.position = position;
        }
        else
        {
            player.position = new Vector3(centerX, heightScale + 2, centerZ);
        }
    }
}