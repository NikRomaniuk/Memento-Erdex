using UnityEngine;

[System.Serializable]
public struct BranchSlot
{
    [Step(0.05f)] public float yPoint;
    public Orientation branchOrientation;

    public BranchSlot(float yPoint, Orientation branchOrientation)
    {
        this.yPoint = yPoint;
        this.branchOrientation = branchOrientation;
    }
}

[CreateAssetMenu(fileName = "NewChunkData", menuName = "Entries/ChunkData")]
public class ChunkData : ScriptableObject, IData
{
    string IData.id => id;

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