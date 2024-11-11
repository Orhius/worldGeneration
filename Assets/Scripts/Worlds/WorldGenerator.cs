using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Burst;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    private void OnEnable()
    {
        DontDestroyOnLoad(gameObject);
        OnStartWorldGeneratorLoad += LoadGameScene;
        GlobalEventManager.OnWorldSceneIsLoaded += StartWorldGeneration;
        GlobalEventManager.OnPlayerChunkPositionChanged += GenerateChunks;

        noiseLite.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
    }
    private void OnDisable()
    {
        OnStartWorldGeneratorLoad -= LoadGameScene;
        GlobalEventManager.OnWorldSceneIsLoaded -= StartWorldGeneration;
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

    private void StartWorldGeneration()
    {
        //GenerateChunks(Vector2Int.zero);
    }
    [BurstCompile]
    private void GenerateChunks(Vector2Int vector)
    {
        //List<Task> tasks = new List<Task>();
        //for (int x = -chunkRenderingDistance / 2 - 1; x < chunkRenderingDistance / 2 + 1; x++)
        //{
        //    for (int z = -chunkRenderingDistance / 2 - 1; z < chunkRenderingDistance / 2 + 1; z++)
        //    {
        //        var chunkObj = Instantiate(chunkObject, new Vector3((x + vector.x) * chunkSize, 0, (z + vector.y) * chunkSize), Quaternion.identity, transform);

        //        if (!ChunkData.ContainsKey(new Vector2Int(x + vector.x, z + vector.y)))
        //        {
        //            chunkObj.InitChunk();
        //            ChunkData.Add(new Vector2Int(x + vector.x, z + vector.y), chunkObj);
        //        }
        //    }
        //}
        //for (int x = -chunkRenderingDistance / 2; x < chunkRenderingDistance / 2; x++)
        //{
        //    for (int z = -chunkRenderingDistance / 2; z < chunkRenderingDistance / 2; z++)
        //    {

        //        ChunkData.TryGetValue(new Vector2Int(x + vector.x, z + vector.y), out Chunk chunk);

        //        //if (!chunk.isGenerated) chunk.GenerateChunkMesh();
        //        tasks.Add(chunk.GenerateChunkMesh());
        //        ChunksMeshQueue.Enqueue(chunk);
        //    }
        //}
        //while (tasks.Count > 0)
        //{
        //    tasks.RemoveAll(t => t.IsCompleted);
        //}
        if (isGenerating) { return; }
        isGenerating = true;
        StartCoroutine(GenerateChunksCoroutine(vector));
    }
    [BurstCompile]
    private IEnumerator GenerateChunksCoroutine(Vector2Int vector)
    {
        List<Task> tasks = new List<Task>();
        for (int x = -chunkRenderingDistance / 2 - 1; x < chunkRenderingDistance / 2 + 1; x++)
        {
            for (int z = -chunkRenderingDistance / 2 - 1; z < chunkRenderingDistance / 2 + 1; z++)
            {
                var chunkObj = Instantiate(chunkObject, new Vector3((x + vector.x) * chunkSize, 0, (z + vector.y) * chunkSize), Quaternion.identity, transform);

                if (!ChunkData.ContainsKey(new Vector2Int(x + vector.x, z + vector.y)))
                {
                    yield return new WaitForSecondsRealtime(0.0001f);

                    chunkObj.InitChunk();
                    ChunkData.TryAdd(new Vector2Int(x + vector.x, z + vector.y), chunkObj);

                }
            }
        }
        for (int x = -chunkRenderingDistance / 2; x < chunkRenderingDistance / 2; x++)
        {
            for (int z = -chunkRenderingDistance / 2; z < chunkRenderingDistance / 2; z++)
            {
                yield return new WaitForSecondsRealtime(0.0001f);

                ChunkData.TryGetValue(new Vector2Int(x + vector.x, z + vector.y), out Chunk chunk);

                tasks.Add(chunk.GenerateChunkMesh());
                ChunksMeshQueue.Enqueue(chunk);
            }
        }
        while (tasks.Count > 0)
        {
            tasks.RemoveAll(t => t.IsCompleted);
        }
        isGenerating = false;
    }

    [BurstCompile]
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
