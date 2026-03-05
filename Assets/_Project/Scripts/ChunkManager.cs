using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [Header("Data")]
    [SerializeField] private ChunkData _chunkData;
    // --- References ---

    // --- General ---
    [Step(0.05f)] [SerializeField] private float _height = 0f;
    //private float _currentHeight = 0f; // Current height of the chunk

    // --- Maths ---
    // Branch points
    [SerializeField] BranchSlot[] branchSlots;


    private void Awake()
    {
        // --- Preparations ---
        // None

        Initialize(_chunkData, Side.Right);
    }

    private void ApplyData()
    {
        if (_chunkData == null) return;

        // --- Apply general data ---
        _height = _chunkData.height;

        // --- Apply slots data ---
        // Copy branch slots from ChunkData
        if (_chunkData.branchSlots != null)
        {
            branchSlots = new BranchSlot[_chunkData.branchSlots.Length];
            System.Array.Copy(_chunkData.branchSlots, branchSlots, _chunkData.branchSlots.Length);
        }
        else
        {
            branchSlots = new BranchSlot[0];
        }
        
    }

    /// <summary>
    /// Initialize segment with ChunkData
    /// </summary>
    public void Initialize(ChunkData data, Side side)
    {
        _chunkData = data;
        ApplyData();
    }

    // Public accessors
    public ChunkData Data => _chunkData;
    //public float CurrentHeight => _currentHeight;
}


