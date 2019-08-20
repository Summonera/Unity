using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifyTerrain : MonoBehaviour
{
    [HideInInspector]public GameObject playerGO;
    [SerializeField] GameObject wireframeGO;

    [SerializeField] float stoneDestroyingTime;
    [SerializeField] float sandDestroyingTime;
    [SerializeField] float dirtDestroyingTime;
    [SerializeField] float snowDestroyingTime;

    private GameObject temp;
    private World world;
    private  UiController ui;
    private Vector3 currentDestroying = Vector3.zero;
    private float destryoingTime;
    private int xPos;
    private int zPos;
    private int dis;

    void Start()
    {
        world = gameObject.GetComponent<World>();
        ui = gameObject.GetComponent<UiController>();
    }

    private void Update()
    {
        xPos = (int)(playerGO.transform.position.x / world.chunkSize);
        zPos = (int)(playerGO.transform.position.z / world.chunkSize);
        dis = world.renderDistance / world.chunkSize;

        LoadChunks(playerGO.transform.position);

        if (Input.GetMouseButton(0))
        {
            ReplaceBlockCursor(0);
            destryoingTime += Time.deltaTime;
            ui.UpdateSlider(destryoingTime);
        }

        if (Input.GetMouseButtonUp(0))
        {
            destryoingTime = 0;
            ui.ResetSlider();
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (temp != null)
            {
                SetBlockAt((int)temp.transform.position.x, (int)temp.transform.position.y, (int)temp.transform.position.z, ui.currentByte);

            }
        }

        if (Input.mouseScrollDelta.y < 0)
        {
            ui.GetInventoryIndex(-1);
        }

        if (Input.mouseScrollDelta.y > 0)
        {
            ui.GetInventoryIndex(1);
        }

        AddBlockCursor(0);
    }

    public void ReplaceBlockCenter(float range, byte block)
    {
        Ray ray = new Ray(playerGO.transform.position, playerGO.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.distance < range)
            {
                ReplaceBlockAt(hit, block);
            }
        }
    }

    public void AddBlockCenter(float range, byte block)
    {
        Ray ray = new Ray(playerGO.transform.position, playerGO.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {

            if (hit.distance < range)
            {
                AddBlockAt(hit, block);
            }
        }
    }

    public void ReplaceBlockCursor(byte block)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 5f))
        {
            ReplaceBlockAt(hit, block);
        }
    }

    public void AddBlockCursor(byte block)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 5f))
        {
            AddBlockAt(hit, block);
        }
        else
        {
            Destroy(temp);
        }
    }

    public void ReplaceBlockAt(RaycastHit hit, byte block)
    {
        Vector3 position = hit.point;
        position += (hit.normal * -0.5f);
        SetBlockAt(position, block);
    }

    public void AddBlockAt(RaycastHit hit, byte block)
    {
        Vector3 position = hit.point;
        position += (hit.normal * 0.5f);
        ShowBlockAt(position, block);
    }

    public void SetBlockAt(Vector3 position, byte block)
    {
        int x = Mathf.RoundToInt(position.x);
        int y = Mathf.RoundToInt(position.y);
        int z = Mathf.RoundToInt(position.z);
        Vector3 thisDestroying = new Vector3(x, y, z);

        if (thisDestroying == currentDestroying)
        {
            GetBlockID(x, y, z);
        }
        else
        {
            destryoingTime = 0f;
            currentDestroying = thisDestroying;
        }
    }

    public void ShowBlockAt(Vector3 position, byte block)
    {
        int x = Mathf.RoundToInt(position.x);
        int y = Mathf.RoundToInt(position.y);
        int z = Mathf.RoundToInt(position.z);

        int playerX = Mathf.RoundToInt(playerGO.transform.position.x);
        int playerY = Mathf.RoundToInt(playerGO.transform.position.y);
        int playerZ = Mathf.RoundToInt(playerGO.transform.position.z);

        if (playerX == x && (playerY == y || playerY + 1 == y) && playerZ == z)
        {
            Destroy(temp);

        }
        else
        {
            Destroy(temp);
            temp = GameObject.Instantiate(wireframeGO, new Vector3(x, y, z), Quaternion.identity);
        }

    }

    public void GetBlockID(int x, int y, int z)
    {
        int id = world.data[x, y, z];
        if (id == 2)
        {
            ui.SetSlider(stoneDestroyingTime);
            if (destryoingTime >= stoneDestroyingTime)
            {
                destryoingTime = 0;
                SetBlockAt(x, y, z, 0);
            }
        }
        else if (id == 3)
        {
            ui.SetSlider(sandDestroyingTime);
            if (destryoingTime >= sandDestroyingTime)
            {
                destryoingTime = 0;
                SetBlockAt(x, y, z, 0);
            }
        }
        else if (id == 4)
        {
            ui.SetSlider(dirtDestroyingTime);
            if (destryoingTime >= dirtDestroyingTime)
            {
                destryoingTime = 0;
                SetBlockAt(x, y, z, 0);
            }
        }
        else if (id == 5)
        {
            ui.SetSlider(snowDestroyingTime);
            if (destryoingTime >= snowDestroyingTime)
            {
                destryoingTime = 0;
                SetBlockAt(x, y, z, 0);
            }
        }
    }

    public void SetBlockAt(int x, int y, int z, byte block)
    {
        ui.ResetSlider();
        world.data[x, y, z] = block;
        UpdateChunkAt(x, y, z);
        world.saver.SaveChunkData(world.chunks[x / world.chunkSize, 0, z / world.chunkSize]);
        world.saver.SaveGlobalData();
    }

    public void UpdateChunkAt(int x, int y, int z)
    {
        int updateX = Mathf.FloorToInt(x / world.chunkSize);
        int updateY = Mathf.FloorToInt(y / world.chunkHeight);
        int updateZ = Mathf.FloorToInt(z / world.chunkSize);

        world.chunks[updateX, updateY, updateZ].update = true;

        if (x - (world.chunkSize * updateX) == 0 && updateX != 0)
        {
            world.chunks[updateX - 1, updateY, updateZ].update = true;
        }

        if (x - (world.chunkSize * updateX) == 15 && updateX != world.chunks.GetLength(0) - 1)
        {
            world.chunks[updateX + 1, updateY, updateZ].update = true;
        }

        if (y - (world.chunkHeight * updateY) == 0 && updateY != 0)
        {
            world.chunks[updateX, updateY - 1, updateZ].update = true;
        }

        if (y - (world.chunkHeight * updateY) == 15 && updateY != world.chunks.GetLength(1) - 1)
        {
            world.chunks[updateX, updateY + 1, updateZ].update = true;
        }

        if (z - (world.chunkSize * updateZ) == 0 && updateZ != 0)
        {
            world.chunks[updateX, updateY, updateZ - 1].update = true;
        }

        if (z - (world.chunkSize * updateZ) == 15 && updateZ != world.chunks.GetLength(2) - 1)
        {
            world.chunks[updateX, updateY, updateZ + 1].update = true;
        }
    }

    public void LoadChunks(Vector3 playerPos)
    {
        for (int x = xPos - dis - 1; x < xPos + dis + 1; x++)
        {
            for (int z = zPos - dis - 1; z < zPos + dis + 1; z++)
            {
                if (x == xPos - dis - 1 || x == xPos + dis || z == zPos - dis - 1 || z == zPos + dis)
                {
                    world.DeleteChunk(x, z);
                }
                else if (world.chunks[x, 0, z] == null)
                {
                    world.CheckChunk(xPos, zPos, dis);
                }

            }
        }

    }
}
