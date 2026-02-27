using UnityEngine;

public enum AvaliableSide
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
    public BoxCollider2D collider;
    // Visual representation
    public Sprite sprite;
    // On which side this trunk can be on
    public AvaliableSide avaliableSide = AvaliableSide.Both;
    // Can sprite be flipped vertically
    public bool canBeYFlipped = true;
    
    [Header("Points Configuration")]
    [Step(0.05f)] public Vector2 downNearPoint; // heart
    [Step(0.05f)] public Vector2 downFarPoint;
    [Step(0.05f)] public Vector2 topNearPoint;
    [Step(0.05f)] public Vector2 topFarPoint;


    //[Header("Gameplay Configuration")]
}
