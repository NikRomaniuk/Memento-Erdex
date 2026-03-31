using System.Collections.Generic;
using UnityEngine;

public class ChunkGenerator : MonoBehaviour
{
    [System.Serializable]
    public class StaticChunkEntry
    {
        public ChunkData ChunkData;
        [Step(0.05f)] public float Height;
    }

    [Header("Chunk Pool")]
    [SerializeField] private ChunkData[] _chunkDataPool;

    [Header("Static Chunks")]
    [SerializeField] private List<StaticChunkEntry> _staticChunks = new List<StaticChunkEntry>();

    public int StaticChunksSpawned { get; private set; }
    public int StaticChunksAmount { get; private set; }

    // --- Components ---
    private BranchGenerator _branchGenerator;

    private void Awake()
    {
        _branchGenerator = GetComponent<BranchGenerator>();
    }

    public void GenerateChunks(System.Random random, TreeGen treeGen, float maxHeight)
    {
        // --- Validations ---
        if (_chunkDataPool == null || _chunkDataPool.Length == 0)
        {
            Debug.LogError("ChunkData pool is empty! Cannot generate chunks");
            return;
        }

        if (_branchGenerator == null)
        {
            Debug.LogError("BranchGenerator component not found! Cannot generate branches for chunks");
            return;
        }

        float currentHeight = 0f;
        List<StaticChunkEntry> pendingStaticChunks = _staticChunks != null
            ? new List<StaticChunkEntry>(_staticChunks)
            : new List<StaticChunkEntry>();

        StaticChunksSpawned = 0;
        StaticChunksAmount = pendingStaticChunks.Count;

        while (currentHeight < maxHeight)
        {
            // First prioritize spawning any static Chunks
            ChunkData selectedChunkData = TryGetStaticChunkData(currentHeight, pendingStaticChunks);

            // Successfully got a static ChunkData -> Increment static spawn count
            if (selectedChunkData != null)
            {
                StaticChunksSpawned++;
            }
            else // No static ChunkData available -> Get random ChunkData from pool
            {
                // Get random ChunkData from pool
                int randomIndex = random.Next(0, _chunkDataPool.Length);
                selectedChunkData = _chunkDataPool[randomIndex];

                if (selectedChunkData == null)
                {
                    Debug.LogWarning($"ChunkData at index {randomIndex} is null. Skipping");
                    continue;
                }
            }

            // Create ChunkGen with selected ChunkData and current height
            ChunkGen chunkGen = new ChunkGen(selectedChunkData, currentHeight);
            treeGen.Chunks.Add(chunkGen);

            // Generate branches for this chunk immediately after creation
            _branchGenerator.GenerateBranches(random, chunkGen);

            // Increment height
            currentHeight += selectedChunkData.height;
        }

        Debug.Log($"Static chunks spawned: {StaticChunksSpawned}/{StaticChunksAmount}");
        Debug.Log($"Generated {treeGen.Chunks.Count} chunks with total height: {currentHeight}");
    }

    private ChunkData TryGetStaticChunkData(float currentHeight, List<StaticChunkEntry> pendingStaticChunks)
    {
        // --- Validations ---
        if (pendingStaticChunks == null || pendingStaticChunks.Count == 0) { return null; }

        // Iterate through pending Static Chunks
        for (int i = 0; i < pendingStaticChunks.Count; i++)
        {
            StaticChunkEntry entry = pendingStaticChunks[i];

            if (entry == null)
            {
                pendingStaticChunks.RemoveAt(i);
                i--;
                continue;
            }

            // Static Chunk is not ready to be spawned yet -> Skip for now
            if (currentHeight < entry.Height)
            {
                continue;
            }

            // Static Chunk ready to be spawned -> Remove from pending list
            pendingStaticChunks.RemoveAt(i);

            if (entry.ChunkData == null)
            {
                Debug.LogWarning($"Static chunk entry at height {entry.Height} has no ChunkData. Skipping");
                i--;
                continue;
            }

            // Return Static Chunk's ChunkData
            return entry.ChunkData;
        }

        return null;
    }
}
