using System;

[Serializable]
public class WorldMoistureSettings
{
    public WorldMoisture worldMoisture = WorldMoisture.normal;
}
public enum WorldMoisture
{
    dry = 0,
    normal = 1,
    wet = 2
}