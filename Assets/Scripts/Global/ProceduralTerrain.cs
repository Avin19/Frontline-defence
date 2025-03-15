// #define MESH_GENERATION_TESTING
// #define TERRAIN_DEBUG_PERLIN
// #define TERRAIN_DEBUG_EDGES

using UnityEngine;

namespace FrontLineDefense.Global
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
    public class ProceduralTerrain : MonoBehaviour
    {
        [SerializeField] private int _xWidth = 100; // Terrain Width // Provide extra padding for before and after points
        [SerializeField] private int _zDepth = 100; // Terrain depth

        [SerializeField] private float _scale = 20f; // Sacle of the noise

        [SerializeField] private float _heightMultiplier = 10f; // Multiplier to control mountain height
        [SerializeField] private string _generatedHash;
        [SerializeField] private AnimationCurve _lerpCurve;

#if MESH_GENERATION_TESTING
        [SerializeField] private bool _meshGenerateRequested;
#endif
        // private byte[] _randomBytes;

        private Mesh mesh;
        // private Vector2 offset;
        private MeshCollider meshCollider;
        private const int zPerlinPoint = 15, zSmoothingRange = 10;      // zPerlinPoint cannot be constant as it depends on _zDepth

        public Vector3[] randomEnemyPositions;             //Debugging
        // public Vector3[] vertices;             //Debugging
        // [SerializeField] private int seed = 0;

        private void Start()
        {
            // GenerateHash();

            // if (seed == 0)
            //     seed = Random.Range(0, int.MaxValue); // Random seed if seed is 0
            // Random.InitState(seed);

            Random.InitState(_generatedHash.GetHashCode());
            // offset = new Vector2(Random.Range(0f, 1000f), Random.Range(0f, 1000f));

            mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = mesh;
            meshCollider = GetComponent<MeshCollider>();

            // CreateTerrain();
            CreateTerrain2();
        }

#if MESH_GENERATION_TESTING
        private void FixedUpdate()
        {
            if (_meshGenerateRequested)
            {
                Random.InitState(_generatedHash.GetHashCode());
                offset = new Vector2(Random.Range(0f, 1000f), Random.Range(0f, 1000f));

                // CreateTerrain();
                CreateTerrain2();
                _meshGenerateRequested = false;
            }
        }
#endif

        private void GenerateHash()
        {
            // _randomBytes = new byte[6];

            byte[] _randomBytes = new byte[6];
            // using (var rng = new RNGCryptoServiceProvider())            
            //     rng.GetBytes(bytes);

            System.Random randomByteGenerator = new System.Random(System.DateTime.Now.Day + System.DateTime.Now.Hour
                                                 + System.DateTime.Now.Minute + System.DateTime.Now.Second
                                                 + System.DateTime.Now.Millisecond);

            randomByteGenerator.NextBytes(_randomBytes);

            // and if you need it as a string...
            // _generatedHash = System.BitConverter.ToString(_randomBytes);
            _generatedHash = System.BitConverter.ToString(_randomBytes).Replace("-", "");

            // or maybe...
            // string hash2 = System.BitConverter.ToString(_randomBytes).Replace("-", "").ToLower();
        }


        void CreateTerrain()
        {
            Vector3[] vertices = new Vector3[(_xWidth + 1) * (_zDepth + 1)];
            int[] triangles = new int[_xWidth * _zDepth * 6];
            Vector2[] uvs = new Vector2[vertices.Length];

            int enemyPosFilled = 0, fillInterval = 10;
            Vector3[] randomEnemyPositions = new Vector3[5];

            float xOffset = Random.Range(0f, 1000f);
            float yOffset = Random.Range(0f, 1000f);
            // offset = new Vector2(Random.Range(0f, 1000f), Random.Range(0f, 1000f));

            for (int z = 0, i = 0; z <= _zDepth; z++)
            {
                for (int x = 0; x <= _xWidth; x++, i++)
                {
                    // Calculate height using Perlin noise and the offset for a different map
                    float y = Mathf.PerlinNoise((x + xOffset) * _scale * 0.1f, (z + yOffset) * _scale * 0.1f) * _heightMultiplier;
                    vertices[i] = new Vector3(x, y, z);
                    uvs[i] = new Vector2((float)x / _xWidth, (float)z / _zDepth);

                    if (z == 2                                          // In line with the player
                     && enemyPosFilled < 5 && fillInterval >= 1
                     && Random.Range(0f, 1f) <= 0.3f)                   //Can get rid of this part
                    {
                        fillInterval = 0;
                        randomEnemyPositions[enemyPosFilled] = vertices[i];             //We dont need z
                        enemyPosFilled++;
                    }

                    fillInterval++;
                }
            }

            int vert = 0, tris = 0;
            for (int z = 0; z < _zDepth; z++)
            {
                for (int x = 0; x < _xWidth; x++)
                {
                    triangles[tris + 0] = vert + 0;
                    triangles[tris + 1] = vert + _xWidth + 1;
                    triangles[tris + 2] = vert + 1;
                    triangles[tris + 3] = vert + 1;
                    triangles[tris + 4] = vert + _xWidth + 1;
                    triangles[tris + 5] = vert + _xWidth + 2;

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

            GameManager.Instance.OnMapGenerated?.Invoke(randomEnemyPositions);
        }

        void CreateTerrain2()
        {
            Vector3[] vertices = new Vector3[(_xWidth + 1) * (_zDepth + 1)];
            int[] triangles = new int[_xWidth * _zDepth * 6];
            Vector2[] uvs = new Vector2[vertices.Length];

            int enemyPosFilled = 0, fillInterval = 0;
            // Vector3[] 
            randomEnemyPositions = new Vector3[5];

            // int zPerlinPoint = 15;
            // int zRange = 10;

#if TERRAIN_DEBUG_PERLIN
            int printTimeLerpZ = 0, printTimeZ = 0;
#endif

            float[] perlinYPoints = new float[_xWidth + 1];
            //Generate an array of Perlin Y Points
            // ************************************************************************************
            //                      PART 1: Filling the middle portion of the mesh. 
            // ************************************************************************************
            // We are not taking the full portion of the mesh, as that will make the mesh start at a higher position 
            // than raising from the sea

            float xOffset = Random.Range(0f, 1000f);
            float yOffset = Random.Range(0f, 1000f);

            // for (int x = 0; x <= xWidth; x++)
            for (int x = zSmoothingRange; x <= _xWidth - zSmoothingRange; x++)
            {
                perlinYPoints[x] = Mathf.PerlinNoise((x + xOffset) * _scale * 0.1f,
                                    (zPerlinPoint + yOffset) * _scale * 0.1f) * _heightMultiplier;

#if TERRAIN_DEBUG_PERLIN
                        if (printTimeZ <= xWidth)
                        {
                            Debug.Log($"Perlin Noise | index : {i} | y : {y}");
                            printTimeZ++;
                        }
#endif
            }

            // ************************************************************************************
            //                      PART 2: Filling the start and end of the mesh
            // ************************************************************************************
            for (int x = 0; x < zSmoothingRange; x++)
            {
                // Taking 10 points plus/minus and using curve to fill the points value
                float pointVal = _lerpCurve.Evaluate(x / (float)zSmoothingRange + 0.15f);

                // perlinYPoints[zRange - x - 1] = pointVal * heightMultiplier;
                // perlinYPoints[xWidth - zRange + x + 1] = pointVal * heightMultiplier;

                perlinYPoints[zSmoothingRange - x - 1] = Mathf.Lerp(0, perlinYPoints[zSmoothingRange], pointVal);       // Smoothing Start
                perlinYPoints[_xWidth - zSmoothingRange + x + 1] = Mathf.Lerp(0, perlinYPoints[_xWidth - zSmoothingRange], pointVal);       // Smoothing End

#if TERRAIN_DEBUG_EDGES
                Debug.Log($"Perlin Before Points | x : {x} | val : {pointVal} | Before : {zRange - x - 1} "
                        + $"| After {xWidth - zRange + x + 1}");
#endif
            }

            // ************************************************************************************
            //                      PART 3: Smoothing the mesh at the sides
            // ************************************************************************************
            // We can walk through x/y co-ord regularly, 
            for (int zCoord = 0, i = 0; zCoord <= _zDepth; zCoord++)
            {
                for (int xCoord = 0; xCoord <= _xWidth; xCoord++, i++)
                {
                    // Calculate height using Perlin noise and the offset for a different map
                    float yCoord;

                    if ((zCoord - zPerlinPoint) > 0 && (zCoord - zPerlinPoint) <= zSmoothingRange)
                    {
                        // Debug.Log($"Greater than Z | z : {z}");
                        // float lerpNormalized = 1 - ((z - zPoint) / zRange);       //We would need inverse as we are progressing forward
                        // float lerpNormalized = (z - zPoint) / zRange;
                        float val = _lerpCurve.Evaluate((zCoord - zPerlinPoint) / (float)zSmoothingRange);
                        // y = Mathf.Lerp(0, vertices[(zPoint * (xWidth + 1)) + x].y, val);              //Total xWidth+1
                        yCoord = Mathf.Lerp(0, perlinYPoints[xCoord], val);              //Total xWidth+1

#if TERRAIN_DEBUG_PERLIN
                        if (printTimeLerpZ < 20)
                        {
                            Debug.Log($"Lerping | lerpNormalized : {(z - zPoint) / zRange} | val : {val} | z : {z} | zpoint : {zPoint} "
                            + $"| Index : {(zPoint * (xWidth + 1)) + x} | x : {x} | verticeY : {vertices[(zPoint * (xWidth + 1)) + x].y}");
                            printTimeLerpZ++;
                        }
#endif
                    }
                    else if ((zPerlinPoint - zCoord) > 0 && (zPerlinPoint - zCoord) <= zSmoothingRange)
                    {
                        // Debug.Log($"Less than Z | z : {z}");
                        float val = _lerpCurve.Evaluate((zPerlinPoint - zCoord) / (float)zSmoothingRange);
                        // y = Mathf.Lerp(0, vertices[(zPoint * (xWidth + 1)) + x].y, val);              //Total xWidth+1
                        yCoord = Mathf.Lerp(0, perlinYPoints[xCoord], val);              //Total xWidth+1

#if TERRAIN_DEBUG_PERLIN
                        if (printTimeLerpZ < 20)
                        {
                            Debug.Log($"Lerping | lerpNormalized : {1 - ((zPoint - z) / zRange)} | val : {val} | z : {z} | zpoint : {zPoint} "
                            + $"| Index : {(zPoint * (xWidth + 1)) + x} | x : {x} | verticeY : {vertices[(zPoint * (xWidth + 1)) + x].y}");
                            printTimeLerpZ++;
                        }
#endif
                    }
                    else if (zCoord == zPerlinPoint)                 // In line with the player
                    {
                        yCoord = perlinYPoints[xCoord];

                        if (yCoord >= 5
                         && enemyPosFilled < 5 && fillInterval >= 2
                         && Random.Range(0f, 1f) <= 0.3f)                   //Can get rid of this part
                        {
                            fillInterval = 0;
                            randomEnemyPositions[enemyPosFilled] = new Vector3(xCoord, yCoord, zCoord);
                            enemyPosFilled++;
                        }
                    }
                    else yCoord = 0f;

                    vertices[i] = new Vector3(xCoord, yCoord, zCoord);
                    uvs[i] = new Vector2((float)xCoord / _xWidth, (float)zCoord / _zDepth);

                    /*if (zCoord == zPerlinPoint                                          // In line with the player
                     && enemyPosFilled < 5 && fillInterval >= 1
                     && Random.Range(0f, 1f) <= 0.3f)                   //Can get rid of this part
                    {
                        fillInterval = 0;
                        randomEnemyPositions[enemyPosFilled] = vertices[i];
                        enemyPosFilled++;
                    }*/

                    fillInterval++;
                }
            }

            int vert = 0, tris = 0;
            for (int z = 0; z < _zDepth; z++)
            {
                for (int x = 0; x < _xWidth; x++)
                {
                    triangles[tris + 0] = vert + 0;
                    triangles[tris + 1] = vert + _xWidth + 1;
                    triangles[tris + 2] = vert + 1;
                    triangles[tris + 3] = vert + 1;
                    triangles[tris + 4] = vert + _xWidth + 1;
                    triangles[tris + 5] = vert + _xWidth + 2;

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

            GameManager.Instance.OnMapGenerated?.Invoke(randomEnemyPositions);
        }

    }
}