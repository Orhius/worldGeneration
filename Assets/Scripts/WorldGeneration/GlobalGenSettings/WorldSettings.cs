using System;
[Serializable]
public class WorldSettings
{
    public GlobalWorldGenSettings globalWorldGenSettings;
    public WorldTempSettings worldTempSettings;
    public WorldMoistureSettings worldMoistureSettings;

    public WorldSettings(GlobalWorldGenSettings globalWorldGenSettings, WorldTempSettings worldTempSettings, WorldMoistureSettings worldMoistureSettings)
    {
        this.globalWorldGenSettings = globalWorldGenSettings;
        this.worldTempSettings = worldTempSettings;
        this.worldMoistureSettings = worldMoistureSettings;
    }
}