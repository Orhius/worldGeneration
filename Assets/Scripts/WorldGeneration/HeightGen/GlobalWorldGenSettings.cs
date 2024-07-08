using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class GlobalWorldGenSettings : MonoBehaviour
{
    [Header("global")]
    public string worldName = "New World";

    [Header("world gen")]
    public ulong width = 256;
    public ulong Width
    {
        get { return width; }
        set
        {
            if (value > ulong.MaxValue) width = ulong.MaxValue;
            else if (value < ulong.MinValue) width = 1;
            else width = value;
        }
    }
    public ulong length = 256;
    public ulong Length
    {
        get { return length; }
        set
        {
            if (value > ulong.MaxValue) length = ulong.MaxValue;
            else if (value < ulong.MinValue) length = 1;
            else length = value;
        }
    }
    public int height = 32;
    public ushort primaryChunkSize = 256;
    public byte chunkSize = 16;
    private ulong seed = 0;
    public ulong Seed
    {
        get { return seed; }
        set
        {
            if (value > ulong.MaxValue) seed = ulong.MaxValue;
            else if (value < ulong.MinValue) seed = 0;
            else seed = value;
        }
    }

    public WorldType worldType = WorldType.continents;

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
        worldNameInputField.text = worldName;
        Seed = (uint)UnityEngine.Random.Range(0, 99999);
        seedInputField.text = Seed.ToString();
        widthInputField.text = Width.ToString();
        lengthInputField.text = Length.ToString();

        //height
        heightDropdown.ClearOptions();
        List<string> namesHeight = new();
        foreach (var val in heighthList)
        {
            namesHeight.Add(val.ToString());
        }
        heightDropdown.AddOptions(namesHeight);
        heightDropdown.value = heighthList.IndexOf(height);

        //primaryChunkSize
        primaryChunkSizeDropdown.ClearOptions();
        List<string> namesPrimaryChunkSize = new();
        foreach (var val in primaryChunkSizeList)
        {
            namesPrimaryChunkSize.Add(val.ToString());
        }
        primaryChunkSizeDropdown.AddOptions(namesPrimaryChunkSize);
        primaryChunkSizeDropdown.value = primaryChunkSizeList.IndexOf(primaryChunkSize);

        //chunkSize
        ChunkSizeDropdown.ClearOptions();
        List<string> namesChunkSize = new();
        foreach (var val in chunkSizeList)
        {
            namesChunkSize.Add(val.ToString());
        }
        ChunkSizeDropdown.AddOptions(namesChunkSize);
        ChunkSizeDropdown.value = chunkSizeList.IndexOf(chunkSize);

        //worldType
        worldTypeDropdown.ClearOptions();
        List<string> namesWorldType = Enum.GetNames(typeof(WorldType)).ToList();
        worldTypeDropdown.AddOptions(namesWorldType);
        worldTypeDropdown.value = (int)worldType;
    }


    public void ChangeWorldName() => worldName = worldNameInputField.text;
    public void ChangeWorldWidth()
    {
        bool successfull = ulong.TryParse(widthInputField.text, out width);
        if (!successfull) Width = 1;
        widthInputField.text = Width.ToString();
    }
    public void ChangeWorldLength()
    {
        bool successfull = ulong.TryParse(lengthInputField.text, out length);
        if (!successfull) Length = 1;
        lengthInputField.text = Length.ToString();
    }
    public void ChangeWorldHeight() => height = heighthList.ElementAt(heightDropdown.value);
    public void ChangeSeed()
    {
        ulong.TryParse(seedInputField.text, out seed);
        seedInputField.text = Seed.ToString();
    }
    public void ChangePrimaryChunkSize() => primaryChunkSize = primaryChunkSizeList.ElementAt(primaryChunkSizeDropdown.value);
    public void ChangeChunkSize() => chunkSize = chunkSizeList.ElementAt(ChunkSizeDropdown.value);
    public void ChangeWorldType() => worldType = (WorldType)worldTypeDropdown.value;
}
public enum WorldType
{
    continents = 0,
    archipelagoes = 1,
    islands = 2
}