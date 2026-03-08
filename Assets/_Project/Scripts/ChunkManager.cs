using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour, IBakeable, IBuildable
{
    [Header("Properties")]
    [Step(0.05f)] [SerializeField] private float _height = 0f;
    [SerializeField] private BranchSlot[] _branchSlots;

    // --- Runtime ---
    [HideInInspector] public List<TrunkSegment> LoadedTrunks = new List<TrunkSegment>(); // Active TrunkSegments loaded for this chunk

    // --- IBakeable ---
    public void GatherData(IData data)
    {
        if (data is not ChunkData chunkData) return;

        chunkData.height = _height;
        chunkData.branchSlots = _branchSlots != null ? (BranchSlot[])_branchSlots.Clone() : new BranchSlot[0];
    }

    // --- IBuildable ---

    /// <summary>
    /// Set up private fields with given data
    /// </summary>
    public void SetData(float height, BranchSlot[] branchSlots)
    {
        _height = height;
        _branchSlots = branchSlots;
    }
    public void SetData(IData data)
    {
        if (data is not ChunkData chunkData) return;
        SetData(chunkData.height, chunkData.branchSlots != null ? (BranchSlot[])chunkData.branchSlots.Clone() : new BranchSlot[0]);
    }

    public void Clear()
    {
        SetData(0f, new BranchSlot[0]);

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
        UnityEditor.EditorUtility.SetDirty(gameObject);
#endif
        gameObject.SetActive(false);
    }

    // --- Public accessors ---
    public float Height => _height;
    public BranchSlot[] BranchSlots => _branchSlots;
}


