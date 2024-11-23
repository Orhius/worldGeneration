using System;
using Unity.Burst;
using Unity.Collections;
using UnityEngine;
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
[Serializable]
[BurstCompile]
public class Chunk : MonoBehaviour
{
    public GameObject thisChunk = null;
    public NativeArray<BlockData> blocks = new NativeArray<BlockData>(WorldGenerator.chunkSize * WorldGenerator.chunkHeight *  WorldGenerator.chunkSize, Allocator.Persistent);
    public Vector2Int position = new Vector2Int();

    public NativeList<Vector3> vertices = new NativeList<Vector3>(0, Allocator.Persistent);
    public NativeList<int> triangles = new NativeList<int>(0, Allocator.Persistent);

    public SimplexNoise.Layer noiseHeigthLayer;

    public bool isGenerated = false;
    private void Awake()
    {
        thisChunk = gameObject;
    }
    public void InitChunk()
    {
        position.x = (int)transform.position.x;
        position.y = (int)transform.position.z;
        for (int x = 0; x < WorldGenerator.chunkSize; x++)
        {
            for (int y = 0; y < WorldGenerator.chunkHeight; y++)
            {
                for (int z = 0; z < WorldGenerator.chunkSize; z++)
                {
                    int index = x + z * WorldGenerator.chunkSize + y * WorldGenerator.chunkSize * WorldGenerator.chunkSize;
                    blocks[index] = new BlockData();
                }
            }
        }

        GenerateHeight();
    }
    public void GenerateHeight()
    {
        BlockData blockTemp = new BlockData();
        for (int x = 0; x < WorldGenerator.chunkSize; x++)
        {
            for (int y = 0; y < WorldGenerator.chunkSize; y++)
            {
                float height = WorldGenerator.GenerateHeight(position.x + x, position.y + y);
                if (WorldGenerator.chunkHeight > height)
                {
                    for (int z = 0; z < height; z++)
                    {
                        int index = x + y * WorldGenerator.chunkSize + z * WorldGenerator.chunkSize * WorldGenerator.chunkSize;
                        blockTemp = blocks[index];
                        blockTemp.blockType = BlockType.Surface;
                        blocks[index] = blockTemp;
                    }
                }
                else
                {
                    for (int z = 0; z < WorldGenerator.chunkHeight; z++)
                    {
                        int index = x + y * WorldGenerator.chunkSize + z * WorldGenerator.chunkSize * WorldGenerator.chunkSize;
                        blockTemp = blocks[index];
                        blockTemp.blockType = BlockType.Surface;
                        blocks[index] = blockTemp;

                    }
                }
            }
        }
    }
    public void GenerateChunk()
    {
        Mesh chunkMesh = new Mesh();
        MeshFilter meshFilter = GetComponent<MeshFilter>();

        chunkMesh.vertices = vertices.AsArray().ToArray();
        chunkMesh.triangles = triangles.AsArray().ToArray();

        chunkMesh.Optimize();
        chunkMesh.RecalculateBounds();
        chunkMesh.RecalculateNormals();

        meshFilter.mesh = chunkMesh;
    }
}