using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class WorldTempSettings : MonoBehaviour
{
    [Header("temp gen")]
    public WorldTemperatureType worldTempType = WorldTemperatureType.realistic;
    public WorldTemperature worldTemperature = WorldTemperature.normal;

    [Range(0, 100)] private byte equatorWidthPercent = 20;
    public byte EquatorWidthPercent
    {
        get { return equatorWidthPercent; }
        set
        {
            if (value >= 0) equatorWidthPercent = value;
            else equatorWidthPercent = 0;
            if (value >= 100) equatorWidthPercent = value;
            else equatorWidthPercent = 100;
        }
    }

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
        worldTempTypeDropdown.value = (int)worldTempType;

        worldTemperatureDropdown.ClearOptions();
        List<string> namesWorldTemperatureDropdown = Enum.GetNames(typeof(WorldTemperature)).ToList();
        worldTemperatureDropdown.AddOptions(namesWorldTemperatureDropdown);
        worldTemperatureDropdown.value = (int)worldTemperature;

        equatorWidthPercentDropdown.ClearOptions();
        List<string> namesEquatorWidthPercent = equatorWidthDictionary.Values.ToList();
        equatorWidthPercentDropdown.AddOptions(namesEquatorWidthPercent);
        equatorWidthPercentDropdown.value = equatorWidthDictionary.Keys.ToList().IndexOf(EquatorWidthPercent);
    }
    public void ChangeWorldTempType() => worldTempType = (WorldTemperatureType)worldTempTypeDropdown.value;
    public void ChangeWorldTemperature() => worldTemperature = (WorldTemperature)worldTemperatureDropdown.value;
    public void ChangeWorldEquator() => EquatorWidthPercent = (byte)equatorWidthDictionary.Keys.ElementAt(equatorWidthPercentDropdown.value);
}
public enum WorldTemperatureType
{
    realistic = 0,
    random = 1
}
public enum WorldTemperature
{
    cold = 0,
    normal = 1,
    hot = 2
}