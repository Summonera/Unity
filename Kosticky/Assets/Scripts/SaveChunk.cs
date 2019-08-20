using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveChunk
{
    public byte[] byteArray;
    public int chunkIdX;
    public int chunkIdZ;
    public int chunkPosX;
    public int chunkPosZ;

    public SaveChunk(Chunk chunk)
    {
        chunkIdX = chunk.chunkX / chunk.chunkSize;
        chunkIdZ = chunk.chunkZ / chunk.chunkSize;
        chunkPosX = chunk.chunkX;
        chunkPosZ = chunk.chunkZ;
        byteArray = new byte[chunk.chunkSize * chunk.chunkSize * chunk.chunkHeight];
    }

}
