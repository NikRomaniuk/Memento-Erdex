using UnityEngine;

[CreateAssetMenu(fileName = "NewShapeData", menuName = "Entries/ShapeData")]
public class ShapeData : ScriptableObject, IData
{
    string IData.id => id;

    public enum Size
    {
        Tiny,
        Small,
        Medium
    }

    public enum Type
    {
        Base,
        Tip
    }

    [Header("Main Configuration")]
    // Unique identifier
    public string id;

    [Header("Points Configuration")]
    [Step(0.05f)] public Vector2 downNearPoint; 
    [Step(0.05f)] public Vector2 downFarPoint;
    [Step(0.05f)] public Vector2 topNearPoint; // heart (anchor)
    [Step(0.05f)] public Vector2 topFarPoint;
}