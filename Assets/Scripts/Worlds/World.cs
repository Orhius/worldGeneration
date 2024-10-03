using Newtonsoft.Json;
using System;

[Serializable]
public class World
{
    public WorldSettings settings;
    public WorldData data;

    public World(WorldSettings settings, WorldData data)
    {
        this.settings = settings;
        this.data = data;
    }
    public World() { }
}
