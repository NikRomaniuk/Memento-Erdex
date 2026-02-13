using UnityEngine;

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
    // Also its bounds will be used for overlap checks during generation
    public BoxCollider2D collider;
    // Visual representation
    public Sprite sprite;
    // Can sprite be flipped horizontally
    public bool canBeFlipped = true;
    // Inset for the trunk bounds, to avoid the holes between objects
    // Should be half of the in-game pixel
    [HideInInspector] public Vector2 boundsInset = new Vector2(0.1f, 0.1f);

    //[Header("Gameplay Configuration")]
}
