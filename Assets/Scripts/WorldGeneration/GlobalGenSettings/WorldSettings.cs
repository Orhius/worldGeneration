using System;

[Serializable]
public class WorldSettings
{
    public GlobalWorldGenSettings globalWorldGenSettings;
    public WorldTemperatureSettings worldTempSettings;
    public WorldMoistureSettings worldMoistureSettings;

    public WorldSettings()
    {
    }

    public WorldSettings(GlobalWorldGenSettings globalWorldGenSettings, WorldTemperatureSettings worldTempSettings, WorldMoistureSettings worldMoistureSettings)
    {
        this.globalWorldGenSettings = globalWorldGenSettings;
        this.worldTempSettings = worldTempSettings;
        this.worldMoistureSettings = worldMoistureSettings;
    }
}