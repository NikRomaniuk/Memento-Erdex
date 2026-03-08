using UnityEngine;

/// <summary>
/// <para> A class that holds generated data for a <see cref="TrunkSegment"/> from the <see cref="TrunkGenerator"/> </para>
/// <para> Contains: </para>
/// <list>
///   <item> - TrunkData: The ScriptableObject containing STATIC Data </item>
///   <item> - Settings: Extra DYNAMIC Data needed for loading the <see cref="TrunkSegment"/> into Scene </item>
/// </list>
/// </summary>
public class TrunkGen
{
    // --- TrunkData ---
    public TrunkData TrunkData { get; private set; }

    // --- Settings ---
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
