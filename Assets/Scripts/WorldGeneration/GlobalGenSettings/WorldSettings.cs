using System;

[Serializable]
public class WorldSettings : ICloneable
{
    public GlobalWorldGenSettings globalWorldGenSettings;
    public WorldTempSettings worldTempSettings;
    public WorldMoistureSettings worldMoistureSettings;

    public WorldSettings()
    {
    }

    public WorldSettings(GlobalWorldGenSettings globalWorldGenSettings, WorldTempSettings worldTempSettings, WorldMoistureSettings worldMoistureSettings)
    {
        this.globalWorldGenSettings = globalWorldGenSettings;
        this.worldTempSettings = worldTempSettings;
        this.worldMoistureSettings = worldMoistureSettings;
    }

    public object Clone()
    {
        var clone = new WorldSettings();
        clone.globalWorldGenSettings = (GlobalWorldGenSettings)globalWorldGenSettings.Clone();
        clone.worldTempSettings = (WorldTempSettings)worldTempSettings.Clone();
        clone.worldMoistureSettings = (WorldMoistureSettings)worldMoistureSettings.Clone();
        return clone;
    }
    public WorldSettings DeepCopy()
    {
        WorldSettings temp = (WorldSettings)this.MemberwiseClone();
        temp.globalWorldGenSettings = this.globalWorldGenSettings;
        temp.worldTempSettings = this.worldTempSettings;
        temp.worldMoistureSettings = this.worldMoistureSettings;
        return temp;
    }
}