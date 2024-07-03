using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
public class TextureGenerator : MonoBehaviour
{
    [Header("Height")]
    [SerializeField] private int width = 256;
    [SerializeField] private int height = 256;
    [SerializeField] private string seed = string.Empty;
    [SerializeField] private float deepEdge = Generator.deepEdge;
    [SerializeField] private float shallowEdge = Generator.shallowEdge;
    [SerializeField] private float sandEdge = Generator.sandEdge;
    [SerializeField] private float grassEdge = Generator.grassEdge;
    [SerializeField] private float forestEdge = Generator.forestEdge;
    [SerializeField] private float rockEdge = Generator.rockEdge;
    [SerializeField] private float snowEdge = Generator.snowEdge;
    private Tile[,] tiles;
    private List<Tile> tilesRock = new List<Tile>();
    private List<Tile> tilesShallow = new List<Tile>();
    public SimplexNoise.Layer noiseLayer;

    private const float surfaceAreaPerRoughness = 128000;

    [Header("Heat")]
    private float equatorWidth = 0;
    public float EquatorWidth
    {
        get { return equatorWidth; }
        set
        {
            if(equatorWidth <= width/2) equatorWidth = value;
            else equatorWidth = width / 2;
        }
    }
    public float newEquatorWidth = 0;

    [SerializeField] private int heatOffsetMultiplier = 10;
    [SerializeField] private float heatScale = 10f;

    [Header("Moisture")]
    [SerializeField] private float moistureIncrease = 1f;
    [SerializeField] private float moistureDecrease = 1f;
    [SerializeField] private float moistureScale = 10f;

    [Header("Rivers")]
    [SerializeField]private int riversCount = 10;
    List<River> rivers = new List<River>();

    private const int surfaceAreaPerRiver = 49152;

    [Header("Lightning")]
    [SerializeField] private GameObject lightSource;

    private void Start()
    {
        if (string.IsNullOrEmpty(seed)) seed = Random.Range(0, 99999).ToString();
        tiles = ResizeMap();
        equatorWidth = height / 10;
        GenerateBiomeTexture();
    }

    public void GenerateHeightTexture() => GetComponent<RawImage>().texture = GetHeightTexture();
    public void GenerateHeatTexture() => GetComponent<RawImage>().texture = GetHeatTexture(width, height, equatorWidth);
    public void GenerateMoistureTexture() => GetComponent<RawImage>().texture = GetMoistureTexture(width, height);
    public void GenerateBiomeTexture() => GetComponent<RawImage>().texture = GetBiomeTexture(width, height);

    #region Reset Map Settings
    public Tile[,] ResizeMap()
    {
        Tile[,] newTiles = new Tile[width, height];
        return newTiles;
    }
    public string ResetSeed()
    {
        seed = Random.Range(0, 99999).ToString();
        return seed;
    }
    #endregion

