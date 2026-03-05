using UnityEngine;

public enum TrunkAvaliableSide
{
    Both,
    Left,
    Right
}

[CreateAssetMenu(fileName = "NewTrunkData", menuName = "Entries/TrunkData")]
public class TrunkData : ScriptableObject
{
    // NOTE FOR MYSELF:
    // BAD EXAMPLE -> [Range(0f, 1f)] public float slipperyValue = 0;
    // Everything here should be static
    // ====================================
    // GOOD EXAMPLE -> everything below lol

    [Header("Main Configuration")]
    // Unique identifier (e.g. "stone_sharp_01", "mossy_rounded_02")
    public string id;
    // Collider used for physics interactions
    public Vector2 colliderSize;
    public Vector2 colliderOffset;
    // Visual representation
    public Sprite sprite;
    // Sprite offset
    [Step(0.05f)] public Vector2 spriteOffset;
    // On which side this trunk can be on
    public TrunkAvaliableSide avaliableSide = TrunkAvaliableSide.Both;
    // Can sprite be flipped vertically
    public bool canBeYFlipped = true;
    
    [Header("Points Configuration")]
    [Step(0.05f)] public Vector2 downNearPoint; // heart
    [Step(0.05f)] public Vector2 downFarPoint;
    [Step(0.05f)] public Vector2 topNearPoint;
    [Step(0.05f)] public Vector2 topFarPoint;

    // Height calculated as distance from downNearPoint to topNearPoint
    public float Height => Vector2.Distance(downNearPoint, topNearPoint);


    //[Header("Gameplay Configuration")]
}
