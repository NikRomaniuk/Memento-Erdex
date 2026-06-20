using UnityEngine;

[CreateAssetMenu(fileName = "NewShapeData", menuName = "Entries/ShapeData")]
public class ShapeData : ScriptableObject, IData
{
    string IData.id => id;

    public enum Type
    {
        Base,
        Tip
    }

    // --- Main ---
    public string id; // Unique identifier
    public Sprite borderSprite;
    public Sprite shapeSprite;
    [Step(0.05f)] public Vector2 spritesOffset;
    public Color defaultOutlineColor = Color.black; // Default outline color
    public Type type = Type.Base; // Type of Shape
    public bool canBeXFlipped = true; // Can sprite be flipped horizontally

    // --- Points ---
    [Step(0.05f)] public Vector2 topNearPoint; // heart (anchor)
    [Step(0.05f)] public Vector2 downNearPoint; 
    [Step(0.05f)] public Vector2 topFarPoint;
    [Step(0.05f)] public Vector2 downFarPoint;

    // --- Clutter Slots ---
    public ClutterSlot[] clutterSlots;

    // Length calculated as distance from topNearPoint to topFarPoint
    public float Length => Vector2.Distance(topNearPoint, topFarPoint);
}