using UnityEngine;

public class TrunkGen
{
    public TrunkData TrunkData { get; private set; }
    public Side Side { get; private set; }
    public bool IsYFlipped { get; private set; }

    public TrunkGen(TrunkData trunkData, Side side, bool isYFlipped)
    {
        TrunkData = trunkData;
        Side = side;
        IsYFlipped = isYFlipped;
    }
}
