using UnityEngine;

[System.Serializable]
public struct PropSlot
{
    [Step(0.05f)] public float xPos;
    public PropData propData;

    public PropSlot(float xPos, PropData propData)
    {
        this.xPos = xPos;
        this.propData = propData;
    }
}

[CreateAssetMenu(fileName = "NewIslandData", menuName = "Entries/IslandData")]
public class IslandData : ScriptableObject, IData
{
    string IData.id => id;

    [Header("Main Data")]
    public string id; // Unique identifier

    [Header("Collider Data")]
    public Vector2 colliderSize;
    public Vector2 colliderOffset;

    [Header("Visual Data")]
    public Sprite sprite;
    [Step(0.05f)] public Vector2 spriteOffset;
    public bool canBeXFlipped = true;

    [Header("General Data")]
    public Size size = Size.Tiny;
    public bool allowLeft = true;
    public bool allowRight = true;
    public bool allowMiddle = true;

    [Header("Prop Data")]
    public PropSlot[] staticPropSlots;
    public int propCapacity;
}