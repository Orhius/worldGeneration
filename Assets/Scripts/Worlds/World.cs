using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public WorldSettings settings;
    public WorldData data;
    public World(WorldSettings settings)
    {
        this.settings = settings;
    }
}
