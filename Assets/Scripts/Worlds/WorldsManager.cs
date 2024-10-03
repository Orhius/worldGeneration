using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class WorldsManager : MonoBehaviour
{
    public List<World> worlds = new List<World>();

    [SerializeField] private GameObject worldPrefab;
    [SerializeField] private Transform worldGrid;
    private static string worldSavingPath = "";
    public static string WorldSavingPath
    {
        private set { worldSavingPath = value; }
        get { return worldSavingPath; }
    }

    private void OnEnable()
    {
        WorldConfigurator.OnWorldCreated += AddNewWorldPanel;
    }
    private void OnDisable()
    {
        WorldConfigurator.OnWorldCreated -= AddNewWorldPanel;
    }

    private void Awake()
    {
        WorldSavingPath = Application.persistentDataPath + "/worlds/";

        if (Directory.Exists(WorldSavingPath))
        {
            string[] files = Directory.GetFiles(WorldSavingPath);

            foreach (string file in files)
            {
                string fileName = Path.GetFileName(file.Replace(".json",""));
                World world = LoadWorldData(fileName);
                GameObject worldPref = Instantiate(worldPrefab, worldGrid);
                worldPref.GetComponent<WorldObject>().world.settings = world.settings;
                worldPref.GetComponent<WorldObject>().world.data = world.data;

                worldPref.GetComponent<WorldObject>().worldName.text = world.data.WorldName;
                worldPref.GetComponent<WorldObject>().worldInfo.text = $"creation time: {world.data.CreationTime.ToString("dd/MM/yyyy")}";

                Instantiate(worldPref);
                worlds.Add(world);
            }
        }
    }

    public void AddNewWorldPanel(World world)
    {
        GameObject worldPref = Instantiate(worldPrefab, worldGrid);
        World worldComp = worldPref.GetComponent<WorldObject>().world;
        worldComp.settings.globalWorldGenSettings = world.settings.globalWorldGenSettings;
        worldComp.settings.worldTempSettings = world.settings.worldTempSettings;
        worldComp.settings.worldMoistureSettings = world.settings.worldMoistureSettings;
        worldComp.data = world.data;
        worldComp.data.WorldName = world.settings.globalWorldGenSettings.worldName;
        worldComp.data.CreationTime = DateTime.Now;
        worldComp.data.WorldFileName = WorldSavingPath + world.data.WorldName;

        worldPref.GetComponent<WorldObject>().worldName.text = world.settings.globalWorldGenSettings.worldName;
        worldPref.GetComponent<WorldObject>().worldInfo.text = $"creation time: {worldComp.data.CreationTime.ToString("dd/MM/yyyy")}";

        SaveWorldData(worldComp);
        worlds.Add(worldComp);
    }

    public void SaveWorldData(World world)
    {
        //string json = JsonUtility.ToJson(world, true);
        string json = JsonConvert.SerializeObject(world, Formatting.Indented, new JsonSerializerSettings()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        });

        if (!Directory.Exists(WorldSavingPath + world.data.WorldName))
        {
            Directory.CreateDirectory(WorldSavingPath);
        }
        Debug.Log(WorldSavingPath + $"/{world.data.WorldName}.json");
        File.WriteAllText(WorldSavingPath + $"/{world.data.WorldName}.json", json);
        Debug.Log("World saved to : " + WorldSavingPath);
    }
    public static World LoadWorldData(string worldName)
    {
        if (File.Exists(WorldSavingPath + $"/{worldName}.json"))
        {
            Debug.Log("World loaded from : " + WorldSavingPath);
            return JsonConvert.DeserializeObject<World>(File.ReadAllText(WorldSavingPath + $"/{worldName}.json"));
            //return JsonUtility.FromJson<World>(json);
        }
        else
        {
            Debug.LogWarning("No save file found at: " + WorldSavingPath);
            return null;
        }
    }
}
