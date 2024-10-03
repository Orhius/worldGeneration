using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldGenerator : MonoBehaviour
{
    public World currentWorld = new World();
    [SerializeField] private string GameSceneName = "GameScene";

    public static event Action<World> OnStartWorldGeneratorLoad;

    private void OnEnable()
    {
        DontDestroyOnLoad(gameObject);
        OnStartWorldGeneratorLoad += LoadGameScene;
    }
    private void OnDisable()
    {
        OnStartWorldGeneratorLoad -= LoadGameScene;
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

        SceneManager.LoadScene(GameSceneName);
        Debug.Log("waaaaaaaaaaaaagh");
    }
}
