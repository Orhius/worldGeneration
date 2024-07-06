using UnityEngine;

public class WorldTempSettings : MonoBehaviour
{
    [Header("temp gen")]
    public WorldTemperatureType worldTempType = WorldTemperatureType.Random;
    public WorldTemperature worldTemperature = WorldTemperature.normal;
    public byte EquatorWidthPercent = 20;

    public void ChangeWorldTemperature(WorldTemperature worldTemperature)
    {
        this.worldTemperature = worldTemperature;
    }
    public void ChangeWorldTempType(WorldTemperatureType worldTempType)
    {
        this.worldTempType = worldTempType;
    }
}
public enum WorldTemperatureType
{
    Realistic = 0,
    Random = 1
}
public enum WorldTemperature
{
    cold = 0,
    normal = 1,
    hot = 2
}