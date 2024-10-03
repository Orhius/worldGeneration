using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine;
using System.Linq;

public class WorldMoistureSettingsUI : MonoBehaviour
{
    [Header("moisture gen")]
    [SerializeField] private TMP_Dropdown worldMoistureDropdown;
    public WorldMoistureSettings worldMoistureSettings { private set; get; } = new();

    private void Awake()
    {
        worldMoistureDropdown.onValueChanged.AddListener(x => ChangeWorldMoisture());
    }
    private void Start()
    {
        worldMoistureDropdown.ClearOptions();
        List<string> namesWorldMoisture = Enum.GetNames(typeof(WorldMoisture)).ToList();
        worldMoistureDropdown.AddOptions(namesWorldMoisture);
        worldMoistureDropdown.value = (int)worldMoistureSettings.worldMoisture;
    }
    public void ChangeWorldMoisture() => worldMoistureSettings.worldMoisture = (WorldMoisture)worldMoistureDropdown.value;
}