    #region TextureGenerator
    public Texture2D GetHeightTexture()
    {
        tiles = ResizeMap();
        ResetSeed();
        //noiseLayer.baseRoughness = (width / 256f) / 1000f + 0.002f;
        tilesRock = new List<Tile>();
        tilesShallow = new List<Tile>();
        Vector3 offsetVector = new Vector3(Random.Range(0, 99999), Random.Range(0, 99999), Random.Range(0, 99999));
        noiseLayer.offset = offsetVector;
        var texture = new Texture2D(width, height);
        var pixels = new Color[width * height];
        int i = 0;
        for (int x  = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                Tile tile = new Tile();
                Vector3 v = new Vector3(x, y, 0);
                float noise = SimplexNoise.GenerateValue(noiseLayer, v);
                i++;
                tile.HeightValue = noise;
                tile.x = x;
                tile.y = y;
                if (noise < deepEdge)
                {
                    pixels[x + y * width] = Generator.deepColor;
                    tile.heightType = HeightType.DeepWater;
                    tile.surfaceType = SurfaceType.Water;
                }
                else if (noise < shallowEdge)
                {
                    pixels[x + y * width] = Generator.shallowColor;
                    tile.heightType = HeightType.ShallowWater;
                    tile.surfaceType = SurfaceType.Water;
                    tilesShallow.Add(tile);
                }
                else if (noise < sandEdge)
                {
                    pixels[x + y * width] = Generator.sandColor;
                    tile.heightType = HeightType.Sand;
                    tile.surfaceType = SurfaceType.Land;
                }
                else if (noise < grassEdge)
                {
                    pixels[x + y * width] = Generator.grassColor;
                    tile.heightType = HeightType.Grass;
                    tile.surfaceType = SurfaceType.Land;
                }
                else if (noise < forestEdge)
                {
                    pixels[x + y * width] = Generator.forestColor;
                    tile.heightType = HeightType.Forest;
                    tile.surfaceType = SurfaceType.Land;
                }
                else if (noise <= rockEdge)
                {
                    pixels[x + y * width] = Generator.rockColor;
                    tile.heightType = HeightType.Rock;
                    tile.surfaceType = SurfaceType.Land;
                    tilesRock.Add(tile);
                }
                else
                {
                    pixels[x + y * width] = Generator.snowColor;
                    tile.heightType = HeightType.Snow;
                    tile.surfaceType = SurfaceType.Land;
                }
                tiles[x, y] = tile;
            }
        }
        pixels = GenerateRivers(pixels);

