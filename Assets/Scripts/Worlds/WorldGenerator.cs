using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.SceneManagement;

[BurstCompile]
public class WorldGenerator : MonoBehaviour
{
    public static World currentWorld = new World();
    [SerializeField] private string GameSceneName = "GameScene";
    [SerializeField] private Chunk chunkObject;

    [SerializeField] private List<FastNoiseSettings> noiseSettings = new List<FastNoiseSettings>();
    [SerializeField] private FastNoiseSettings warpNoiseSettings;

    public static Dictionary<Vector2Int, Chunk> ChunkData = new();

    private static List<FastNoiseLite> noiseOctaves = new List<FastNoiseLite>();
    private static List<float> noiseAmplitude = new List<float>();

    public static int chunkSize = 16;
    public static int chunkHeight = 128;
    public static int baseChunkHeight = chunkHeight/2;
    public static byte chunkRenderingDistance = 8;

    public static SimplexNoise.Layer noiseHeigthLayer = new();
    public static FastNoiseLite noiseLite = new FastNoiseLite();
    public static FastNoiseLite warpNoiseLite = new FastNoiseLite();

    public static event Action<World> OnStartWorldGeneratorLoad;

    public ConcurrentQueue<Chunk> ChunksMeshQueue = new ConcurrentQueue<Chunk>();

    public bool isGenerating = false;
    [ReadOnly] int maxChunkGenSteps = (chunkRenderingDistance * 2 + 1) * (chunkRenderingDistance * 2 + 1);
    [ReadOnly] int maxChunkInitSteps = (chunkRenderingDistance * 2 + 3) * (chunkRenderingDistance * 2 + 3);

