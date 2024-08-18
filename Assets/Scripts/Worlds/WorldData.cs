using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class WorldData : MonoBehaviour
{
    public string worldName = "New World";
    public string worldPath = string.Empty;
    public string worldFileName = string.Empty;

    public List<Chunk> chunksLoaded = new List<Chunk>();
}
