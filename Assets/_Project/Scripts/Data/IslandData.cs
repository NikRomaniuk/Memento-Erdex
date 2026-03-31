using UnityEngine;

[System.Serializable]
public struct InteractiveSlot
{
    public bool isStatic;
    [Step(0.05f)] public Vector2 pos;

    // Used when isStatic == True
    public InteractiveData staticData;

    // Used when isStatic == False

    public InteractiveSlot(bool isStatic, Vector2 pos, InteractiveData staticData)
    {
        this.isStatic = isStatic;
        this.pos = pos;
        this.staticData = staticData;
    }
}

[System.Serializable]
public struct InteractiveFormation
{
    public InteractiveSlot[] interactiveSlots;
    [Min(0)] public int randomValue;

    public InteractiveFormation(InteractiveSlot[] interactiveSlots, int randomValue)
    {
        this.interactiveSlots = interactiveSlots;
        this.randomValue = Mathf.Max(0, randomValue);
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
    public bool allowLeft;
    public bool allowRight;
    public bool allowMiddle;

    [Header("Interactive Formations")]
    public InteractiveFormation[] interactiveFormations;

    private void OnValidate()
    {
        // Safety net for editor-time data integrity
        // [Min(0)] helps in Inspector UI, but this guarantees clamping after any deserialization/editor changes
        if (interactiveFormations == null) return;

        for (int i = 0; i < interactiveFormations.Length; i++)
        {
            InteractiveFormation formation = interactiveFormations[i];
            // Keep randomValue non-negative for all formations
            formation.randomValue = Mathf.Max(0, formation.randomValue);
            interactiveFormations[i] = formation;
        }
    }
}