public class WorldSettings
{
    public uint width = 256;
    public uint height = 256;
    public uint seed = 0;
    public WorldType worldType;
    public WorldTemperature worldTemperature;
    public WorldMoisture worldMoisture;


    public WorldSettings(uint width, uint height, uint seed, WorldType worldType, WorldTemperature worldTemperature, WorldMoisture worldMoisture)
    {
        this.width = width;
        this.height = height;
        this.seed = seed;
        this.worldType = worldType;
        this.worldTemperature = worldTemperature;
        this.worldMoisture = worldMoisture;
    }
}

public enum WorldType
{
    continents = 0,
    archipelagoes = 1,
    islands = 2
}
public enum WorldTemperature
{
    cold = 0,
    normal = 1,
    hot = 2
}
public enum WorldMoisture
{
   dry = 0,
   normal = 1,
   wet = 2
}
