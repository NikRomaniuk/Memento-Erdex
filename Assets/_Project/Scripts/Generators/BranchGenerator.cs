using UnityEngine;

public class BranchGenerator : MonoBehaviour
{
    [Header("Branch Pool")]
    [SerializeField] private BranchData[] _branchDataPool;

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

            float branchHeight = chunkGen.CurrentHeight + slot.yPoint;
            BranchGen branchGen = new BranchGen(selectedBranchData, slot.branchOrientation, branchHeight);
            chunkGen.Branches.Add(branchGen);
            generatedCount++;
        }

        Debug.Log($"Generated {generatedCount} branches for chunk at height {chunkGen.CurrentHeight}");
    }

    private BranchData GetRandomBranchDataForSide(System.Random random, BranchOrientation side)
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
                          (candidate.avaliableSide == BranchData.AvailableSide.Left && side == BranchOrientation.Left) ||
                          (candidate.avaliableSide == BranchData.AvailableSide.Right && side == BranchOrientation.Right);

            if (canUse)
            {
                return candidate;
            }

            attempts++;
        }

        return null;
    }

    private bool CanUseOrientation(BranchOrientation orientation)
    {
        switch (orientation)
        {
            case BranchOrientation.Left:
            case BranchOrientation.Right:
                return true;

            case BranchOrientation.Middle:
            default:
                return false;
        }
    }
}
