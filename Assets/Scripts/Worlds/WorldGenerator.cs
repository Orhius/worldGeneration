using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class WorldGenerator : MonoBehaviour
{
    public static World currentWorld = new World();
    [SerializeField] private string GameSceneName = "GameScene";
    [SerializeField] private Dictionary<Vector2Int, Chunk> ChunkData = new();
    [SerializeField] private Chunk chunkObject;

    [SerializeField] private List<FastNoiseSettings> noiseSettings = new List<FastNoiseSettings>();
    private static List<FastNoiseLite> noiseOctaves = new List<FastNoiseLite>();
    private static List<float> noiseAmplitude = new List<float>();

    public static int chunkSize = 16;
    public static int chunkHeight = 128;
    public static int baseChunkHeight = chunkHeight/2;

    public static SimplexNoise.Layer noiseHeigthLayer = new();
    public static FastNoiseLite noiseLite = new FastNoiseLite();


    public static event Action<World> OnStartWorldGeneratorLoad;

    private void OnEnable()
    {
        DontDestroyOnLoad(gameObject);
        OnStartWorldGeneratorLoad += LoadGameScene;
        GlobalEventManager.OnWorldSceneIsLoaded += StartWorldGeneration;

        noiseLite.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
    }
    private void OnDisable()
    {
        OnStartWorldGeneratorLoad -= LoadGameScene;
        GlobalEventManager.OnWorldSceneIsLoaded -= StartWorldGeneration;

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

        SceneManager.LoadScene(GameSceneName);

        GlobalEventManager.WorldSceneIsLoaded();
    }

    private void StartWorldGeneration()
    {
        for (int x = 0; x < 10; x++)
        {
            for (int z = 0; z < 10; z++)
            {
                var chunkObj = Instantiate(chunkObject, new Vector3(x * chunkSize, 0, z * chunkSize), Quaternion.identity, transform);
                ChunkData.Add(new Vector2Int(x, z), chunkObj);
            }
        }
    }

    public static float GenerateHeight(float x, float y)
    {
        float noise = (float)baseChunkHeight;

        for (int i = 0; i < noiseOctaves.Count; i++)
        {
            float noiseOctave = noiseOctaves[i].GetNoise(x, y);
            noise += noiseOctave * noiseAmplitude[i];
        }
        //Vector3 offsetVector = new Vector3(x, 0, y);
        //noiseHeigthLayer.offset = offsetVector;
        //Vector3 v = new Vector3(0, 0, 0);
        //float noise = SimplexNoise.GenerateValue(noiseHeigthLayer, v);
        return noise;
    }
}
