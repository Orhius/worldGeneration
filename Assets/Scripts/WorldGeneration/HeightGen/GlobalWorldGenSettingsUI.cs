using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine;
using System.Linq;

public class GlobalWorldGenSettingsUI : MonoBehaviour
{

    [Header("world gen")]
    public GlobalWorldGenSettings globalWorldGenSettings { private set; get; } = new();

    private List<int> heighthList = new List<int>() { 16, 32, 64 };
    private readonly List<ushort> primaryChunkSizeList = new List<ushort>() { 128, 256 };
    private readonly List<byte> chunkSizeList = new List<byte>() { 16, 32 };

    [Header("dropdowns")]
    [SerializeField] private TMP_Dropdown heightDropdown;
    [SerializeField] private TMP_Dropdown primaryChunkSizeDropdown;
    [SerializeField] private TMP_Dropdown ChunkSizeDropdown;
    [SerializeField] private TMP_Dropdown worldTypeDropdown;

    [Header("input fields")]
    [SerializeField] private TMP_InputField worldNameInputField;
    [SerializeField] private TMP_InputField widthInputField;
    [SerializeField] private TMP_InputField lengthInputField;
    [SerializeField] private TMP_InputField seedInputField;

    private void Awake()
    {
        heightDropdown.onValueChanged.AddListener(x => ChangeWorldHeight());
        primaryChunkSizeDropdown.onValueChanged.AddListener(x => ChangePrimaryChunkSize());
        ChunkSizeDropdown.onValueChanged.AddListener(x => ChangeChunkSize());
        worldTypeDropdown.onValueChanged.AddListener(x => ChangeWorldType());

        worldNameInputField.onValueChanged.AddListener(x => ChangeWorldName());
        widthInputField.onValueChanged.AddListener(x => ChangeWorldWidth());
        lengthInputField.onValueChanged.AddListener(x => ChangeWorldLength());
        seedInputField.onValueChanged.AddListener(x => ChangeSeed());
    }
    private void Start()
    {
        worldNameInputField.text = globalWorldGenSettings.worldName;
        globalWorldGenSettings.Seed = (uint)UnityEngine.Random.Range(0, 99999);
        seedInputField.text = globalWorldGenSettings.Seed.ToString();
        widthInputField.text = globalWorldGenSettings.Width.ToString();
        lengthInputField.text = globalWorldGenSettings.Length.ToString();

        //height
        heightDropdown.ClearOptions();
        List<string> namesHeight = new();
        foreach (var val in heighthList)
        {
            namesHeight.Add(val.ToString());
        }
        heightDropdown.AddOptions(namesHeight);
        heightDropdown.value = heighthList.IndexOf(globalWorldGenSettings.height);

        //primaryChunkSize
        primaryChunkSizeDropdown.ClearOptions();
        List<string> namesPrimaryChunkSize = new();
        foreach (var val in primaryChunkSizeList)
        {
            namesPrimaryChunkSize.Add(val.ToString());
        }
        primaryChunkSizeDropdown.AddOptions(namesPrimaryChunkSize);
        primaryChunkSizeDropdown.value = primaryChunkSizeList.IndexOf(globalWorldGenSettings.primaryChunkSize);

        //chunkSize
        ChunkSizeDropdown.ClearOptions();
        List<string> namesChunkSize = new();
        foreach (var val in chunkSizeList)
        {
            namesChunkSize.Add(val.ToString());
        }
        ChunkSizeDropdown.AddOptions(namesChunkSize);
        ChunkSizeDropdown.value = chunkSizeList.IndexOf(globalWorldGenSettings.chunkSize);

        //worldType
        worldTypeDropdown.ClearOptions();
        List<string> namesWorldType = Enum.GetNames(typeof(WorldType)).ToList();
        worldTypeDropdown.AddOptions(namesWorldType);
        worldTypeDropdown.value = (int)globalWorldGenSettings.worldType;
    }


    public void ChangeWorldName() => globalWorldGenSettings.worldName = worldNameInputField.text;
    public void ChangeWorldWidth()
    {
        bool successfull = ulong.TryParse(widthInputField.text, out globalWorldGenSettings.width);
        if (!successfull) globalWorldGenSettings.Width = 1;
        widthInputField.text = globalWorldGenSettings.Width.ToString();
    }
    public void ChangeWorldLength()
    {
        bool successfull = ulong.TryParse(lengthInputField.text, out globalWorldGenSettings.length);
        if (!successfull) globalWorldGenSettings.Length = 1;
        lengthInputField.text = globalWorldGenSettings.Length.ToString();
    }
    public void ChangeWorldHeight() => globalWorldGenSettings.height = heighthList.ElementAt(heightDropdown.value);
    public void ChangeSeed()
    {
        ulong.TryParse(seedInputField.text, out globalWorldGenSettings.seed);
        seedInputField.text = globalWorldGenSettings.Seed.ToString();
    }
    public void ChangePrimaryChunkSize() => globalWorldGenSettings.primaryChunkSize = primaryChunkSizeList.ElementAt(primaryChunkSizeDropdown.value);
    public void ChangeChunkSize() => globalWorldGenSettings.chunkSize = chunkSizeList.ElementAt(ChunkSizeDropdown.value);
    public void ChangeWorldType() => globalWorldGenSettings.worldType = (WorldType)worldTypeDropdown.value;
}
