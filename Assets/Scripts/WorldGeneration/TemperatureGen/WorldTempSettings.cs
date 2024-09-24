using System;
using UnityEngine;

[Serializable]
public class WorldTempSettings : ICloneable
{
    public WorldTemperatureType worldTempType = WorldTemperatureType.realistic;
    public WorldTemperature worldTemperature = WorldTemperature.normal;

    [Range(0, 100)] private byte equatorWidthPercent = 20;
    public byte EquatorWidthPercent
    {
        get { return equatorWidthPercent; }
        set
        {
            if (value >= 0) equatorWidthPercent = value;
            else equatorWidthPercent = 0;
            if (value >= 100) equatorWidthPercent = value;
            else equatorWidthPercent = 100;
        }
    }

    public object Clone()
    {
        var clone = new WorldTempSettings
        {
            worldTempType = worldTempType,
            worldTemperature = worldTemperature,
            EquatorWidthPercent = EquatorWidthPercent
        };
        return clone;
    }
}
public enum WorldTemperatureType
{
    realistic = 0,
    random = 1
}
public enum WorldTemperature
{
    cold = 0,
    normal = 1,
    hot = 2
}