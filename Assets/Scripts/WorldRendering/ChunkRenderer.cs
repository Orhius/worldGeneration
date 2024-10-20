using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ChunkRenderer : MonoBehaviour
{
    private static int chunkSize = WorldGenerator.currentWorld.settings.globalWorldGenSettings.chunkSize;
    private static int chunkHeight = WorldGenerator.currentWorld.settings.globalWorldGenSettings.height;

    public int[,,] Blocks = new int[chunkSize, chunkHeight, chunkSize];

    private List<Vector3> vertices = new();
    private List<int> triangles = new();

    private void Start()
    {
        Mesh chunkMesh = new();


        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkHeight; y++)
            {
                for (int z = 0; z < chunkSize; z++)
                {
                    Blocks[x, y, z] = 1;
                }
            }
        }

        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkHeight; y++)
            {
                for(int z = 0; z < chunkSize; z++)
                {
                    GenerateBlock(new Vector3Int(x, y, z));
                }
            }
        }
        
        chunkMesh.RecalculateBounds();
        chunkMesh.RecalculateNormals();

        chunkMesh.vertices = vertices.ToArray();
        chunkMesh.triangles = triangles.ToArray();

        GetComponent<MeshFilter>().mesh = chunkMesh;
    }

    #region Block Gen
    private void GenerateBlock(Vector3Int coordinates)
    {
        if (GetBlock(coordinates) == 0) return;

        if (GetBlock(coordinates + Vector3Int.right) == 0) GenerateRightSide(coordinates);
        if (GetBlock(coordinates + Vector3Int.left) == 0) GenerateLeftSide(coordinates);
        if (GetBlock(coordinates + Vector3Int.back) == 0) GenerateFrontSide(coordinates);
        if (GetBlock(coordinates + Vector3Int.forward) == 0) GenerateBackSide(coordinates);
        if (GetBlock(coordinates + Vector3Int.up) == 0) GenerateUpSide(coordinates);
        if (GetBlock(coordinates + Vector3Int.down) == 0) GenerateBottomSide(coordinates); 
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

    private int GetBlock(Vector3Int coordinates)
    {
        Debug.Log(coordinates);
        if (coordinates.x >= 0 && coordinates.x < chunkSize
            && coordinates.y >= 0 && coordinates.y < chunkHeight
            && coordinates.z >= 0 && coordinates.z < chunkSize)
        {
            return Blocks[coordinates.x, coordinates.y, coordinates.z];
        }
        else
        {
            return 0;
        }
    }
    #endregion
}