        texture.SetPixels(pixels);
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.filterMode = FilterMode.Point;
        texture.Apply();
        return texture;
    }
    public Texture2D GetHeatTexture(int width, int height, float equatorWidth)
    {
        float hemisphere = height/2;
        var texture = new Texture2D(width, height);
        var pixels = new Color[width * height];
        for (int x = 0; x < width; x++)
        {
            int y = (int)(height / 2);
            int dy = (int)(height / 2)-1;
            for (; y < height;)
            {
                Tile tileN = new Tile();
                Tile tileS = new Tile();
                
                float xc1 = (float)x / width * heatScale + int.Parse(seed);
                float yc1 = (float)y / height * heatScale - int.Parse(seed);
                float noise = Mathf.PerlinNoise(xc1,yc1);
                float noise2 = Mathf.PerlinNoise(xc1, yc1);
                
                float offset = noise * heatOffsetMultiplier * (noise2 * heatOffsetMultiplier/2);
                if (y < height /2 + equatorWidth - offset)
                {
                    tileN.HeatValue = Generator.warmestEdge;
                    tileS.HeatValue = Generator.warmestEdge;
                }
                else if(y < height / 2 + equatorWidth + hemisphere / 5.5f - offset)
                {
                    tileN.HeatValue = Generator.warmerEdge;
                    tileS.HeatValue = Generator.warmerEdge;
                }
                else if (y < height / 2 + equatorWidth + 2 * (hemisphere / 5.5f) - offset)
                {
                    tileN.HeatValue = Generator.warmEdge;
                    tileS.HeatValue = Generator.warmEdge;
                }
                else if (y < height / 2 + equatorWidth + 3 * (hemisphere / 5.5f) - offset)
                {
                    tileN.HeatValue = Generator.coldEdge;
                    tileS.HeatValue = Generator.coldEdge;
                }
                else if (y < height / 2 + equatorWidth + 4 * (hemisphere / 5.5f) - offset)
                {
                    tileN.HeatValue = Generator.colderEdge;
                    tileS.HeatValue = Generator.colderEdge;
                }
                else if (y < height / 2 + equatorWidth + hemisphere - offset)
                {
                    tileN.HeatValue = Generator.coldestEdge;
                    tileS.HeatValue = Generator.coldestEdge;
                }
                
                switch (tiles[x, y].heightType)
                {
                    case HeightType.DeepWater:
                        tileN.HeatValue -= 0.6f;
                        break;

                    case HeightType.ShallowWater:
                        tileN.HeatValue -= 0.15f * tiles[x, y].HeightValue;
                        break;
                    case HeightType.Sand:
                        tileN.HeatValue += 0.05f * tiles[x, y].HeightValue;
                        break;
                    case HeightType.Grass:
                        tileN.HeatValue += 0.05f * tiles[x, y].HeightValue;
                        break;
                    case HeightType.Forest:
                        tileN.HeatValue -= 0 * tiles[x, y].HeightValue;
                        break;
                    case HeightType.Rock:
                        tileN.HeatValue -= 0.05f * tiles[x, y].HeightValue;
                        break;
                    case HeightType.Snow:
                        tileN.HeatValue -= 0.1f * tiles[x, y].HeightValue;
                        break;
                    default:
                        tileN.HeatValue = 0 * tiles[x, y].HeightValue;
                        break;
                }
                switch (tiles[x, dy].heightType)
                {
                    case HeightType.DeepWater:
                        tileS.HeatValue -= 0.6f;
                        break;
                    case HeightType.ShallowWater:
                        tileS.HeatValue -= 0.15f * tiles[x, dy].HeightValue;
                        break;
                    case HeightType.Sand:
                        tileS.HeatValue += 0.05f * tiles[x, dy].HeightValue;
                        break;
                    case HeightType.Grass:
                        tileS.HeatValue += 0.05f * tiles[x, dy].HeightValue;
                        break;
                    case HeightType.Forest:
                        tileS.HeatValue -= 0 * tiles[x, dy].HeightValue;
                        break;
                    case HeightType.Rock:
                        tileS.HeatValue -= 0.05f  * tiles[x, dy].HeightValue;
                        break;
                    case HeightType.Snow:
                        tileS.HeatValue -= 0.1f * tiles[x, dy].HeightValue;
                        break;
                    default:
                        tileS.HeatValue = 0 * tiles[x, dy].HeightValue;
                        break;
                }
                
                switch (tileN.HeatValue)
                {
                    case var value when value <= Generator.coldestEdge:
                        pixels[x + y * width] = Generator.coldestColor;
                        tileN.heatType = HeatType.Coldest;
                        break;
                    case var value when value <= Generator.colderEdge:
                        pixels[x + y * width] = Generator.colderColor;
                        tileN.heatType = HeatType.Colder;
                        break;
                    case var value when value <= Generator.coldEdge:
                        pixels[x + y * width] = Generator.coldColor;
                        tileN.heatType = HeatType.Cold;
                        break;
                    case var value when value <= Generator.warmEdge:
                        pixels[x + y * width] = Generator.warmColor;
                        tileN.heatType = HeatType.Warm;
                        break;
                    case var value when value <= Generator.warmerEdge:
                        pixels[x + y * width] = Generator.warmerColor;
                        tileN.heatType = HeatType.Warmer;
                        break;
                    case var value when value <= Generator.warmestEdge:
                        pixels[x + y * width] = Generator.warmestColor;
                        tileN.heatType = HeatType.Warmest;
                        break;
                    default:
                        pixels[x + y * width] = Generator.warmestColor;
                        tileN.heatType = HeatType.Warmest;
                        break;
                }
                switch (tileS.HeatValue)
                {
                    case var value when value <= Generator.coldestEdge:
                        pixels[x + dy * width] = Generator.coldestColor;
                        tileS.heatType = HeatType.Coldest;
                        break;
                    case var value when value <= Generator.colderEdge:
                        pixels[x + dy * width] = Generator.colderColor;
                        tileS.heatType = HeatType.Colder;
                        break;
                    case var value when value <= Generator.coldEdge:
                        pixels[x + dy * width] = Generator.coldColor;
                        tileS.heatType = HeatType.Cold;
                        break;
                    case var value when value <= Generator.warmEdge:
                        pixels[x + dy * width] = Generator.warmColor;
                        tileS.heatType = HeatType.Warm;
                        break;
                    case var value when value <= Generator.warmerEdge:
                        pixels[x + dy * width] = Generator.warmerColor;
                        tileS.heatType = HeatType.Warmer;
                        break;
                    case var value when value <= Generator.warmestEdge:
                        pixels[x + dy * width] = Generator.warmestColor;
                        tileS.heatType = HeatType.Warmest;
                        break;
                    default:
                        pixels[x + dy * width] = Generator.warmestColor;
                        tileS.heatType = HeatType.Warmest;
                        break;
                }
                tiles[x,y].heatType = tileN.heatType;
                tiles[x, y].HeatValue = tileN.HeatValue;

                tiles[x, dy].heatType = tileS.heatType;
                tiles[x, dy].HeatValue = tileS.HeatValue;
                y++;
                dy--;
            }
            //if(equatorWidth < height/2) pixels[x + height - 1 * width] = Generator.coldestColor;
            //else pixels[x + height - 1 * width] = Generator.warmestColor;
        }

        texture.SetPixels(pixels);
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.filterMode = FilterMode.Point;
        texture.Apply();
        return texture;
    }
    public Texture2D GetMoistureTexture(int width, int height)
    {
        var texture = new Texture2D(width, height);
        var pixels = new Color[width * height];
        Tile tile = new Tile();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float xc1 = (float)x / width * moistureScale + int.Parse(seed);
                float yc1 = (float)y / height * moistureScale - int.Parse(seed);

                float xc2 = (float)x / width * moistureScale - int.Parse(seed);
                float yc2 = (float)y / height * moistureScale + int.Parse(seed);
                float noise = Mathf.PerlinNoise(xc1, yc1)/ moistureDecrease - Mathf.PerlinNoise(xc2, yc2) / moistureIncrease;
                if (tiles[x, y].heightType == HeightType.DeepWater)
                {
                    tile.MoistureValue = 1;
                }
                else if (tiles[x, y].heightType == HeightType.ShallowWater)
                {
                    tile.MoistureValue = 0.7f + noise;
                }
                else if (tiles[x, y].heightType == HeightType.Sand)
                {
                    tile.MoistureValue = 0.4f + noise;
                }
                else if (tiles[x, y].heightType == HeightType.Grass)
                {
                    tile.MoistureValue = 0.3f + noise;
                }
                else if (tiles[x, y].heightType == HeightType.Forest)
                {
                    tile.MoistureValue = 0.4f + noise;
                }
                else if (tiles[x, y].heightType == HeightType.Rock)
                {
                    tile.MoistureValue = 0.2f + noise;
                }
                else if (tiles[x, y].heightType == HeightType.Snow)
                {
                    tile.MoistureValue = 0.1f + noise;
                }


                if (tile.MoistureValue < Generator.DryestValue)
                {
                    tile.moistureType = MoistureType.Dryest;
                    pixels[x + y * width] = Generator.DryestColor;
                }
                else if(tile.MoistureValue < Generator.DryerValue)
                {
                    tile.moistureType = MoistureType.Dryer;
                    pixels[x + y * width] = Generator.DryerColor;
                }
                else if (tile.MoistureValue < Generator.DryValue)
                {
                    tile.moistureType = MoistureType.Dry;
                    pixels[x + y * width] = Generator.DryColor;
                }
                else if (tile.MoistureValue < Generator.WetValue)
                {
                    tile.moistureType = MoistureType.Wet;
                    pixels[x + y * width] = Generator.WetColor;
                }
                else if (tile.MoistureValue < Generator.WetterValue)
                {
                    tile.moistureType = MoistureType.Wetter;
                    pixels[x + y * width] = Generator.WetterColor;
                }
                else if (tile.MoistureValue < Generator.WettestValue)
                {
                    tile.moistureType = MoistureType.Wettest;
                    pixels[x + y * width] = Generator.WettestColor;
                }
                else
                {
                    tile.moistureType = MoistureType.Wettest;
                    pixels[x + y * width] = Generator.WettestColor;
                }
                tiles[x, y].MoistureValue = tile.MoistureValue;
                tiles[x, y].moistureType = tile.moistureType;
            }
        }
        texture.SetPixels(pixels);
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.filterMode = FilterMode.Point;
        texture.Apply();
        return texture;
    }
    public Texture2D GetBiomeTexture(int width, int height)
    {
        GenerateHeightTexture();
        GenerateHeatTexture();
        GenerateMoistureTexture();
        var texture = new Texture2D(width, height);
        var pixels = new Color[width * height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Color modifiedColor = new();
                Color LowHeightColor = new Color(0 / 255f, 0 / 255f, 20 / 255f, 1);

                if (tiles[x, y].surfaceType == SurfaceType.Land && tiles[x, y].heightType != HeightType.Sand && tiles[x, y].heightType != HeightType.Rock && tiles[x, y].heightType != HeightType.Snow)
                {
                    tiles[x, y].biomeType = Generator.GetBiomeType(tiles[x, y]);
                    switch (tiles[x, y].biomeType)
                    {
                        case BiomeType.Ice:
                            pixels[x + y * width] = Generator.IceColor;
                            break;
                        case BiomeType.Tundra:
                            pixels[x + y * width] = Generator.TundraColor;
                            break;
                        case BiomeType.BorealForest:
                            pixels[x + y * width] = Generator.BorealForestColor;
                            break;
                        case BiomeType.Woodland:
                            pixels[x + y * width] = Generator.WoodlandColor;
                            break;
                        case BiomeType.Grassland:
                            pixels[x + y * width] = Generator.GrasslandColor;
                            break;
                        case BiomeType.SeasonalForest:
                            pixels[x + y * width] = Generator.SeasonalForestColor;
                            break;
                        case BiomeType.TemperateRainforest:
                            pixels[x + y * width] = Generator.TemperateRainforestColor;
                            break;
                        case BiomeType.TropicalForest:
                            pixels[x + y * width] = Generator.TropicalRainforestColor;
                            break;
                        case BiomeType.Savanna:
                            pixels[x + y * width] = Generator.SavannaColor;
                            break;
                        case BiomeType.Desert:
                            pixels[x + y * width] = Generator.DesertColor;
                            break;
                        default:
                            break;
                    }
                    modifiedColor = Color.Lerp(pixels[x + y * width], LowHeightColor, 1 - tiles[x, y].HeightValue);
                }
                else
                {
                    if (tiles[x, y].heightType == HeightType.DeepWater)
                    {
                        pixels[x + y * width] = Generator.deepColor;
                        modifiedColor = Color.Lerp(pixels[x + y * width], LowHeightColor, 0.9f - tiles[x, y].HeightValue);
                    }
                    else if (tiles[x, y].heightType == HeightType.ShallowWater)
                    {
                        tiles[x, y].biomeType = Generator.GetBiomeType(tiles[x, y]);
                        if (tiles[x, y].biomeType == BiomeType.Desert)
                        {
                            pixels[x + y * width] = Generator.DesertColor;
                            modifiedColor = Color.Lerp(pixels[x + y * width], LowHeightColor, 1 - tiles[x, y].HeightValue);
                        }
                        else
                        {
                            pixels[x + y * width] = Generator.shallowColor;
                            modifiedColor = Color.Lerp(pixels[x + y * width], LowHeightColor, 1.1f - tiles[x, y].HeightValue);
                        }
                    }
                    else if (tiles[x, y].heightType == HeightType.Sand)
                    {
                        pixels[x + y * width] = Generator.sandColor;
                        modifiedColor = Color.Lerp(pixels[x + y * width], LowHeightColor, 1 - tiles[x, y].HeightValue);
                    }
                    else if (tiles[x, y].heightType == HeightType.Rock)
                    {
                        pixels[x + y * width] = Generator.rockColor;
                        modifiedColor = Color.Lerp(pixels[x + y * width], LowHeightColor, 1 - tiles[x, y].HeightValue);
                    }
                    else if (tiles[x, y].heightType == HeightType.Snow)
                    {
                        pixels[x + y * width] = Generator.snowColor;
                        modifiedColor = Color.Lerp(pixels[x + y * width], LowHeightColor, 1 - tiles[x, y].HeightValue);
                    }
                }

                pixels[x + y * width] = modifiedColor;
            }
        }
        pixels = GenerateShadowedTexture(pixels, tiles);

        texture.SetPixels(pixels);
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.filterMode = FilterMode.Point;
        texture.Apply();
        return texture;
    }
    #endregion

    #region Rivers Generation
    public Color[] GenerateRivers(Color[] pixels)
    {
        riversCount = width * height / surfaceAreaPerRiver;
        if (tilesRock == null || tilesShallow == null) return pixels;
        if (tilesRock.Count <= 1 || tilesShallow.Count <= 1) return pixels;
        for (int i = 0; i < riversCount; i++)
        {
            int rock = Random.Range(0, tilesRock.Count - 1);
            List<Tile> shallowTilesNearby = tilesShallow.Where(x =>  Vector2.Distance(new Vector2(tilesRock[rock].x, tilesRock[rock].y), new Vector2(x.x, x.y)) < 256).ToList();
            if(shallowTilesNearby != null)
            {
                int shallow = Random.Range(0, shallowTilesNearby.Count - 1);

                Vector2 startPoint = new Vector2(tilesRock[rock].x, tilesRock[rock].y);
                Vector2 destinationPoint = new Vector2(shallowTilesNearby[shallow].x, shallowTilesNearby[shallow].y);
                RiversGenerator riverGenerator = new RiversGenerator();
                (tiles, pixels) = riverGenerator.GeneratePath(startPoint, destinationPoint, tiles, pixels, width);
            }
        }
        return pixels;
    }
    #endregion

    #region Normals
    Color[] GenerateShadowedTexture(Color[] pixels, Tile[,] tiles)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Color baseColor = pixels[x + y * width];
                if (tiles[x, y].surfaceType != SurfaceType.Water)
                {
                    float shadowFactor = ComputeShadowFactor(x, y, 1);
                    Color shadowedColor = baseColor * shadowFactor * 30 * tiles[x, y].HeightValue;
                    pixels[x + y * width] += shadowedColor;
                }
                else
                {
                    float shadowFactor = ComputeShadowFactor(x, y, -1);
                    Color shadowedColor = baseColor * shadowFactor * 20;
                    pixels[x + y * width] += shadowedColor;
                }
                //Debug.Log(shadowFactor);
                //if (tiles[x,y].surfaceType != SurfaceType.Water) 
            }
        }

        return pixels;
    }
    float ComputeShadowFactor(int x, int y, int modifier)
    {
        Vector3 normal = modifier * ComputeNormal(x, y);
        Vector3 lightDir = (lightSource.transform.position - new Vector3(x, y, tiles[x, y].HeightValue)).normalized;
        float dot = Mathf.Max(Vector3.Dot(normal, lightDir), 0);
        return dot;
    }
    Vector3 ComputeNormal(int x, int y)
    {
        float heightL = tiles[Mathf.Clamp(x - 1, 0, width - 1), y].HeightValue;
        float heightR = tiles[Mathf.Clamp(x + 1, 0, width - 1), y].HeightValue;
        float heightD = tiles[x, Mathf.Clamp(y - 1, 0, height - 1)].HeightValue;
        float heightU = tiles[x, Mathf.Clamp(y + 1, 0, height - 1)].HeightValue;

        Vector3 normal = new Vector3(heightL - heightR, heightD - heightU, 3.0f).normalized;
        return normal;
    }
    #endregion
}