using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlanksLibrary : MonoBehaviour
{
    // Blank types
    public enum BlankType { Trunk, Chunk, Branch }

    [Header("Reference Data")]
    // Prefabs to instantiate blanks from
    [SerializeField] private TrunkSegment _trunkBlankPrefab; // For Trunk Parts
    [SerializeField] private ChunkManager _chunkManagerPrefab; // For Chunks
    [SerializeField] private BranchManager _branchManagerPrefab; // For Branches

    [Header("Properties")]
    // Number of blanks to pre-generate
    [SerializeField] private int _trunkBlanksCount = 70; // For Trunk Parts
    [SerializeField] private int _chunkBlanksCount = 10; // For Chunks
    [SerializeField] private int _branchBlanksCount = 50; // For Branches
    // Extra blanks generated when pool runs out
    [SerializeField] private int _emergencyBlanksCount = 10;  

    // --- State ---
    public bool IsReady { get; private set; }

    // --- Pool ---
    private ArrayList _trunkBlanks;                           // All instantiated Trunk blanks
    private Stack<TrunkSegment> _availableTrunkBlanks;        // Trunk blanks available for use

    private ArrayList _chunkBlanks;                           // All instantiated Chunk blanks
    private Stack<ChunkManager> _availableChunkBlanks;        // Chunk blanks available for use

    private ArrayList _branchBlanks;                          // All instantiated Branch blanks
    private Stack<BranchManager> _availableBranchBlanks;      // Branch blanks available for use

    private void Awake()
    {
        GenerateBlanks();

        // Give TreeLoader access to the BlanksLibrary
        TreeLoader.BlanksLibrary = this;
    }

    /// <summary>
    /// Instantiates <see cref="_trunkBlanksCount"/> copies of <see cref="_trunkBlankPrefab"/> and fills the available stack
    /// </summary>
    private void GenerateBlanks()
    {
        if (_trunkBlankPrefab == null)
        {
            Debug.LogError("TrunkBlankPrefab is not assigned!");
            return;
        }

        if (_chunkManagerPrefab == null)
        {
            Debug.LogError("ChunkManagerPrefab is not assigned!");
            return;
        }

        if (_branchManagerPrefab == null)
        {
            Debug.LogError("BranchManagerPrefab is not assigned!");
            return;
        }

        // --- Instantiate blanks ---
        // Trunk Parts
        _trunkBlanks = new ArrayList(_trunkBlanksCount);
        _availableTrunkBlanks = new Stack<TrunkSegment>(_trunkBlanksCount);
        for (int i = 0; i < _trunkBlanksCount; i++)
            CreateTrunkBlank();

        // Chunks
        _chunkBlanks = new ArrayList(_chunkBlanksCount);
        _availableChunkBlanks = new Stack<ChunkManager>(_chunkBlanksCount);
        for (int i = 0; i < _chunkBlanksCount; i++)
            CreateChunkBlank();

        // Branches
        _branchBlanks = new ArrayList(_branchBlanksCount);
        _availableBranchBlanks = new Stack<BranchManager>(_branchBlanksCount);
        for (int i = 0; i < _branchBlanksCount; i++)
            CreateBranchBlank();

        // Blanks are ready to use!
        IsReady = true;
        Debug.Log($"Generated Trunks: '{_trunkBlanks.Count}'");
        Debug.Log($"Generated Chunks: '{_chunkBlanks.Count}'");
        Debug.Log($"Generated Branches: '{_branchBlanks.Count}'");
    }

    /// <summary>
    /// Returns an available blank of the given <see cref="BlankType"/> from the pool
    /// Generates emergency blanks if the pool is empty
    /// </summary>
    public object GetBlank(BlankType type)
    {
        switch (type)
        {
            case BlankType.Trunk:
                // --- Emergency refill ---
                if (_availableTrunkBlanks.Count == 0)
                {
                    Debug.LogWarning($"No available TrunkBlanks! Generating {_emergencyBlanksCount} emergency blanks...");
                    for (int i = 0; i < _emergencyBlanksCount; i++)
                        CreateTrunkBlank();
                }
                return _availableTrunkBlanks.Pop();

            case BlankType.Chunk:
                // --- Emergency refill ---
                if (_availableChunkBlanks.Count == 0)
                {
                    Debug.LogWarning($"No available ChunkBlanks! Generating {_emergencyBlanksCount} emergency blanks...");
                    for (int i = 0; i < _emergencyBlanksCount; i++)
                        CreateChunkBlank();
                }
                return _availableChunkBlanks.Pop();

            case BlankType.Branch:
                // --- Emergency refill ---
                if (_availableBranchBlanks.Count == 0)
                {
                    Debug.LogWarning($"No available BranchBlanks! Generating {_emergencyBlanksCount} emergency blanks...");
                    for (int i = 0; i < _emergencyBlanksCount; i++)
                        CreateBranchBlank();
                }
                return _availableBranchBlanks.Pop();

            default:
                Debug.LogWarning($"Unknown BlankType: {type}. Could not get blank from pool");
                return null;
        }
    }

    /// <summary>
    /// Returns a used blank back to its corresponding available pool
    /// </summary>
    public void ReturnBlank(object blank, BlankType type)
    {
        switch (type)
        {
            case BlankType.Trunk:
                _availableTrunkBlanks.Push((TrunkSegment)blank);
                break;
            case BlankType.Chunk:
                _availableChunkBlanks.Push((ChunkManager)blank);
                break;
            case BlankType.Branch:
                _availableBranchBlanks.Push((BranchManager)blank);
                break;
            default:
                Debug.LogWarning($"Unknown BlankType: {type}. Could not return blank to pool");
                break;
        }
    }

    /// <summary>
    /// Instantiates a single Trunk blank and pushes it onto <see cref="_availableTrunkBlanks"/>
    /// </summary>
    private void CreateTrunkBlank()
    {
        TrunkSegment blank = Instantiate(_trunkBlankPrefab);
        blank.gameObject.SetActive(false); // Deactivate until needed
        _trunkBlanks.Add(blank);
        _availableTrunkBlanks.Push(blank);
    }

    /// <summary>
    /// Instantiates a single Chunk blank and pushes it onto <see cref="_availableChunkBlanks"/>
    /// </summary>
    private void CreateChunkBlank()
    {
        ChunkManager blank = Instantiate(_chunkManagerPrefab);
        blank.gameObject.SetActive(false); // Deactivate until needed
        _chunkBlanks.Add(blank);
        _availableChunkBlanks.Push(blank);
    }

    /// <summary>
    /// Instantiates a single Branch blank and pushes it onto <see cref="_availableBranchBlanks"/>
    /// </summary>
    private void CreateBranchBlank()
    {
        BranchManager blank = Instantiate(_branchManagerPrefab);
        blank.gameObject.SetActive(false); // Deactivate until needed
        _branchBlanks.Add(blank);
        _availableBranchBlanks.Push(blank);
    }
}
