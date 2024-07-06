using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalWorldGenSettings : MonoBehaviour
{
    [Header("world gen")]
    public long width = 256;
    public long length = 256;
    public int height = 32;
    public uint primaryChunkSize = 256;
    public byte chunkSize = 16;
    public uint seed = 0;
    public WorldType worldType = WorldType.continents;

    private void Awake()
    {
        seed = (uint)Random.Range(0, 99999);
    }

    public void ChangeWorldWidth(long width)
    {
        this.width = width;
    }
    public void ChangeWorldLength(long length)
    {
        this.length = length;
    }
    public void ChangeWorldHeight(int height)
    {
        this.height = height;
    }
    public void ChangeSeedRandom()
    {
        this.seed = (uint)Random.Range(0, 99999);
    }
    public void ChangePrimaryChunkSize(uint primaryChunkSize)
    {
        this.primaryChunkSize = primaryChunkSize;
    }
    public void ChangeChunkSize(byte chunkSize)
    {
        this.chunkSize = chunkSize;
    }
    public void ChangeWorldType(WorldType worldType)
    {
        this.worldType = worldType;
    }
}
public enum WorldType
{
    continents = 0,
    archipelagoes = 1,
    islands = 2
}