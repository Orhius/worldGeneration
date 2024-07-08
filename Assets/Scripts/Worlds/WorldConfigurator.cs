using System;
using UnityEngine;

public class WorldConfigurator : MonoBehaviour
{
    public WorldSettings worldSettings;

    public static event Action<World> OnWorldCreated;

    [SerializeField] private GlobalWorldGenSettings globalWorldGenSettings;
    [SerializeField] private WorldTempSettings worldTempSettings;
    [SerializeField] private WorldMoistureSettings worldMoistureSettings;
    public void ResetSettings()
    {
        
    }
    public void AceptWorld()
    {
        worldSettings = new WorldSettings(globalWorldGenSettings, worldTempSettings, worldMoistureSettings);
        OnWorldCreated.Invoke(new World(worldSettings));
    }
}