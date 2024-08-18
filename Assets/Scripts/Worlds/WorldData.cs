using System;
using System.Collections.Generic;
[Serializable]
public class WorldData
{
    public string worldName = "New World";
    public string worldPath = string.Empty;
    public string worldFileName = string.Empty;
    public DateTime creationTime = new();

    public List<Chunk> chunksLoaded = new List<Chunk>();
}
