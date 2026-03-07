using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour, IBakeable, IBuildable
{
    [Header("Properties")]
    [Step(0.05f)] [SerializeField] private float _height = 0f;
    [SerializeField] private BranchSlot[] _branchSlots;

    // --- Runtime ---
    [HideInInspector] public List<TrunkSegment> LoadedTrunks = new List<TrunkSegment>(); // Active TrunkSegments loaded for this chunk

    /// <summary>
    /// Set up private fields with given data
    /// </summary>
    public void SetData(float height, BranchSlot[] branchSlots)
    {
        _height = height;
        _branchSlots = branchSlots;
    }

    // --- IBakeable ---
    public void GatherData(IData data)
    {
        if (data is not ChunkData chunkData) return;

        chunkData.height = _height;
        chunkData.branchSlots = _branchSlots != null ? (BranchSlot[])_branchSlots.Clone() : new BranchSlot[0];
    }

    // --- IBuildable ---
    public void SetData() { }

    // --- Public accessors ---
    public float Height => _height;
    public BranchSlot[] BranchSlots => _branchSlots;
}


