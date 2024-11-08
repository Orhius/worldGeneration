using System;
using UnityEngine;

public static class GlobalEventManager 
{
    // World
    public static event Action OnWorldSceneIsLoaded;
    public static void WorldSceneIsLoaded() => OnWorldSceneIsLoaded?.Invoke();

    //Player

    public static event Action OnPlayerPositionChanged;
    public static void PlayerPositionChanged() => OnPlayerPositionChanged?.Invoke();

    public static event Action<Vector2Int> OnPlayerChunkPositionChanged;
    public static void PlayerChunkPositionChanged(Vector2Int vector) => OnPlayerChunkPositionChanged?.Invoke(vector);
}
