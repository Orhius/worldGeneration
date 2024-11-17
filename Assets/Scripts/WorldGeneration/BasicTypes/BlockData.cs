public struct BlockData
{
    //public int x, y, z;
    //public string blockName;
    public BlockType blockType;
}

public enum BlockType : byte
{
    Air,
    Surface
}