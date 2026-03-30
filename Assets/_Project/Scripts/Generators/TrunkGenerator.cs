using UnityEngine;

public class TrunkGenerator : MonoBehaviour
{
    [Header("Trunk Pool")]
    [SerializeField] private TrunkData[] _trunkDataPool;

    [Header("Visual Settings")]
    [SerializeField] private Color _color = Color.white;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void GenerateTrunk(System.Random random, TreeGen treeGen, float targetHeight)
    {
        if (_trunkDataPool == null || _trunkDataPool.Length == 0)
        {
            Debug.LogError("TrunkData pool is empty! Cannot generate trunk");
            return;
        }

        // Generate Trunk Parts
        GenerateSide(random, treeGen, targetHeight, Side.Right); // Generate right side
        GenerateSide(random, treeGen, targetHeight, Side.Left); // Generate left side

        Debug.Log($"Generated trunk with {CountTotalTrunkParts(treeGen)} total trunk parts");
    }

    private void GenerateSide(System.Random random, TreeGen treeGen, float targetHeight, Side side)
    {
        float currentHeight = 0f;

        // --- Sprite order alternation ---
        // Right: 111 -> 112 -> 111 -> ...
        // Left:  101 -> 102 -> 101 -> ...
        short spriteOrderBase = side == Side.Right ? (short)111 : (short)101;
        bool spriteOrderToggle = false; // false = base, true = base+1

        // --- Generate Trunk Parts ---
        while (currentHeight < targetHeight)
        {
            // Get random TrunkData from pool that can be on this side
            TrunkData selectedTrunkData = GetRandomTrunkDataForSide(random, side);

            if (selectedTrunkData == null)
            {
                Debug.LogWarning($"No suitable TrunkData found for {side} side");
                break;
            }

            // Generate random isYFlipped with 50/50 chance
            bool isYFlipped = selectedTrunkData.canBeYFlipped && random.Next(0, 2) == 1;

            // Compute alternating sprite order
            short spriteOrder = (short)(spriteOrderBase + (spriteOrderToggle ? 1 : 0));
            spriteOrderToggle = !spriteOrderToggle;

            // Create TrunkGen
            TrunkGen trunkGen = new TrunkGen(selectedTrunkData, side, isYFlipped, currentHeight, spriteOrder, _color);

            // Find which ChunkGen this TrunkPart belongs to
            ChunkGen targetChunk = FindChunkAtHeight(treeGen, currentHeight);

            if (targetChunk != null)
            {
                targetChunk.TrunkParts.Add(trunkGen);
            }
            else
            {
                Debug.LogWarning($"Could not find chunk at height {currentHeight}");
            }

            // Increment height
            currentHeight += selectedTrunkData.Height;
        }
    }

    private TrunkData GetRandomTrunkDataForSide(System.Random random, Side side)
    {
        // Filter available trunk data based on side
        int maxAttempts = _trunkDataPool.Length * 2; // Prevent infinite loop
        int attempts = 0;

        while (attempts < maxAttempts)
        {
            int randomIndex = random.Next(0, _trunkDataPool.Length);
            TrunkData candidate = _trunkDataPool[randomIndex];

            if (candidate == null)
            {
                attempts++;
                continue;
            }

            // Check if this TrunkData can be used on this side
            bool canUse = candidate.avaliableSide == TrunkAvaliableSide.Both ||
                         (candidate.avaliableSide == TrunkAvaliableSide.Left && side == Side.Left) ||
                         (candidate.avaliableSide == TrunkAvaliableSide.Right && side == Side.Right);

            if (canUse)
            {
                return candidate;
            }

            attempts++;
        }

        return null;
    }

    // --- HELPER METHODS ---

    /// <summary>
    /// Returns the Chunk that contains given Trunk Part
    /// </summary>
    /// <returns>ChunkGen object</returns>
    private ChunkGen FindChunkAtHeight(TreeGen treeGen, float height)
    {
        foreach (var chunk in treeGen.Chunks)
        {
            float chunkStartHeight = chunk.CurrentHeight;
            float chunkEndHeight = chunkStartHeight + chunk.ChunkData.height;

            // Check trunk height within current chunk
            if (height >= chunkStartHeight && height < chunkEndHeight)
            {
                return chunk;
            }
        }

        // If trunk above last chunk starting height -> return the last chunk
        if (treeGen.Chunks.Count > 0 && height >= treeGen.Chunks[treeGen.Chunks.Count - 1].CurrentHeight)
        {
            return treeGen.Chunks[treeGen.Chunks.Count - 1];
        }

        return null;
    }

    /// <summary>
    /// Counts total number of Trunk Parts
    /// </summary>
    private int CountTotalTrunkParts(TreeGen treeGen)
    {
        int count = 0;
        foreach (var chunk in treeGen.Chunks)
        {
            count += chunk.TrunkParts.Count;
        }
        return count;
    }
}
