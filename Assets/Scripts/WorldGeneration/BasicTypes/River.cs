using System.Collections.Generic;
using UnityEngine;

public class River : MonoBehaviour
{
    /*
    [Range(0.5f, 0.9f)] public float weight = 0.6f;
    public Tile source;
    public int riverLength = 0;
    public List<Tile> riverTiles;
    public Vector2 startPosition;
    public Vector2 currentPosition;
    public Vector2 destinationPoint;
    public Vector2 currentDirection;

    Tile currentTile;
    Tile[,] tiles;

    public River(Vector2 startPoint, Vector2 destinationPoint, Tile[,] tiles)
    {
        currentDirection = Random.insideUnitCircle.normalized;
        currentPosition = startPoint;
        startPosition = startPoint;
        currentTile = tiles[(int)startPoint.x, (int)startPoint.y];
        this.destinationPoint = destinationPoint;
        this.tiles = tiles;
    }

    public static float RangeMap(float inputValue, float inMin, float inMax, float outMin, float outMax)
    {
        return outMin + (inputValue - inMin) * (outMax - outMin) / (inMax - inMin);
    }
    public Vector2 MoveToDirection()
    {
        Vector3 direction = GetPerlinNoiseDirection();
        var directionToConvergancePoint = (this.destinationPoint - currentPosition).normalized;
        var endDirection = ((Vector2)direction * (1 - weight) + directionToConvergancePoint * weight).normalized;
        currentPosition += endDirection;
        return currentPosition;
    }
    private Vector3 GetPerlinNoiseDirection()
    {
        float noise = currentTile.HeightValue;
        float degrees = RangeMap(noise, 0, 1, -90, 90);
        currentDirection = (Quaternion.AngleAxis(degrees, Vector3.forward) * currentDirection).normalized;
        return currentDirection;
    }
    */
}
