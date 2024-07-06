using UnityEngine;

public class WorldConfigurator : MonoBehaviour
{
    [SerializeField] private GlobalWorldGenSettings globalWorldGenSettings;
    [SerializeField] private WorldTempSettings worldTempSettings;
    [SerializeField] private WorldMoistureSettings worldMoistureSettings;
    private WorldSettings worldSettings;

    private void Awake()
    {
        globalWorldGenSettings = new GlobalWorldGenSettings();
    }
    public void AcceptWorldSettings()
    {
        worldSettings = new WorldSettings(globalWorldGenSettings);
    }
}