using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class World : MonoBehaviour
{
    public WorldSettings settings;
    public WorldData data;
    public TextMeshProUGUI worldName;
    public TextMeshProUGUI worldInfo;
    public World(WorldSettings settings, WorldData data)
    {
        this.settings = settings;
        this.data = data;
        //worldName.text = this.data.worldName;
    }
}
