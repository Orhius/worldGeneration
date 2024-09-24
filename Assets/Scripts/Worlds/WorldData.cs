using System;
using System.Collections.Generic;
[Serializable]
public class WorldData
{
    public string worldName = "New world";
    public string worldPath = string.Empty;
    public string worldFileName = string.Empty;
    public DateTime creationTime = new();

    public List<Chunk> chunksLoaded = new List<Chunk>();

    public WorldData()
    {

    }
    public WorldData(string worldName, string worldPath, string worldFileName, DateTime creationTime, List<Chunk> chunksLoaded)
    {
        worldName = this.worldName;
        worldPath = this.worldPath;
        worldFileName = this.worldFileName;
        creationTime = this.creationTime;
        chunksLoaded = this.chunksLoaded;

    }
    public object Clone()
    {
        var clone = new WorldData
        {
            worldName = worldName,
            worldPath = worldPath,
            worldFileName = worldFileName,
            creationTime = creationTime,
            chunksLoaded = chunksLoaded

        };
        return clone;
    }
}
