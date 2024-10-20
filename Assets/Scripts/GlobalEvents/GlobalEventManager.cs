using System;

public static class GlobalEventManager 
{
    // World
    public static event Action OnWorldSceneIsLoaded;
    public static void WorldSceneIsLoaded() => OnWorldSceneIsLoaded?.Invoke();
}
