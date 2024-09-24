using System;
using System.Collections.Generic;
using UnityEngine;

public class WorldsManager : MonoBehaviour
{
    public List<World> worlds = new List<World>();

    [SerializeField] private GameObject worldPrefab;
    [SerializeField] private Transform worldGrid;

    private void OnEnable()
    {
        WorldConfigurator.OnWorldCreated += AddNewWorldPanel;
    }
    private void OnDisable()
    {
        WorldConfigurator.OnWorldCreated -= AddNewWorldPanel;
    }

    private void Start()
    {
        if (worlds.Count == 0) { return; }
        foreach (World world in worlds)
        {
            GameObject worldPref = Instantiate(worldPrefab, worldGrid);
            worldPref.GetComponent<WorldObject>().world.settings = world.settings;
            worldPref.GetComponent<WorldObject>().world.data = world.data;
            Instantiate(worldPref);
        }
    }

    public void AddNewWorldPanel(World world)
    {
        GameObject worldPref = Instantiate(worldPrefab, worldGrid);
        World worldComp = worldPref.GetComponent<WorldObject>().world;
        worldComp.settings.globalWorldGenSettings = world.settings.globalWorldGenSettings;
        worldComp.settings.worldTempSettings = world.settings.worldTempSettings;
        worldComp.settings.worldMoistureSettings = world.settings.worldMoistureSettings;
        worldComp.data = (WorldData)world.data.Clone();
        worldComp.data.worldName = world.settings.globalWorldGenSettings.worldName;
        worldComp.data.creationTime = DateTime.Now;

        worldPref.GetComponent<WorldObject>().worldName.text = world.settings.globalWorldGenSettings.worldName;
        worldPref.GetComponent<WorldObject>().worldInfo.text = $"creation time: {worldComp.data.creationTime.ToString("dd/MM/yyyy")}";
    }
}
