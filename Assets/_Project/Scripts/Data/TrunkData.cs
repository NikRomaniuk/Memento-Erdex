using UnityEngine;

public enum TrunkAvaliableSide
{
    Both,
    Left,
    Right
}

[System.Serializable]
public struct ClutterSlot
{
    public bool isStatic;
    // Local position of Clutter Slot center along Trunk
    [Step(0.05f)] public Vector2 pos;

    // Used when isStatic == True
    public ClutterData staticClutterData;

    // Used when isStatic == False
    public Size size;

    public ClutterSlot(bool isStatic, Vector2 pos, ClutterData staticClutterData, Size size)
    {
        this.isStatic = isStatic;
        this.pos = pos;
        this.staticClutterData = staticClutterData;
        this.size = size;
    }
}

[CreateAssetMenu(fileName = "NewTrunkData", menuName = "Entries/TrunkData")]
public class TrunkData : ScriptableObject, IData
{
    string IData.id => id;

    // NOTE FOR MYSELF:
    // BAD EXAMPLE -> [Range(0f, 1f)] public float slipperyValue = 0;
    // Everything here should be static
    // ====================================
    // GOOD EXAMPLE -> everything below lol

    // --- Main ---
    public string id; // Unique identifier (e.g. "stone_sharp_01", "mossy_rounded_02")
    public Sprite shapeSprite;
    public Sprite borderSprite;
    [Step(0.05f)] public Vector2 spritesOffset;
    public Color defaultOutlineColor = Color.black; // Default outline color
    public TrunkAvaliableSide avaliableSide = TrunkAvaliableSide.Both; // On which side this trunk can be on
    public bool canBeYFlipped = true; // Can sprite be flipped vertically
    
    // --- Points ---
    [Step(0.05f)] public Vector2 downNearPoint; // heart (anchor)
    [Step(0.05f)] public Vector2 downFarPoint;
    [Step(0.05f)] public Vector2 topNearPoint;
    [Step(0.05f)] public Vector2 topFarPoint;

    // --- Clutter Slots ---
    public ClutterSlot[] clutterSlots;

    // Height calculated as distance from downNearPoint to topNearPoint
    public float Height => Vector2.Distance(downNearPoint, topNearPoint);
}
