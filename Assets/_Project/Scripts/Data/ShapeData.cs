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

    [Header("Main Configuration")]
    public Type type = Type.Base;
    // Unique identifier
    public string id;
    // Visual representation
    public Sprite sprite;
    // Sprite offset
    [Step(0.05f)] public Vector2 spriteOffset;
    // Default outline color
    public Color defaultOutlineColor = Color.black;
    // Can sprite be flipped horizontally
    public bool canBeXFlipped = true;

    [Header("Points Configuration")]
    [Step(0.05f)] public Vector2 topNearPoint; // heart (anchor)
    [Step(0.05f)] public Vector2 downNearPoint; 
    [Step(0.05f)] public Vector2 topFarPoint;
    [Step(0.05f)] public Vector2 downFarPoint;

    // Length calculated as distance from topNearPoint to topFarPoint
    public float Length => Vector2.Distance(topNearPoint, topFarPoint);
}