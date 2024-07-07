using UnityEngine;

public class WorldSettings
{
    GlobalWorldGenSettings globalWorldGenSettings;
    WorldTempSettings worldTempSettings;
    WorldMoistureSettings worldMoistureSettings;

    public WorldSettings(GlobalWorldGenSettings globalWorldGenSettings, WorldTempSettings worldTempSettings, WorldMoistureSettings worldMoistureSettings)
    {
        this.globalWorldGenSettings = globalWorldGenSettings;
        this.worldTempSettings = worldTempSettings;
        this.worldMoistureSettings = worldMoistureSettings;
    }
}