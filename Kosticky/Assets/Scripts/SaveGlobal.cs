using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveGlobal
{
    public int offsetX;
    public int offsetZ;
    public float[] position;

    public SaveGlobal(Vector3 pos, int offsetX, int offsetZ)
    {
        this.offsetX = offsetX;
        this.offsetZ = offsetZ;

        position = new float[3];
        position[0] = pos.x;
        position[1] = pos.y;
        position[2] = pos.z;
    }
}
