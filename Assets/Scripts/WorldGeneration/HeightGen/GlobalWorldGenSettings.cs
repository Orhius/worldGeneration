using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class GlobalWorldGenSettings : MonoBehaviour
{
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

    [SerializeField] private TMP_Dropdown heightDropdown;
    [SerializeField] private TMP_Dropdown primaryChunkSizeDropdown;
    [SerializeField] private TMP_Dropdown ChunkSizeDropdown;
    [SerializeField] private TMP_Dropdown worldTypeDropdown;

    [SerializeField] private TMP_InputField widthDropdown;
    [SerializeField] private TMP_InputField lengthDropdown;
    [SerializeField] private TMP_InputField seedInputField;

    private void Awake()
    {
        widthDropdown.onValueChanged.AddListener(x => ChangeWorldWidth());
        lengthDropdown.onValueChanged.AddListener(x => ChangeWorldLength());
        heightDropdown.onValueChanged.AddListener(x => ChangeWorldHeight());
        primaryChunkSizeDropdown.onValueChanged.AddListener(x => ChangePrimaryChunkSize());
        ChunkSizeDropdown.onValueChanged.AddListener(x => ChangeChunkSize());
        worldTypeDropdown.onValueChanged.AddListener(x => ChangeWorldType());

        seedInputField.onValueChanged.AddListener(x => ChangeSeed());
    }
    private void Start()
    {
        Seed = (uint)UnityEngine.Random.Range(0, 99999);
        seedInputField.text = Seed.ToString();
        widthDropdown.text = Width.ToString();
        lengthDropdown.text = Length.ToString();

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

    public void ChangeWorldWidth()
    {
        bool successfull = ulong.TryParse(widthDropdown.text, out width);
        if (!successfull) Width = 1;
        widthDropdown.text = Width.ToString();
    }
    public void ChangeWorldLength()
    {
        bool successfull = ulong.TryParse(lengthDropdown.text, out length);
        if (!successfull) Length = 1;
        lengthDropdown.text = Length.ToString();
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