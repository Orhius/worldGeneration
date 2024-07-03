using UnityEngine;

public static class Generator
{
    public static Tile[,] tiles;

    public static float deepEdge = .5f;
    public static float shallowEdge = .6f;
    public static float sandEdge = .62f;
    public static float grassEdge = .75f;
    public static float forestEdge = .85f;
    public static float rockEdge = .95f;
    public static float snowEdge = 1f;

    public static float coldestEdge = .1f;
    public static float colderEdge = .15f;
    public static float coldEdge = .3f;
    public static float warmEdge = .5f;
    public static float warmerEdge = .7f;
    public static float warmestEdge = 1f;

    public static int MoistureOctaves = 4;
    public static double MoistureFrequency = 3.0;
    public static float DryestValue = 0.15f;
    public static float DryerValue = 0.25f;
    public static float DryValue = 0.4f;
    public static float WetValue = 0.6f;
    public static float WetterValue = 0.8f;
    public static float WettestValue = 0.9f;

    public static Color deepColor = new Color(37 / 255f, 109 / 255f, 123 / 255f, 1);
    public static Color shallowColor = new Color(66 / 255f, 170 / 255f, 255 / 255f, 1);
    public static Color sandColor = new Color(240 / 255f, 232 / 255f, 145 / 255f, 1);
    public static Color grassColor = new Color(46 / 255f, 139 / 255f, 87 / 255f, 1);
    public static Color forestColor = new Color(47 / 255f, 69 / 255f, 56 / 255f, 1);
    public static Color rockColor = new Color(142 / 255f, 125 / 255f, 107 / 255f, 1);
    public static Color snowColor = new Color(1, 1, 1, 1);

    public static Color warmestColor = new Color(255 / 255f, 93 / 255f, 87 / 255f, 1);
    public static Color warmerColor = new Color(252 / 255f, 140 / 255f, 136 / 255f, 1);
    public static Color warmColor = new Color(251 / 255f, 187 / 255f, 152 / 255f, 1);
    public static Color coldColor = new Color(190 / 255f, 245 / 255f, 116 / 255f, 1);
    public static Color colderColor = new Color(127 / 255f, 255 / 255f, 212 / 255f, 1);
    public static Color coldestColor = new Color(175 / 255f, 218 / 255f, 252 / 255f, 1);

    public static Color DryestColor = new Color(214 / 255f, 138 / 255f, 89 / 255f, 1);
    public static Color DryerColor = new Color(220 / 255f, 211 / 255f, 106 / 255f, 1);
    public static Color DryColor = new Color(140 / 255f, 203 / 255f, 94 / 255f, 1);
    public static Color WetColor = new Color(72 / 255f, 209 / 255f, 204 / 255f, 1);
    public static Color WetterColor = new Color(102 / 255f, 153 / 255f, 204 / 255f, 1);
    public static Color WettestColor = new Color(42 / 255f, 82 / 255f, 190 / 255f, 1);

    public static Color IceColor = Color.white;
    public static Color DesertColor = new Color(238 / 255f, 218 / 255f, 130 / 255f, 1);
    public static Color SavannaColor = new Color(140 / 255f, 138 / 255f, 79 / 255f, 1);
    public static Color TropicalRainforestColor = new Color(96 / 255f, 140 / 255f, 59 / 255f, 1);
    public static Color TundraColor = new Color(96 / 255f, 131 / 255f, 112 / 255f, 1);
    public static Color TemperateRainforestColor = new Color(59 / 255f, 140 / 255f, 90 / 255f, 1);
    public static Color GrasslandColor = new Color(164 / 255f, 225 / 255f, 99 / 255f, 1);
    public static Color SeasonalForestColor = new Color(73 / 255f, 100 / 255f, 35 / 255f, 1);
    public static Color BorealForestColor = new Color(95 / 255f, 115 / 255f, 62 / 255f, 1);
    public static Color WoodlandColor = new Color(139 / 255f, 175 / 255f, 90 / 255f, 1);

    static BiomeType[,] BiomeTable = new BiomeType[6, 6] 
    {   
    //COLDEST        //COLDER          //COLD                  //HOT                          //HOTTER                       //HOTTEST
    { BiomeType.Ice, BiomeType.Tundra, BiomeType.Grassland,    BiomeType.Grassland,              BiomeType.Desert,              BiomeType.Desert },              //DRYEST
    { BiomeType.Ice, BiomeType.Tundra, BiomeType.Grassland,    BiomeType.Grassland,             BiomeType.Savanna,             BiomeType.Desert },              //DRYER
    { BiomeType.Ice, BiomeType.Tundra, BiomeType.Woodland,     BiomeType.Woodland,            BiomeType.Savanna,             BiomeType.Desert },             //DRY
    { BiomeType.Ice, BiomeType.Tundra, BiomeType.BorealForest, BiomeType.Woodland,            BiomeType.Woodland,             BiomeType.Savanna },             //WET
    { BiomeType.Tundra, BiomeType.Tundra, BiomeType.BorealForest, BiomeType.SeasonalForest,      BiomeType.TemperateRainforest,  BiomeType.TropicalForest },  //WETTER
    { BiomeType.Tundra, BiomeType.Tundra, BiomeType.BorealForest, BiomeType.TemperateRainforest, BiomeType.TropicalForest,  BiomeType.TropicalForest }   //WETTEST
    };
    public static BiomeType GetBiomeType(Tile tile)
    {
        return BiomeTable[(int) tile.moistureType, (int)tile.heatType];
    }
}
public enum HeightType : byte
{
    DeepWater = 0,
    ShallowWater = 1,
    Shore = 2,
    Sand = 3,
    Grass = 4,
    Forest = 5,
    Rock = 6,
    Snow = 7,
    River = 8
}

public enum HeatType : byte
{
    Coldest = 0,
    Colder = 1,
    Cold = 2,
    Warm = 3,
    Warmer = 4,
    Warmest = 5
}

public enum MoistureType : byte
{
    Dryest = 0,
    Dryer = 1,
    Dry = 2,
    Wet = 3,
    Wetter = 4,
    Wettest = 5
}
public enum SurfaceType : byte
{
    Land = 0,
    Water = 1,
    River = 2
}
public enum BiomeType : byte
{
    Desert = 0, // t > 24 осадки < 50 см
    Savanna = 1, // t > 24 50 < осадки < 250 см
    TropicalForest = 2, // t > 20 осадки > 250 см
    TemperateRainforest = 3, // 0 < t < 24 300 < осадки < 360 см
    SeasonalForest = 4, // 0 < t < 20 160 < осадки < 300 см
    Grassland = 5, // 0 < t < 20 осадки < 50 см
    Woodland = 6, // 0 < t < 20 осадки < 160 см
    BorealForest = 7, // -10 < t < 0 осадки < 200 см
    Tundra = 8, // -20 < t <= 0 осадки < 50 см
    Ice = 9 // t < -20
}