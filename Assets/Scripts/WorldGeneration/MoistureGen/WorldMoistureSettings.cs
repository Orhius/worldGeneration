using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class WorldMoistureSettings : MonoBehaviour
{
    [Header("moisture gen")]
    public WorldMoisture worldMoisture = WorldMoisture.normal;

    [SerializeField] private TMP_Dropdown worldMoistureDropdown;


    private void Awake()
    {
        worldMoistureDropdown.onValueChanged.AddListener(x => ChangeWorldMoisture());
    }
    private void Start()
    {
        worldMoistureDropdown.ClearOptions();
        List<string> namesWorldMoisture = Enum.GetNames(typeof(WorldMoisture)).ToList();
        worldMoistureDropdown.AddOptions(namesWorldMoisture);
        worldMoistureDropdown.value = (int)worldMoisture;
    }
    public void ChangeWorldMoisture() => worldMoisture = (WorldMoisture)worldMoistureDropdown.value;
}
public enum WorldMoisture
{
    dry = 0,
    normal = 1,
    wet = 2
}