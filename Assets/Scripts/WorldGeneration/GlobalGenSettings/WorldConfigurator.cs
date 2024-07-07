using UnityEngine;

public class WorldConfigurator : MonoBehaviour
{
    [SerializeField] private GlobalWorldGenSettings globalWorldGenSettings;
    [SerializeField] private WorldTempSettings worldTempSettings;
    [SerializeField] private WorldMoistureSettings worldMoistureSettings;
    public WorldSettings worldSettings;
    public void ResetSettings()
    {
        globalWorldGenSettings = new GlobalWorldGenSettings();
        worldTempSettings = new WorldTempSettings();
        worldMoistureSettings = new WorldMoistureSettings();
    }
    public void AcceptWorldSettings()
    {
        worldSettings = new WorldSettings(globalWorldGenSettings, worldTempSettings, worldMoistureSettings);
    }
}