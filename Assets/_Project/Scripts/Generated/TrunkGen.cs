using UnityEngine;

public class TrunkGen
{
    public TrunkData TrunkData { get; private set; }
    public Side Side { get; private set; }
    public bool IsYFlipped { get; private set; }
    public float Height { get; private set; }
    public short SpriteOrder { get; private set; }

    public TrunkGen(TrunkData trunkData, Side side, bool isYFlipped, float height, short spriteOrder)
    {
        TrunkData = trunkData;
        Side = side;
        IsYFlipped = isYFlipped;
        Height = height;
        SpriteOrder = spriteOrder;
    }
}
