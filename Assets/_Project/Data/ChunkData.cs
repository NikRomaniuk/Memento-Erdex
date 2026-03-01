using UnityEngine;

public enum BranchOrientation
{
    Left,
    Right,
    Middle
}

[System.Serializable]
public struct BranchSlot
{
    [Step(0.05f)] public float yPoint;
    public BranchOrientation branchOrientation;

    public BranchSlot(float yPoint, BranchOrientation branchOrientation)
    {
        this.yPoint = yPoint;
        this.branchOrientation = branchOrientation;
    }
}

[CreateAssetMenu(fileName = "NewChunkData", menuName = "Entries/ChunkData")]
public class ChunkData : ScriptableObject
{
    // NOTE FOR MYSELF:
    // BAD EXAMPLE -> [Range(0f, 1f)] public float slipperyValue = 0;
    // Everything here should be static
    // ====================================
    // GOOD EXAMPLE -> everything below lol

    [Header("Main Configuration")]
    // Unique identifier (e.g. "stone_sharp_01", "mossy_rounded_02")
    public string id;
    // Height of the Chunk
    [Step(0.05f)] public float height;
    public BranchSlot[] branchSlots;

    //[Header("Gameplay Configuration")]

    private void OnValidate()
    {
        if (branchSlots == null) return;

        for (int i = 0; i < branchSlots.Length; i++)
        {
            branchSlots[i].yPoint = Mathf.Clamp(branchSlots[i].yPoint, 0f, height);
        }
    }
}