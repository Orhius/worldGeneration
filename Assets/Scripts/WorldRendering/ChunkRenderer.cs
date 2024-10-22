using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ChunkRenderer : MonoBehaviour
{
    public Chunk chunk;

    private List<Vector3> vertices = new();
    private List<int> triangles = new();

    public void GenerateChunk()
    {
        Mesh chunkMesh = new();
        MeshFilter meshFilter = GetComponent<MeshFilter>();

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
            return chunk.blocks[coordinates.x, coordinates.y, coordinates.z].blockType;
        }
        else
        {
            return BlockType.Air;
        }
    }
    #endregion
}
