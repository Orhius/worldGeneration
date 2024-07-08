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
            worldPref.GetComponent<World>().settings = world.settings;
            worldPref.GetComponent<World>().data = world.data;
            Instantiate(worldPref);
        }
    }

    public void AddNewWorldPanel(World world)
    {
        GameObject worldPref = Instantiate(worldPrefab, worldGrid);
        worldPref.GetComponent<World>().settings = world.settings;
        worldPref.GetComponent<World>().data = world.data;
        Instantiate(worldPref);
    }
}
