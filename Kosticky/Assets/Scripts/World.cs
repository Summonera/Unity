using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    [SerializeField] GameObject chunk;
    [SerializeField] GameObject playerGO;

    [SerializeField] ModifyTerrain modify;
    [SerializeField] public Saver saver;

    public int renderDistance = 64;
    public GameObject player;

    public Chunk[,,] chunks;
    public byte[,,] data;

    [HideInInspector] public int chunkSize = 32;
    [HideInInspector] public int chunkHeight = 64;
    [HideInInspector] public int offsetX = 0;
    [HideInInspector] public int offsetZ = 0;

    private int worldX = 4096;
    private int worldZ = 4096;
    private int worldY;

    private int xPos;
    private int zPos;
    private int dis;

    void Start()
    {
        worldY = chunkHeight;
        data = new byte[worldX, worldY, worldZ];
        chunks = new Chunk[Mathf.FloorToInt(worldX / chunkSize), Mathf.FloorToInt(worldY / chunkHeight), Mathf.FloorToInt(worldZ / chunkSize)];
        CheckData();

        int xPos = (int)(player.transform.position.x / chunkSize);
        int zPos = (int)(player.transform.position.z / chunkSize);
        int dis = renderDistance / chunkSize;

        CheckChunk(xPos, zPos, dis);
    }

    void CheckData()
    {
        if (saver.isFileExists() == true)
        {
            SaveGlobal global = saver.LoadGlobalData();

            offsetX = global.offsetX;
            offsetZ = global.offsetZ;

            player = Instantiate(playerGO, new Vector3(global.position[0], global.position[1], global.position[2]), Quaternion.identity);
            modify.playerGO = player;
            saver.playerGO = player;
            for (int x = 0; x < chunks.GetLength(0); x++)
            {
                for (int z = 0; z < chunks.GetLength(2); z++)
                {
                    SaveChunk chunk = saver.LoadChunkData(x, z);
                    if (chunk != null)
                    {
                        SaveBytesToData(chunk);
                        SaveChunkToArray(chunk.chunkIdX, chunk.chunkIdZ);
                    }
                }
            }
        }
        else
        {
            player = Instantiate(playerGO, new Vector3(worldX / 2f, 50f, worldZ / 2f), Quaternion.identity);
            modify.playerGO = player;
            saver.playerGO = player;
            offsetX = Random.Range(0, 999999);
            offsetZ = Random.Range(0, 999999);
        }
    }


    void SaveChunkToArray(int x, int z)
    {
        GameObject newChunk = Instantiate(chunk, Vector3.zero, Quaternion.identity) as GameObject;

        chunks[x, 0, z] = newChunk.GetComponent<Chunk>();
        chunks[x, 0, z].worldGO = gameObject;
        chunks[x, 0, z].chunkSize = chunkSize;
        chunks[x, 0, z].chunkHeight = chunkHeight;
        chunks[x, 0, z].chunkX = x * chunkSize;
        chunks[x, 0, z].chunkY = 0;
        chunks[x, 0, z].chunkZ = z * chunkSize;

        Destroy(newChunk);
    }

    void SaveBytesToData(SaveChunk data)
    {
        int i = 0;
        for (int x = data.chunkPosX; x < data.chunkPosX + chunkSize; x++)
        {
            for (int y = 0; y < chunkHeight; y++)
            {
                for (int z = data.chunkPosZ; z < chunkSize + data.chunkPosZ; z++)
                {
                    this.data[x, y, z] = data.byteArray[i];
                    i++;
                }
            }
        }
    }

    public void LoadChunk(int x, int z)
    {
        GameObject newChunk = Instantiate(chunk, new Vector3(x * chunkSize - 0.5f,
                 0 * chunkHeight + 0.5f, z * chunkSize - 0.5f), new Quaternion(0, 0, 0, 0)) as GameObject;

        newChunk.name = "Chunk[" + x + "," + 0 + "," + z + "]";
        chunks[x, 0, z] = newChunk.GetComponent<Chunk>();
        chunks[x, 0, z].worldGO = gameObject;
        chunks[x, 0, z].chunkSize = chunkSize;
        chunks[x, 0, z].chunkHeight = chunkHeight;
        chunks[x, 0, z].chunkX = x * chunkSize;
        chunks[x, 0, z].chunkY = 0;
        chunks[x, 0, z].chunkZ = z * chunkSize;

        saver.SaveChunkData(newChunk.GetComponent<Chunk>());
        saver.SaveGlobalData();
    }

    public void GenerateChunk(int xPos, int zPos)
    {
        for (int x = xPos; x < xPos + chunkSize; x++)
        {
            for (int z = zPos; z < zPos + chunkSize; z++)
            {
                int y = PerlinNoise(x, z, 40, 45);

                if (y <= 0)
                {
                    y = 1;
                }
                if (y <= 8)
                {

                    data[x, y, z] = 3;
                }
                else if (y <= 15)
                {
                    data[x, y, z] = 2;
                }
                else
                {
                    data[x, y, z] = 4;
                    if (y >= 35)
                    {
                        data[x, y + 1, z] = 5;
                    }
                }


                for (int i = y - 1; i >= 0; i--)
                {
                    if (i != 0)
                    {
                        if (i <= 15)
                        {
                            data[x, i, z] = 2;
                        }
                        else
                        {
                            data[x, i, z] = 4;
                        }
                    }
                    else
                    {
                        data[x, i, z] = 1;
                    }
                }
            }
        }
        LoadChunk(xPos / chunkSize, zPos / chunkSize);
    }

    public void DeleteChunk(int x, int z)
    {
        if (ChunkExists(x, z) == true)
        {
            Destroy(chunks[x, 0, z].gameObject);
        }
    }

    public void CheckChunk(int xPos, int zPos, int dis)
    {
        for (int x = xPos - dis; x < xPos + dis; x++)
        {
            for (int z = zPos - dis; z < zPos + dis; z++)
            {
                if (ChunkExists(x, z) == true)
                {
                    if (GameObject.Find("Chunk[" + x + "," + 0 + "," + z + "]") == null)
                    {
                        LoadChunk(x, z);
                    }
                }
                else
                {
                    GenerateChunk(x * chunkSize, z * chunkSize);
                }
            }
        }
    }

    private bool ChunkExists(int x, int z)
    {
        if (chunks[x, 0, z] == null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public byte Block(int x, int y, int z)
    {
        if (x >= worldX || x < 0 || y >= worldY || y < 0 || z >= worldZ || z < 0)
        {
            return 1;
        }

        return data[x, y, z];
    }

    private int PerlinNoise(int x, int z, float scale, float height)
    {
        float value;
        value = Mathf.PerlinNoise(x / scale + offsetX, z / scale + offsetZ);
        value *= height;

        return (int)value;
    }
}
