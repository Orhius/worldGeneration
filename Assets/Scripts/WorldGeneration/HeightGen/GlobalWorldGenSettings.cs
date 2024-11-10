using System;
using UnityEngine;

[Serializable]
public class GlobalWorldGenSettings
{
    [Header("global")]
    public string worldName = "New world";
    public int chunkRenderingDistance = 8;
    [Header("world gen")]
    public ulong width = 256;
    public ulong Width
    {
        get { return width; }
        set
        {
            if (value > ulong.MaxValue) width = ulong.MaxValue;
            else if (value < ulong.MinValue) width = 1;
            else width = value;
        }
    }
    public ulong length = 256;
    public ulong Length
    {
        get { return length; }
        set
        {
            if (value > ulong.MaxValue) length = ulong.MaxValue;
            else if (value < ulong.MinValue) length = 1;
            else length = value;
        }
    }
    public int height = 32;
    public ushort primaryChunkSize = 256;
    public byte chunkSize = 16;
    public ulong seed = 0;
    public ulong Seed
    {
        get { return seed; }
        set
        {
            if (value > ulong.MaxValue) seed = ulong.MaxValue;
            else if (value < ulong.MinValue) seed = 0;
            else seed = value;
        }
    }

    public WorldType worldType = WorldType.continents;
}
public enum WorldType
{
    continents = 0,
    archipelagoes = 1,
    islands = 2
}