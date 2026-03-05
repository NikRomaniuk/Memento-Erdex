using UnityEngine;

public class ChunkGenerator : MonoBehaviour
{
    [Header("Chunk Pool")]
    [SerializeField] private ChunkData[] _chunkDataPool;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void GenerateChunks(System.Random random, TreeGen treeGen, float maxHeight)
    {
        if (_chunkDataPool == null || _chunkDataPool.Length == 0)
        {
            Debug.LogError("ChunkData pool is empty! Cannot generate chunks");
            return;
        }

        float currentHeight = 0f;

        while (currentHeight < maxHeight)
        {
            // Get random ChunkData from pool
            int randomIndex = random.Next(0, _chunkDataPool.Length);
            ChunkData selectedChunkData = _chunkDataPool[randomIndex];

            if (selectedChunkData == null)
            {
                Debug.LogWarning($"ChunkData at index {randomIndex} is null. Skipping");
                continue;
            }

            // Create ChunkGen with selected ChunkData and current height
            ChunkGen chunkGen = new ChunkGen(selectedChunkData, currentHeight);
            treeGen.Chunks.Add(chunkGen);

            // Increment height
            currentHeight += selectedChunkData.height;
        }

        Debug.Log($"Generated {treeGen.Chunks.Count} chunks with total height: {currentHeight}");
    }
}
