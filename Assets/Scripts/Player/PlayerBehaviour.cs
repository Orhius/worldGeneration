using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    public Vector3Int playerCoord { get; private set; }
    public Vector2Int currentChunk;
    private void OnEnable()
    {
        GlobalEventManager.OnPlayerPositionChanged += CheckCurrentChunk;
    }
    private void OnDisable()
    {
        GlobalEventManager.OnPlayerPositionChanged -= CheckCurrentChunk;
    }

    private void Start()
    {
        GlobalEventManager.PlayerChunkPositionChanged(currentChunk);
    }
    private void Update()
    {
        Vector3Int lastPlayerCoord = playerCoord;
        playerCoord = new Vector3Int((int)transform.position.x, (int)transform.position.y, (int)transform.position.z);
        if (lastPlayerCoord != playerCoord) GlobalEventManager.PlayerPositionChanged();
    }

    private void CheckCurrentChunk()
    {
        Vector2Int oldChunk = currentChunk;
        currentChunk = new Vector2Int(playerCoord.x / WorldGenerator.chunkSize, playerCoord.z / WorldGenerator.chunkSize);
        if (playerCoord.x < 0) currentChunk.x -= 1;
        if (playerCoord.z < 0) currentChunk.y -= 1;

        if(oldChunk != currentChunk) GlobalEventManager.PlayerChunkPositionChanged(currentChunk);
    }
}
