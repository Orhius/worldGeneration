using System;
using TreeEditor;

[Serializable]
public class World : ICloneable
{
    public WorldSettings settings;
    public WorldData data;

    public World(WorldSettings settings, WorldData data)
    {
        this.settings = settings;
        this.data = data;
    }
    public World()
    {
         
    }

    public object Clone()
    {
        var clone = new World();
        clone.settings = (WorldSettings)settings.Clone();
        clone.data = (WorldData)data.Clone();
        return clone;
    }
    public World DeepCopy()
    {
        World temp = (World)this.MemberwiseClone();
        temp.settings = settings.DeepCopy();
        temp.data = new WorldData(data.worldName, data.worldPath, data.worldFileName, data.creationTime, data.chunksLoaded);
        return temp;
    }
}
