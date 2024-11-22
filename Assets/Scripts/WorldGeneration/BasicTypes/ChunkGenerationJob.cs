using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

[BurstCompile]
public struct ChunkGenerationJob : IJob
{
    public NativeArray<BlockData> blocks;
    public NativeArray<BlockData> blocksLeftChunk;
    public NativeArray<BlockData> blocksRightChunk;
    public NativeArray<BlockData> blocksForwardChunk;
    public NativeArray<BlockData> blocksBackChunk;
    public Vector2Int position;
    public NativeList<Vector3> vertices;
    public NativeList<int> triangles;
    void IJob.Execute()
    {
        for (int x = 0; x < WorldGenerator.chunkSize; x++)
        {
            for (int y = 0; y < WorldGenerator.chunkHeight; y++)
            {
                for (int z = 0; z < WorldGenerator.chunkSize; z++)
                {
                    GenerateBlock(new Vector3Int(x, y, z));
                }
            }
        }
    }
    #region Block Gen
    private void GenerateBlock(Vector3Int coordinates)
    {
        int index = coordinates.x + coordinates.z * WorldGenerator.chunkSize + coordinates.y * WorldGenerator.chunkSize * WorldGenerator.chunkSize;

        if (GetBlock(coordinates) == BlockType.Air) return;

        if (GetBlock(coordinates + Vector3Int.right) == BlockType.Air) GenerateRightSide(coordinates);
        if (GetBlock(coordinates + Vector3Int.left) == BlockType.Air) GenerateLeftSide(coordinates);
        if (GetBlock(coordinates + Vector3Int.back) == BlockType.Air) GenerateFrontSide(coordinates);
        if (GetBlock(coordinates + Vector3Int.forward) == BlockType.Air) GenerateBackSide(coordinates);
        if (GetBlock(coordinates + Vector3Int.up) == BlockType.Air) GenerateUpSide(coordinates);
        if (GetBlock(coordinates + Vector3Int.down) == BlockType.Air) GenerateBottomSide(coordinates);
    }
    private void GenerateRightSide(Vector3Int coordinates)
    {
        vertices.Add(new Vector3(1, 0, 0) + coordinates);
        vertices.Add(new Vector3(1, 1, 0) + coordinates);
        vertices.Add(new Vector3(1, 0, 1) + coordinates);
        vertices.Add(new Vector3(1, 1, 1) + coordinates);


        AddLastVertices();
    }
    private void GenerateLeftSide(Vector3Int coordinates)
    {
        vertices.Add(new Vector3(0, 0, 0) + coordinates);
        vertices.Add(new Vector3(0, 0, 1) + coordinates);
        vertices.Add(new Vector3(0, 1, 0) + coordinates);
        vertices.Add(new Vector3(0, 1, 1) + coordinates);

        AddLastVertices();
    }
    private void GenerateFrontSide(Vector3Int coordinates)
    {
        vertices.Add(new Vector3(0, 0, 0) + coordinates);
        vertices.Add(new Vector3(0, 1, 0) + coordinates);
        vertices.Add(new Vector3(1, 0, 0) + coordinates);
        vertices.Add(new Vector3(1, 1, 0) + coordinates);

        AddLastVertices();
    }
    private void GenerateBackSide(Vector3Int coordinates)
    {
        vertices.Add(new Vector3(0, 0, 1) + coordinates);
        vertices.Add(new Vector3(1, 0, 1) + coordinates);
        vertices.Add(new Vector3(0, 1, 1) + coordinates);
        vertices.Add(new Vector3(1, 1, 1) + coordinates);

        AddLastVertices();
    }
    private void GenerateUpSide(Vector3Int coordinates)
    {
        vertices.Add(new Vector3(0, 1, 0) + coordinates);
        vertices.Add(new Vector3(0, 1, 1) + coordinates);
        vertices.Add(new Vector3(1, 1, 0) + coordinates);
        vertices.Add(new Vector3(1, 1, 1) + coordinates);

        AddLastVertices();
    }
    private void GenerateBottomSide(Vector3Int coordinates)
    {
        vertices.Add(new Vector3(0, 0, 0) + coordinates);
        vertices.Add(new Vector3(1, 0, 0) + coordinates);
        vertices.Add(new Vector3(0, 0, 1) + coordinates);
        vertices.Add(new Vector3(1, 0, 1) + coordinates);

        AddLastVertices();
    }
    private void AddLastVertices()
    {
        if (vertices.Length < 4)
        {
            Debug.LogError("Not enough vertices to form a triangle!");
            return;
        }
        triangles.Add(vertices.Length - 4);
        triangles.Add(vertices.Length - 3);
        triangles.Add(vertices.Length - 2);

        triangles.Add(vertices.Length - 3);
        triangles.Add(vertices.Length - 1);
        triangles.Add(vertices.Length - 2);
    }
    public BlockType GetBlock(Vector3Int coordinates)
    {
        if (coordinates.x >= 0 && coordinates.x < WorldGenerator.chunkSize
            && coordinates.y >= 0 && coordinates.y < WorldGenerator.chunkHeight
            && coordinates.z >= 0 && coordinates.z < WorldGenerator.chunkSize)
        {
            int index = coordinates.x + coordinates.z * WorldGenerator.chunkSize + coordinates.y * WorldGenerator.chunkSize * WorldGenerator.chunkSize;

            return blocks[index].blockType;
        }
        else
        {
            if (coordinates.y >= WorldGenerator.chunkHeight) return BlockType.Air;
            else if (coordinates.y < 0) return BlockType.Surface;


            if (coordinates.x < 0)
            {
                coordinates.x = WorldGenerator.chunkSize - 1;

                if (blocksLeftChunk != null && blocksLeftChunk.Length > 0)
                {
                    int index = coordinates.x + coordinates.z * WorldGenerator.chunkSize + coordinates.y * WorldGenerator.chunkSize * WorldGenerator.chunkSize;

                    return blocksLeftChunk[index].blockType;
                }
                return BlockType.Surface;

            }
            else if (coordinates.x >= WorldGenerator.chunkSize)
            {
                coordinates.x = 0;

                if (blocksRightChunk != null && blocksRightChunk.Length > 0)
                {
                    int index = coordinates.x + coordinates.z * WorldGenerator.chunkSize + coordinates.y * WorldGenerator.chunkSize * WorldGenerator.chunkSize;

                    return blocksRightChunk[index].blockType;
                }
                return BlockType.Surface;

            }

            if (coordinates.z < 0)
            {
                coordinates.z = WorldGenerator.chunkSize - 1;

                if (blocksForwardChunk != null && blocksForwardChunk.Length > 0)
                {

                    int index = coordinates.x + coordinates.z * WorldGenerator.chunkSize + coordinates.y * WorldGenerator.chunkSize * WorldGenerator.chunkSize;

                    return blocksForwardChunk[index].blockType;
                }
                return BlockType.Surface;

            }
            else if (coordinates.z >= WorldGenerator.chunkSize)
            {
                coordinates.z = 0;

                if (blocksBackChunk != null && blocksBackChunk.Length > 0)
                {

                    int index = coordinates.x + coordinates.z * WorldGenerator.chunkSize + coordinates.y * WorldGenerator.chunkSize * WorldGenerator.chunkSize;

                    return blocksBackChunk[index].blockType;
                }
                return BlockType.Surface;

            }
            return BlockType.Air;
        }
    }
    #endregion
}
