using System.Collections.Generic;
using UnityEngine;

public static class TreeLoader
{
    // --- References ---
    public static TreeGen GenData; // Generated tree data
    public static BlanksLibrary BlanksLibrary; // Blanks of all kinds to build the Tree with!

    // --- Settings ---
    // How many chunks to load before and after the current one
    // Example: If current chunk is [5] and LoadRadius is 3 then loaded:
    // 2-3-4-[5]-6-7-8
    public static int LoadRadius = 3;

    // --- State ---
    // Indices of currently loaded chunks
    public static List<int> LoadedChunks = new List<int>(); 
    // Map: Сhunk index -> ChunkManager
    public static Dictionary<int, ChunkManager> LoadedChunkObjects = new Dictionary<int, ChunkManager>();

    /// <summary>
    /// Ensures the given Chunk and its neighbors within <see cref="LoadRadius"/> are loaded
    /// Unloads chunks that are no longer in range
    /// </summary>
    public static void KeepLoaded(int chunkIndex)
    {
        if (GenData == null || GenData.Chunks.Count == 0) return;
        Debug.Log($"LoadedChunks: '{LoadedChunks.Count}'");

        // --- Compute range ---
        int min = System.Math.Max(0, chunkIndex - LoadRadius);                          // Clamp to list start
        int max = System.Math.Min(GenData.Chunks.Count - 1, chunkIndex + LoadRadius);   // Clamp to list end

        var desired = new HashSet<int>();
        for (int i = min; i <= max; i++)
            desired.Add(i);

        // --- Unload Chunks outside range ---
        for (int i = LoadedChunks.Count - 1; i >= 0; i--)
        {
            int idx = LoadedChunks[i];
            if (!desired.Contains(idx))
            {
                Unload(idx);
                LoadedChunks.RemoveAt(i);
            }
        }

        // --- Load new Chunks inside range ---
        foreach (int idx in desired)
        {
            if (!LoadedChunks.Contains(idx))
            {
                Load(idx);
                LoadedChunks.Add(idx);
            }
        }
    }

    /// <summary>
    /// Load a Chunk by index into the Scene (Object pooling)
    /// </summary>
    private static void Load(int index)
    {
        ChunkManager instance = ChunkLoader.Load(GenData.Chunks[index]);
        LoadedChunkObjects[index] = instance;
    }

    /// <summary>
    /// Unload a Chunk by index from the Scene (Object pooling)
    /// </summary>
    private static void Unload(int index)
    {
        if (!LoadedChunkObjects.TryGetValue(index, out ChunkManager instance)) return;

        ChunkLoader.Unload(instance);
        LoadedChunkObjects.Remove(index);
    }
}
