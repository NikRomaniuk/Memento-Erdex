using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
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

    // --- Public accessors ---
    public float Height => _height;
    public BranchSlot[] BranchSlots => _branchSlots;
}


