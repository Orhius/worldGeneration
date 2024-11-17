using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst;
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
    public static byte chunkRenderingDistance = 16;

    public static SimplexNoise.Layer noiseHeigthLayer = new();
    public static FastNoiseLite noiseLite = new FastNoiseLite();
    public static FastNoiseLite warpNoiseLite = new FastNoiseLite();

    public static event Action<World> OnStartWorldGeneratorLoad;

    public ConcurrentQueue<Chunk> ChunksMeshQueue = new ConcurrentQueue<Chunk>();

    public bool isGenerating = false;

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
    }
    private void Update()
    {
        if(ChunksMeshQueue.TryDequeue(out Chunk result))
        {
            result.GenerateChunk();
        }
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
        for (int x = -chunkRenderingDistance / 2 - 1; x < chunkRenderingDistance / 2 + 1; x++)
        {
            for (int z = -chunkRenderingDistance / 2 - 1; z < chunkRenderingDistance / 2 + 1; z++)
            {
                var chunkObj = Instantiate(chunkObject, new Vector3((x + vector.x) * chunkSize, 0, (z + vector.y) * chunkSize), Quaternion.identity, transform);

                if (!ChunkData.ContainsKey(new Vector2Int(x + vector.x, z + vector.y)))
                {
                    yield return new WaitForSecondsRealtime(0.001f);

                    chunkObj.InitChunk();
                    ChunkData.Add(new Vector2Int(x + vector.x, z + vector.y), chunkObj);
                    newChunkData.Add(new Vector2Int(x + vector.x, z + vector.y));

                }
                else
                {
                    newChunkData.Add(new Vector2Int(x + vector.x, z + vector.y));
                }
            }
        }
        foreach (var item in ChunkData.Keys.Except(newChunkData).ToList())
        {
            Destroy(ChunkData[item].gameObject);
            ChunkData.Remove(item);
        }
        newChunkData.Clear();

        for (int x = -chunkRenderingDistance / 2; x < chunkRenderingDistance / 2; x++)
        {
            for (int z = -chunkRenderingDistance / 2; z < chunkRenderingDistance / 2; z++)
            {
                yield return new WaitForSecondsRealtime(0.0001f);

                ChunkData.TryGetValue(new Vector2Int(x + vector.x, z + vector.y), out Chunk chunk);

                ChunkGenerationJob chunkJob = new ChunkGenerationJob
                {
                    blocks = chunk.blocks,
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
        isGenerating = false;
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
