using UnityEngine;

[CreateAssetMenu(fileName = "NewIslandData", menuName = "Entries/IslandData")]
public class IslandData : ScriptableObject, IData
{
    string IData.id => id;

    public enum Size
    {
        Tiny,
        Small,
        Medium
    }

    [Header("Main Configuration")]
    // Unique identifier
    public string id;
    // =======================================
    // Collider used for physics interactions
    public Vector2 colliderSize;
    public Vector2 colliderOffset;
    // =======================================
    // Visual representation
    public Sprite sprite;
    // Sprite offset
    [Step(0.05f)] public Vector2 spriteOffset;

    //[Header("Gameplay Configuration")]
}