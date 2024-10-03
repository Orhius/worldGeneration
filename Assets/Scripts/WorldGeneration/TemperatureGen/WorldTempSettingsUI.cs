using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class WorldTempSettingsUI : MonoBehaviour
{
    [Header("temp gen")]
    public WorldTempSettings worldTempSettings { private set; get; } = new();

    private readonly Dictionary<int, string> equatorWidthDictionary = new Dictionary<int, string>()
    {
        { 0, "0 %"},
        { 5, "5 %"},
        { 10, "10 %"},
        { 20, "20 %"},
        { 30, "30 %"},
        { 40, "40 %"},
        { 50, "50 %"},
        { 60, "60 %"},
        { 70, "70 %"},
        { 80, "80 %"},
        { 90, "90 %"},
        { 100, "100 %"},
    };

    [SerializeField] private TMP_Dropdown worldTempTypeDropdown;
    [SerializeField] private TMP_Dropdown worldTemperatureDropdown;
    [SerializeField] private TMP_Dropdown equatorWidthPercentDropdown;

    private void Awake()
    {
        worldTempTypeDropdown.onValueChanged.AddListener(x => ChangeWorldTempType());
        worldTemperatureDropdown.onValueChanged.AddListener(x => ChangeWorldTemperature());
        equatorWidthPercentDropdown.onValueChanged.AddListener(x => ChangeWorldEquator());
    }
    private void Start()
    {
        worldTempTypeDropdown.ClearOptions();
        List<string> namesWorldTempType = Enum.GetNames(typeof(WorldTemperatureType)).ToList();
        worldTempTypeDropdown.AddOptions(namesWorldTempType);
        worldTempTypeDropdown.value = (int)worldTempSettings.worldTempType;

        worldTemperatureDropdown.ClearOptions();
        List<string> namesWorldTemperatureDropdown = Enum.GetNames(typeof(WorldTemperature)).ToList();
        worldTemperatureDropdown.AddOptions(namesWorldTemperatureDropdown);
        worldTemperatureDropdown.value = (int)worldTempSettings.worldTemperature;

        equatorWidthPercentDropdown.ClearOptions();
        List<string> namesEquatorWidthPercent = equatorWidthDictionary.Values.ToList();
        equatorWidthPercentDropdown.AddOptions(namesEquatorWidthPercent);
        equatorWidthPercentDropdown.value = equatorWidthDictionary.Keys.ToList().IndexOf(worldTempSettings.EquatorWidthPercent);
    }
    public void ChangeWorldTempType() => worldTempSettings.worldTempType = (WorldTemperatureType)worldTempTypeDropdown.value;
    public void ChangeWorldTemperature() => worldTempSettings.worldTemperature = (WorldTemperature)worldTemperatureDropdown.value;
    public void ChangeWorldEquator() => worldTempSettings.EquatorWidthPercent = (byte)equatorWidthDictionary.Keys.ElementAt(equatorWidthPercentDropdown.value);
}