    private void OnEnable()
    {
        DontDestroyOnLoad(gameObject);
        OnStartWorldGeneratorLoad += LoadGameScene;
        GlobalEventManager.OnPlayerChunkPositionChanged += GenerateChunks;

        noiseLite.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
    }
    private void OnDisable()
    {
        OnStartWorldGeneratorLoad -= LoadGameScene;
        GlobalEventManager.OnPlayerChunkPositionChanged += GenerateChunks;

    }
    private void Awake()
    {
        baseChunkHeight = chunkHeight / 2;
        for (int i = 0; i < noiseSettings.Count; i++)
        {
            noiseOctaves.Add(new FastNoiseLite());
            noiseOctaves[i].SetNoiseType(noiseSettings[i].type);
            noiseOctaves[i].SetFrequency(noiseSettings[i].friquency);
            noiseAmplitude.Add(noiseSettings[i].amplitude);
        }
        warpNoiseLite.SetNoiseType(warpNoiseSettings.type);
        warpNoiseLite.SetFrequency(warpNoiseSettings.friquency);
        warpNoiseLite.SetDomainWarpAmp(warpNoiseSettings.amplitude);

        StartCoroutine(ChunkGenerationCoroutine());
    }
    public static void StartWorldGeneratorLoad(World world)
    {
        OnStartWorldGeneratorLoad?.Invoke(world);
    }
    public void LoadGameScene(World world)
    {
        Debug.Log(world.data.WorldName);
        World thisWorld = WorldsManager.LoadWorldData(world.data.WorldName);
        currentWorld = new World(thisWorld.settings, thisWorld.data);
        chunkSize = currentWorld.settings.globalWorldGenSettings.chunkSize;
        chunkHeight = currentWorld.settings.globalWorldGenSettings.height;
        chunkRenderingDistance = currentWorld.settings.globalWorldGenSettings.chunkRenderingDistance;

        SceneManager.LoadScene(GameSceneName);

        GlobalEventManager.WorldSceneIsLoaded();
    }
    private void GenerateChunks(Vector2Int vector)
    {
        if (isGenerating) { return; }
        isGenerating = true;
        StartCoroutine(GenerateChunksCoroutine(vector));
    }
    private IEnumerator GenerateChunksCoroutine(Vector2Int vector)
    {
        List<Vector2Int> newChunkData = new();
        List<Vector2Int> oldChunkData = new();
        List<Vector2Int> overdrawingTempChunks = new();
        int x = 0;
        int z = 0;
        int dx = 0;
        int dz = -1;
        for (int i = 0; i < maxChunkInitSteps; i++)
        {
            if (!ChunkData.ContainsKey(new Vector2Int(x + vector.x, z + vector.y)))
            {
                var chunkObj = Instantiate(chunkObject, new Vector3((x + vector.x) * chunkSize, 0, (z + vector.y) * chunkSize), Quaternion.identity);

                chunkObj.InitChunk();
                ChunkData.Add(new Vector2Int(x + vector.x, z + vector.y), chunkObj);
                if (i > maxChunkGenSteps)
                {
                    overdrawingTempChunks.Add(new Vector2Int(x + vector.x, z + vector.y));
                }
            }
            else
            {
                oldChunkData.Add(new Vector2Int(x + vector.x, z + vector.y));
            }
            if (i < maxChunkGenSteps) newChunkData.Add(new Vector2Int(x + vector.x, z + vector.y));

            if (x == z || (x < 0 && x == - z) || (x > 0 && x == 1 - z))
            {
                (dx, dz) = (-dz, dx);
            }
            x += dx;
            z += dz;
        }
        x = 0;
        z = 0;
        dx = 0;
        dz = -1;
        for (int i = 0; i < maxChunkGenSteps; i++)
        {
            if(ChunkData.Keys.Except(oldChunkData).ToList().Contains(new Vector2Int(x + vector.x, z + vector.y)))
            {
                yield return new WaitForSeconds(0.001f);
                ChunkData.TryGetValue(new Vector2Int(x + vector.x, z + vector.y), out Chunk chunk);

                if (chunk != null)
                {
                    NativeArray<BlockData> blocksLeft = new NativeArray<BlockData>(chunkSize * chunkHeight * chunkSize, Allocator.TempJob);
                    NativeArray<BlockData> blocksRight = new NativeArray<BlockData>(chunkSize * chunkHeight * chunkSize, Allocator.TempJob);
                    NativeArray<BlockData> blocksForward = new NativeArray<BlockData>(chunkSize * chunkHeight * chunkSize, Allocator.TempJob);
                    NativeArray<BlockData> blocksBack = new NativeArray<BlockData>(chunkSize * chunkHeight * chunkSize, Allocator.TempJob);
                    if (ChunkData.TryGetValue(new Vector2Int(chunk.position.x / chunkSize, chunk.position.y / chunkSize) - new Vector2Int(1, 0), out Chunk ChunkLeft))
                    {
                        blocksLeft = ChunkLeft.blocks;
                    }
                    if(ChunkData.TryGetValue(new Vector2Int(chunk.position.x / chunkSize, chunk.position.y / chunkSize) + new Vector2Int(1, 0), out Chunk ChunkRight))
                    {
                        blocksRight = ChunkRight.blocks;
                    }
                    if(ChunkData.TryGetValue(new Vector2Int(chunk.position.x / chunkSize, chunk.position.y / chunkSize) + new Vector2Int(0, 1), out Chunk ChunkForward))
                    {
                        blocksForward = ChunkForward.blocks;
                    }
                    if(ChunkData.TryGetValue(new Vector2Int(chunk.position.x / chunkSize, chunk.position.y / chunkSize) - new Vector2Int(0, 1), out Chunk ChunkBack))
                    {
                        blocksBack = ChunkBack.blocks;
                    }

                    ChunkGenerationJob chunkJob = new ChunkGenerationJob
                    {
                        blocks = chunk.blocks,
                        blocksLeftChunk = blocksLeft,
                        blocksRightChunk = blocksRight,
                        blocksForwardChunk = blocksBack,
                        blocksBackChunk = blocksForward,
                        position = chunk.position,
                        vertices = chunk.vertices,
                        triangles = chunk.triangles
                    };

                    JobHandle handle = chunkJob.Schedule();

                    yield return new WaitUntil(() => handle.IsCompleted);
                    handle.Complete();

                    ChunksMeshQueue.Enqueue(chunk);
                }
            }

            if (x == z || (x < 0 && x == -z) || (x > 0 && x == 1 - z))
            {
                (dx, dz) = (-dz, dx);
            }
            x += dx;
            z += dz;
        }
        foreach (var item in ChunkData.Keys.Except(newChunkData).ToList())
        {
            GameObject temp = ChunkData[item].gameObject;
            ChunkData.Remove(item);
            Destroy(temp);
        }
        newChunkData.Clear();
        foreach (var item in ChunkData.Keys.Intersect(overdrawingTempChunks).ToList())
        {
            GameObject temp = ChunkData[item].gameObject;
            ChunkData.Remove(item);
            Destroy(temp);
        }
        overdrawingTempChunks.Clear();
        oldChunkData.Clear();
        isGenerating = false;
    }
    public IEnumerator ChunkGenerationCoroutine()
    {
        while (true)
        {
            yield return new WaitUntil(() => ChunksMeshQueue.Count() > 0);
            ChunksMeshQueue.TryDequeue(out var chunk);
            chunk.GenerateChunk();
            yield return null;
        }
    }
    public static float GenerateHeight(float x, float y)
    {
        x += currentWorld.settings.globalWorldGenSettings.seed;
        y += currentWorld.settings.globalWorldGenSettings.seed;
        warpNoiseLite.DomainWarp(ref x, ref y);

        float noise = (float)baseChunkHeight;
        for (int i = 0; i < noiseOctaves.Count; i++)
        {
            float noiseOctave = noiseOctaves[i].GetNoise(x, y);
            noise += noiseOctave * noiseAmplitude[i];
        }
        return noise;
    }
}
