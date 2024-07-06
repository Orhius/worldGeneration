using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMoistureSettings : MonoBehaviour
{
    public WorldMoisture worldMoisture = WorldMoisture.normal;

    public void ChangeWorldMoisture(WorldMoisture worldMoisture)
    {
        this.worldMoisture = worldMoisture;
    }
}
public enum WorldMoisture
{
    dry = 0,
    normal = 1,
    wet = 2
}