public class BlockData
{
    public int x, y, z;
    public string blockName = string.Empty;
    public BlockType blockType = BlockType.Air;
}

public enum BlockType : byte
{
    Air,
    Surface
}