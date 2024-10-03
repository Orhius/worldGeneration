using System;
using System.Collections.Generic;
[Serializable]
public class WorldData
{
    private string worldName = "New world";
    public string WorldName
    {
        get { return worldName; }
        set { worldName = value; }
    }
    private string worldPath = string.Empty;
    public string WorldPath
    {
        get { return worldPath; }
        set { worldPath = value; }
    }
    private string worldFileName = string.Empty;
    public string WorldFileName
    {
        get { return worldFileName; }
        set { worldFileName = value; }
    }
    private DateTime creationTime = new();
    public DateTime CreationTime
    {
        get { return creationTime; }
        set { creationTime = value; }
    }

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
}
