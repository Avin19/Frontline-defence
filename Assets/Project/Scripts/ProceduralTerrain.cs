
using UnityEngine;


[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class ProceduralTerrain : MonoBehaviour
{
    [SerializeField] private int width = 100; // Terrain Width 
    [SerializeField] private int depth = 100; // Terrain depth

    [SerializeField] private float scale = 20f; // Sacle of the noise

    [SerializeField] private float heightMultiplier = 10f; // Multiplier to control mountain height


    private Mesh mesh;
    private Vector2 offset;
    private MeshCollider meshCollider;
    private Vector3[] vertices;
    [SerializeField] private int seed = 0;

    private void Start()
    {
        if (seed == 0)
            seed = Random.Range(0, int.MaxValue); // Random seed if seed is 0
        Random.InitState(seed);
        offset = new Vector2(Random.Range(0f, 1000f), Random.Range(0f, 1000f));

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        meshCollider = GetComponent<MeshCollider>();

        CreateTerrain();
    }


    void CreateTerrain()
    {
        vertices = new Vector3[(width + 1) * (depth + 1)];
        int[] triangles = new int[width * depth * 6];
        Vector2[] uvs = new Vector2[vertices.Length];

        for (int z = 0, i = 0; z <= depth; z++)
        {
            for (int x = 0; x <= width; x++, i++)
            {
                // Calculate height using Perlin noise and the offset for a different map
                float y = Mathf.PerlinNoise((x + offset.x) * scale * 0.1f, (z + offset.y) * scale * 0.1f) * heightMultiplier;
                vertices[i] = new Vector3(x, y, z);
                uvs[i] = new Vector2((float)x / width, (float)z / depth);
            }
        }

        int vert = 0, tris = 0;
        for (int z = 0; z < depth; z++)
        {
            for (int x = 0; x < width; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + width + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + width + 1;
                triangles[tris + 5] = vert + width + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();

        meshCollider.sharedMesh = null;
        meshCollider.sharedMesh = mesh;

    }

}


