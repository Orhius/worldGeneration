using System;

[Serializable]
public class WorldMoistureSettings : ICloneable
{
    public WorldMoisture worldMoisture = WorldMoisture.normal;
    public object Clone()
    {
        var clone = new WorldMoistureSettings();
        clone.worldMoisture = worldMoisture;
        return clone;
    }
}
public enum WorldMoisture
{
    dry = 0,
    normal = 1,
    wet = 2
}