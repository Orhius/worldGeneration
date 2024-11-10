using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
[Serializable]
public class Chunk : MonoBehaviour
{
    public BlockData[,,] blocks = new BlockData[WorldGenerator.chunkSize, WorldGenerator.chunkHeight, WorldGenerator.chunkSize];
    public Vector2Int position = new Vector2Int();

    private List<Vector3> vertices = new();
    private List<int> triangles = new();

    public SimplexNoise.Layer noiseHeigthLayer;

    public bool isGenerated = false;

    private event Action OnAllBlockGenerated;

    private void OnEnable()
    {
        OnAllBlockGenerated += GenerateChunk;
    }
    private void OnDisable()
    {
        OnAllBlockGenerated -= GenerateChunk;
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
                    blocks[x, y, z] = new BlockData();
                }
            }
        }

        GenerateHeight();
    }

    public void GenerateHeight()
    {
        //for (int x = 0; x < WorldGenerator.chunkSize; x++)
        //{
        //    for (int y = 0; y < WorldGenerator.chunkSize; y++)
        //    {
        //        float height = WorldGenerator.GenerateHeight(position.x + x, position.y + y);
        //        if (WorldGenerator.chunkHeight > height)
        //        {
        //            for (int z = 0; z < height; z++)
        //            {
        //                blocks[x, z, y].blockType = BlockType.Surface;
        //            }
        //        }
        //        else
        //        {
        //            for (int z = 0; z < WorldGenerator.chunkHeight; z++)
        //            {
        //                blocks[x, z, y].blockType = BlockType.Surface;
        //            }
        //        }
        //    }
        //}
        List<Task> heightTasks = new List<Task>();

        for (int x = 0; x < WorldGenerator.chunkSize; x++)
        {
            for (int y = 0; y < WorldGenerator.chunkSize; y++)
            {
                heightTasks.Add(Task.Run(() =>
                {
                    float height = WorldGenerator.GenerateHeight(position.x + x, position.y + y);
                    if (WorldGenerator.chunkHeight > height)
                    {
                        for (int z = 0; z < height; z++)
                        {
                            blocks[x, z, y].blockType = BlockType.Surface;
                        }
                    }
                    else
                    {
                        for (int z = 0; z < WorldGenerator.chunkHeight; z++)
                        {
                            blocks[x, z, y].blockType = BlockType.Surface;
                        }
                    }
                }));
            }
        }
    }
    #region Chunk Rendering

    public Task GenerateChunkMesh()
    {
        return Task.Factory.StartNew(() =>
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
            isGenerated = true;
            //OnAllBlockGenerated?.Invoke();
        }, TaskCreationOptions.LongRunning);
    }
    public void GenerateChunk()
    {
        Mesh chunkMesh = new Mesh();
        MeshFilter meshFilter = GetComponent<MeshFilter>();

        chunkMesh.vertices = vertices.ToArray();
        chunkMesh.triangles = triangles.ToArray();

        chunkMesh.Optimize();
        chunkMesh.RecalculateBounds();
        chunkMesh.RecalculateNormals();

        meshFilter.mesh = chunkMesh;
    }

    #region Block Gen
    private void GenerateBlock(Vector3Int coordinates)
    {
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
        triangles.Add(vertices.Count - 4);
        triangles.Add(vertices.Count - 3);
        triangles.Add(vertices.Count - 2);

        triangles.Add(vertices.Count - 3);
        triangles.Add(vertices.Count - 1);
        triangles.Add(vertices.Count - 2);
    }

    private BlockType GetBlock(Vector3Int coordinates)
    {
        if (coordinates.x >= 0 && coordinates.x < WorldGenerator.chunkSize
            && coordinates.y >= 0 && coordinates.y < WorldGenerator.chunkHeight
            && coordinates.z >= 0 && coordinates.z < WorldGenerator.chunkSize)
        {
            return blocks[coordinates.x, coordinates.y, coordinates.z].blockType;
        }
        else
        {
            Vector2Int nextChunkPos = new Vector2Int(position.x / WorldGenerator.chunkSize, position.y / WorldGenerator.chunkSize);

            if (coordinates.y >= WorldGenerator.chunkHeight) return BlockType.Air;
            else if(coordinates.y < 0) return BlockType.Surface;


            if (coordinates.x < 0)
            {
                nextChunkPos.x--;
                coordinates.x = WorldGenerator.chunkSize - 1;
            }
            else if (coordinates.x >= WorldGenerator.chunkSize)
            {
                nextChunkPos.x++;
                coordinates.x = 0;
            }

            if (coordinates.z < 0)
            {
                nextChunkPos.y--;
                coordinates.z = WorldGenerator.chunkSize - 1;
            }
            else if(coordinates.z >= WorldGenerator.chunkSize)
            {
                nextChunkPos.y++;
                coordinates.z = 0;
            }

            if (WorldGenerator.ChunkData.TryGetValue(nextChunkPos, out Chunk nextChunk))
            {
                return nextChunk.blocks[coordinates.x, coordinates.y, coordinates.z].blockType;
            }
            else
            {
                return BlockType.Air;
            }
        }
    }
    #endregion

    #endregion
}
public struct MeshVertex
{
    public Vector3 position;
    public sbyte normalX, normalY, normalZ, normalW;
}
public class ChunkMeshData
{
    public MeshVertex[] vertices;
    public Bounds bounds;
}