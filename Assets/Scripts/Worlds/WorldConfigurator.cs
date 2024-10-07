using System;
using UnityEngine;

public class WorldConfigurator : MonoBehaviour
{
    public WorldSettings worldSettings;

    public static event Action<World> OnWorldCreated;

    [SerializeField] private GlobalWorldGenSettingsUI globalWorldGenSettingsUI;
    [SerializeField] private WorldTemperatureSettingsUI worldTempSettingsUI;
    [SerializeField] private WorldMoistureSettingsUI worldMoistureSettingsUI;
    public void ResetSettings()
    {
        
    }
    public void AceptWorld()
    {
        worldSettings = new WorldSettings(globalWorldGenSettingsUI.globalWorldGenSettings, worldTempSettingsUI.worldTempSettings, worldMoistureSettingsUI.worldMoistureSettings);
        WorldData worldData = new WorldData();
        //worldData.name = worldSettings.globalWorldGenSettings.worldName;
        OnWorldCreated.Invoke(new World(worldSettings, worldData));
    }
}