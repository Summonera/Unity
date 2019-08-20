using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class Saver : MonoBehaviour
{
    [SerializeField] World world;
    [HideInInspector] public GameObject playerGO;

    byte[] byteArray;

    public void SaveGlobalData()
    {
        if (!Directory.Exists(Application.persistentDataPath + "/SaveSlot"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/SaveSlot");
        }

        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/SaveSlot/GlobalData.save";
        FileStream stream = new FileStream(path, FileMode.Create);
        SaveGlobal data = new SaveGlobal(playerGO.transform.position, world.offsetX, world.offsetZ);
        formatter.Serialize(stream, data);
        stream.Close();
    }

    public void SaveChunkData(Chunk chunk)
    {
        if (!Directory.Exists(Application.persistentDataPath + "/SaveSlot"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/SaveSlot");
        }

        SaveChunk data = new SaveChunk(chunk);
        string path = Application.persistentDataPath + "/SaveSlot/Chunk[" + data.chunkIdX + ";" + data.chunkIdZ + "].save";


        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        SaveChunkBytes(data);
        formatter.Serialize(stream, data);
        stream.Close();

    }

    void SaveChunkBytes(SaveChunk chunk)
    {
        int i = 0;

        for (int x = chunk.chunkPosX; x < chunk.chunkPosX + world.chunkSize; x++)
        {
            for (int y = 0; y < world.chunkHeight; y++)
            {
                for (int z = chunk.chunkPosZ; z < chunk.chunkPosZ + world.chunkSize; z++)
                {
                    chunk.byteArray[i] = world.data[x, y, z];
                    i++;
                }
            }
        }
    }

    public SaveChunk LoadChunkData(int x, int z)
    { 
        string path = Application.persistentDataPath + "/SaveSlot/Chunk[" + x + ";" + z + "].save";

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            SaveChunk data = formatter.Deserialize(stream) as SaveChunk;
            stream.Close();
            return data;
        }
        else
        {
            return null;
        }
    }

    public SaveGlobal LoadGlobalData()
    {
        string path = Application.persistentDataPath + "/SaveSlot/GlobalData.save";

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            SaveGlobal data = formatter.Deserialize(stream) as SaveGlobal;
            stream.Close();
            return data;
        }
        else
        {
            return null;
        }
    }

    public bool isFileExists()
    {
        string path = Application.persistentDataPath + "/SaveSlot";

        if (File.Exists(path))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
