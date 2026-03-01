using UnityEngine;

[CreateAssetMenu(fileName = "NewBranchData", menuName = "Entries/BranchData")]
public class BranchData : ScriptableObject
{
    // NOTE FOR MYSELF:
    // BAD EXAMPLE -> [Range(0f, 1f)] public float slipperyValue = 0;
    // Everything here should be static
    // ====================================
    // GOOD EXAMPLE -> everything below lol

    [Header("Main Configuration")]
    // Unique identifier (e.g. "stone_sharp_01", "mossy_rounded_02")
    public string id;
    // On which side this trunk can be on
    // Can sprite be flipped vertically
    
    [Header("Points Configuration")]
    [Step(0.05f)] public Vector2 downNearPoint; // heart
    [Step(0.05f)] public Vector2 downFarPoint;
    [Step(0.05f)] public Vector2 topNearPoint;
    [Step(0.05f)] public Vector2 topFarPoint;


    //[Header("Gameplay Configuration")]
}