using UnityEngine;
public class RiversGenerator
{
    private int riverWidth = 10;
    private float frequency = 2.2f;
    private float amplitude = 8.0f;
    private float maxDepth = Generator.deepEdge;
    private float minDepth = Generator.shallowEdge;

    public (Tile[,], Color[]) GeneratePath(Vector2 startPosition, Vector2 endPosition, Tile[,] tiles, Color[] pixels, int worldSize)
    {
        int pathLength = 300;
        float distance = Vector2.Distance(startPosition, endPosition);
        float step = distance / pathLength*2;
        Vector2 direction = (endPosition - startPosition).normalized;

        for (int i = 0; i <= pathLength; i++)
        {
            riverWidth = Random.Range(2, 3);

            float t = (float)i / pathLength;
            Vector2 linearPosition = startPosition + direction * (t * distance);
            float sineOffset = Mathf.Sin(t * frequency * Mathf.PI * 2) * amplitude;
            Vector2 offset = new Vector2(-direction.y, direction.x) * sineOffset;
            Vector2 position = linearPosition + offset;

            if(position.x > worldSize - 1 || position.y > worldSize - 1 || position.x <= 0 || position.y <= 0) return (tiles, pixels);

            if (tiles[(int)position.x, (int)position.y].surfaceType == SurfaceType.Water) return (tiles, pixels);

            (tiles, pixels) = DigAtPoint(worldSize, position, tiles, pixels);

            frequency += Random.Range(-0.1f, 0.1f);
            amplitude += Random.Range(-0.2f, 0.2f);
        }
        return (tiles, pixels);
    }
    private (Tile[,], Color[]) DigAtPoint(int worldSize, Vector2 point, Tile[,] tiles, Color[] pixels)
    {
        int centerX = Mathf.RoundToInt(point.x);
        int centerY = Mathf.RoundToInt(point.y);

        if (centerX > worldSize - 1 || centerY > worldSize - 1 || centerX <= 0 || centerY <= 0) return (tiles, pixels);

        tiles[centerX, centerY].surfaceType = SurfaceType.River;
        tiles[centerX, centerY].heightType = HeightType.ShallowWater;
        tiles[centerX, centerY].HeightValue = Generator.shallowEdge;
        pixels[centerX + centerY * worldSize] = Generator.shallowColor;

        riverWidth = Random.Range(2, 3);

        for (int x = 0; x < riverWidth; x++)
        {
            for (int y = 0; y < riverWidth; y++)
            {
                int pixelX = Mathf.Clamp(centerX + x, 0, worldSize - 1);
                int pixelY = Mathf.Clamp(centerY + y, 0, worldSize - 1);
                float distance = Vector2.Distance(new Vector2(centerX, centerY), new Vector2(pixelX, pixelY));
                float depth = Mathf.Lerp(minDepth, maxDepth, distance / riverWidth);

                if (depth <= Generator.shallowEdge && depth > Generator.deepEdge)
                {
                    tiles[pixelX, pixelY].surfaceType = SurfaceType.River;
                    tiles[pixelX, pixelY].heightType = HeightType.ShallowWater;
                    pixels[pixelX + pixelY * worldSize] = Generator.shallowColor;
                    tiles[pixelX, pixelY].HeightValue = depth;
                }
                else
                {
                    tiles[pixelX, pixelY].surfaceType = SurfaceType.River;
                    tiles[pixelX, pixelY].heightType = HeightType.DeepWater;
                    pixels[pixelX + pixelY * worldSize] = Generator.deepColor;
                    tiles[pixelX, pixelY].HeightValue = depth;
                }
            }
        }
        return (tiles, pixels);
    }
}
