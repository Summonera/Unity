using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public bool update;
    public GameObject worldGO;

    [HideInInspector] public int chunkSize;
    [HideInInspector] public int chunkHeight;

    [HideInInspector] public int chunkX;
    [HideInInspector] public int chunkY;
    [HideInInspector] public int chunkZ;


    private World world;

    private List<Vector3> newVertices = new List<Vector3>();
    private List<int> newTriangles = new List<int>();
    private List<Vector2> newUV = new List<Vector2>();

    private float tUnit = 0.0625f;
    private Vector2 tStone = new Vector2(1, 15);
    private Vector2 tGrassEdge = new Vector2(3, 15);
    private Vector2 tGrass = new Vector2(2, 15);
    private Vector2 tGrassTop = new Vector2(0, 15);
    private Vector2 tUnbreakable = new Vector2(1, 14);
    private Vector2 tSand = new Vector2(2, 14);
    private Vector2 tSnow = new Vector2(2, 11);

    private Mesh mesh;
    private MeshCollider col;
    private int faceCount;

    void Start()
    {
        world = worldGO.GetComponent<World>();

        mesh = GetComponent<MeshFilter>().mesh;
        col = GetComponent<MeshCollider>();
        GenerateMesh();
    }

    void LateUpdate()
    {
        if (update)
        {
            GenerateMesh();
            update = false;
        }
    }

    void CubeTop(int x, int y, int z, byte block)
    {
        newVertices.Add(new Vector3(x, y, z + 1));
        newVertices.Add(new Vector3(x + 1, y, z + 1));
        newVertices.Add(new Vector3(x + 1, y, z));
        newVertices.Add(new Vector3(x, y, z));

        Vector2 texturePos = GetTexture(x, y, z, tGrassTop);

        Cube(texturePos);
    }

    void CubeNorth(int x, int y, int z, byte block)
    {
        newVertices.Add(new Vector3(x + 1, y - 1, z + 1));
        newVertices.Add(new Vector3(x + 1, y, z + 1));
        newVertices.Add(new Vector3(x, y, z + 1));
        newVertices.Add(new Vector3(x, y - 1, z + 1));

        Vector2 texturePos;

        if (Block(x, y + 1, z) == 0)
        {
            texturePos = GetTexture(x, y, z, tGrassEdge);
        }
        else
        {
            texturePos = GetTexture(x, y, z, tGrass);
        }

        Cube(texturePos);
    }

    void CubeEast(int x, int y, int z, byte block)
    {
        newVertices.Add(new Vector3(x + 1, y - 1, z));
        newVertices.Add(new Vector3(x + 1, y, z));
        newVertices.Add(new Vector3(x + 1, y, z + 1));
        newVertices.Add(new Vector3(x + 1, y - 1, z + 1));

        Vector2 texturePos;

        if (Block(x, y + 1, z) == 0)
        {
            texturePos = GetTexture(x, y, z, tGrassEdge);
        }
        else
        {
            texturePos = GetTexture(x, y, z, tGrass);
        }

        Cube(texturePos);
    }

    void CubeSouth(int x, int y, int z, byte block)
    {
        newVertices.Add(new Vector3(x, y - 1, z));
        newVertices.Add(new Vector3(x, y, z));
        newVertices.Add(new Vector3(x + 1, y, z));
        newVertices.Add(new Vector3(x + 1, y - 1, z));

        Vector2 texturePos;
        if (Block(x, y + 1, z) == 0)
        {
            texturePos = GetTexture(x, y, z, tGrassEdge);
        }
        else
        {
            texturePos = GetTexture(x, y, z, tGrass);
        }

        Cube(texturePos);
    }

    void CubeWest(int x, int y, int z, byte block)
    {
        newVertices.Add(new Vector3(x, y - 1, z + 1));
        newVertices.Add(new Vector3(x, y, z + 1));
        newVertices.Add(new Vector3(x, y, z));
        newVertices.Add(new Vector3(x, y - 1, z));

        Vector2 texturePos;
        if (Block(x, y + 1, z) == 0)
        {
            texturePos = GetTexture(x, y, z, tGrassEdge);
        }
        else
        {
            texturePos = GetTexture(x, y, z, tGrass);
        }
        Cube(texturePos);
    }

    void CubeBot(int x, int y, int z, byte block)
    {
        newVertices.Add(new Vector3(x, y - 1, z));
        newVertices.Add(new Vector3(x + 1, y - 1, z));
        newVertices.Add(new Vector3(x + 1, y - 1, z + 1));
        newVertices.Add(new Vector3(x, y - 1, z + 1));

        Vector2 texturePos = GetTexture(x, y, z, tGrass);

        Cube(texturePos);
    }

    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = newVertices.ToArray();
        mesh.uv = newUV.ToArray();
        mesh.triangles = newTriangles.ToArray();
        mesh.Optimize();
        mesh.RecalculateNormals();

        col.sharedMesh = null;
        col.sharedMesh = mesh;

        newVertices.Clear();
        newUV.Clear();
        newTriangles.Clear();

        faceCount = 0;
    }

    void Cube(Vector2 texturePos)
    {
        newTriangles.Add(faceCount * 4);
        newTriangles.Add(faceCount * 4 + 1);
        newTriangles.Add(faceCount * 4 + 2);
        newTriangles.Add(faceCount * 4);
        newTriangles.Add(faceCount * 4 + 2);
        newTriangles.Add(faceCount * 4 + 3);

        newUV.Add(new Vector2(tUnit * texturePos.x + tUnit, tUnit * texturePos.y));
        newUV.Add(new Vector2(tUnit * texturePos.x + tUnit, tUnit * texturePos.y + tUnit));
        newUV.Add(new Vector2(tUnit * texturePos.x, tUnit * texturePos.y + tUnit));
        newUV.Add(new Vector2(tUnit * texturePos.x, tUnit * texturePos.y));

        faceCount++;
    }

    public void GenerateMesh()
    {

        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkHeight; y++)
            {
                for (int z = 0; z < chunkSize; z++)
                {
                    if (Block(x, y, z) != 0)
                    {
                        if (Block(x, y + 1, z) == 0)
                        {
                            CubeTop(x, y, z, Block(x, y, z));
                        }
                        if (Block(x, y - 1, z) == 0)
                        {
                            CubeBot(x, y, z, Block(x, y, z));

                        }
                        if (Block(x + 1, y, z) == 0)
                        {
                            CubeEast(x, y, z, Block(x, y, z));

                        }
                        if (Block(x - 1, y, z) == 0)
                        {
                            CubeWest(x, y, z, Block(x, y, z));

                        }
                        if (Block(x, y, z + 1) == 0)
                        {
                            CubeNorth(x, y, z, Block(x, y, z));

                        }
                        if (Block(x, y, z - 1) == 0)
                        {
                            CubeSouth(x, y, z, Block(x, y, z));

                        }
                    }
                }
            }
        }
        UpdateMesh();
    }

    byte Block(int x, int y, int z)
    {
        return world.Block(x + chunkX, y + chunkY, z + chunkZ);
    }

    Vector2 GetTexture(int x, int y, int z, Vector2 tGrassPos)
    {
        if (Block(x, y, z) == 1)
        {
            return tUnbreakable;
        }
        else if (Block(x, y, z) == 2)
        {
            return tStone;
        }
        else if (Block(x, y, z) == 3)
        {
            return tSand;
        }
        else if (Block(x, y, z) == 4)
        {
            return tGrassPos;
        }
        else if (Block(x, y, z) == 5)
        {
            return tSnow;
        }
        else
        {
            return Vector2.zero;
        }
    }
}
