using UnityEngine;

/// <summary>
/// <para> A class that holds generated data for a <see cref="ShapeManager"/> from the <see cref="ShapeGenerator"/> </para>
/// <para> Contains: </para>
/// <list>
///   <item> - ShapeData: The ScriptableObject containing STATIC Data </item>
///   <item> - Settings: Extra DYNAMIC Data needed for loading the <see cref="ShapeManager"/> into Scene </item>
/// </list>
/// </summary>
public class ShapeGen
{
    // --- SO Data ---
	public ShapeData ShapeData { get; private set; }

    // --- Settings ---
    public Side Side { get; private set; }
    public bool IsXFlipped { get; private set; }
    public float Height { get; private set; }
    public float YPos { get; private set; }
    public short SpriteOrder { get; private set; }

    public ShapeGen(ShapeData shapeData, Side side, bool isXFlipped, float height, float yPos, short spriteOrder)
    {
        ShapeData = shapeData;
        Side = side;
        IsXFlipped = isXFlipped;
        Height = height;
        YPos = yPos;
        SpriteOrder = spriteOrder;
    }
}
