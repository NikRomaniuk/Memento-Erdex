using UnityEngine;

public class BranchGenerator : MonoBehaviour
{
    [Header("Branch Pool")]
    [SerializeField] private BranchData[] _branchDataPool;

    // --- Components ---
    // Shape generation is chained to branch generation
    private ShapeGenerator _shapeGenerator;
    private IslandGenerator _islandGenerator;

    private void Awake()
    {
        _shapeGenerator = GetComponent<ShapeGenerator>();
        _islandGenerator = GetComponent<IslandGenerator>();
    }

    public void GenerateBranches(System.Random random, ChunkGen chunkGen)
    {
        // --- Validations ---
        if (_branchDataPool == null || _branchDataPool.Length == 0)
        {
            Debug.LogError("BranchData pool is empty! Cannot generate branches"); 
            return; 
        }

        BranchSlot[] branchSlots = chunkGen.ChunkData.branchSlots;
        if (branchSlots == null || branchSlots.Length == 0)
        {
            Debug.LogError("Branch slots are empty! Cannot generate branches"); 
            return;
        }

        int generatedCount = 0;

        foreach (BranchSlot slot in branchSlots)
        {
            if (!CanUseOrientation(slot.branchOrientation))
            {
                // Middle slots are skipped for now
                continue;
            }

            BranchData selectedBranchData = GetRandomBranchDataForSide(random, slot.branchOrientation);
            if (selectedBranchData == null)
            {
                Debug.LogWarning($"No suitable BranchData found for slot side {slot.branchOrientation} at local height {slot.yPoint}");
                continue;
            }

            // Store absolute branch origin -> it becomes the origin for shape chain placement
            Vector2 branchPos = new Vector2(GetBranchXPos(slot.branchOrientation), chunkGen.CurrentHeight + slot.yPoint);
            BranchGen branchGen = new BranchGen(selectedBranchData, slot.branchOrientation, branchPos);
            chunkGen.Branches.Add(branchGen);

            // Immediately generate Shapes for this branch from the same random stream
            _shapeGenerator.GenerateShapes(branchGen, random);

            // Immediately generate Islands for this branch from the same random stream
            if (_islandGenerator != null)
                _islandGenerator.GenerateIslands(random, branchGen);

            generatedCount++;
        }

        Debug.Log($"Generated {generatedCount} branches for chunk at height {chunkGen.CurrentHeight}");
    }

    private BranchData GetRandomBranchDataForSide(System.Random random, Orientation side)
    {
        int maxAttempts = _branchDataPool.Length * 2;
        int attempts = 0;

        while (attempts < maxAttempts)
        {
            int randomIndex = random.Next(0, _branchDataPool.Length);
            BranchData candidate = _branchDataPool[randomIndex];

            if (candidate == null)
            {
                attempts++;
                continue;
            }

            bool canUse = candidate.avaliableSide == BranchData.AvailableSide.Both ||
                          (candidate.avaliableSide == BranchData.AvailableSide.Left && side == Orientation.Left) ||
                          (candidate.avaliableSide == BranchData.AvailableSide.Right && side == Orientation.Right);

            if (canUse)
            {
                return candidate;
            }

            attempts++;
        }

        return null;
    }

    private bool CanUseOrientation(Orientation orientation)
    {
        switch (orientation)
        {
            case Orientation.Left:
            case Orientation.Right:
                return true;

            case Orientation.Middle:
            default:
                return false;
        }
    }

    private float GetBranchXPos(Orientation orientation)
    {
        // Absolute X is derived from branch side offset in current world layout
        switch (orientation)
        {
            case Orientation.Left:
                return -Constants.BRANCH_SLOT_X_OFFSET;

            case Orientation.Right:
                return Constants.BRANCH_SLOT_X_OFFSET;

            case Orientation.Middle:
            default:
                return 0f;
        }
    }
}
