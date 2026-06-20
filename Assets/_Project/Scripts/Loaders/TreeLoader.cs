using System.Collections.Generic;
using UnityEngine;

public static class TreeLoader
{
    // --- References ---
    public static TreeGen GenData; // Generated tree data
    public static BlanksLibrary BlanksLibrary; // Blanks of all kinds to build the Tree with!

    // --- State ---
    // Indices of currently loaded chunks
    public static List<int> LoadedChunks = new List<int>(); 
    // Map: Сhunk index -> ChunkManager
    public static Dictionary<int, ChunkManager> LoadedChunkObjects = new Dictionary<int, ChunkManager>();

    /// <summary>
    /// Binds the active <see cref="BlanksLibrary"/> for all Loaders
    /// </summary>
    public static void SetBlanksLibrary(BlanksLibrary library)
    {
        BlanksLibrary = library;

        // Keep Loaders in sync with the Blank Library
        ChunkLoader.BlanksLibrary = library;
        TrunkLoader.BlanksLibrary = library;
        BranchLoader.BlanksLibrary = library;
        ShapeLoader.BlanksLibrary = library;
        IslandLoader.BlanksLibrary = library;
        ClutterLoader.BlanksLibrary = library;
    }

    /// <summary>
    /// Clears runtime static data between Scene loads
    /// </summary>
    public static void ResetRuntimeState()
    {
        GenData = null;
        LoadedChunks.Clear();
        LoadedChunkObjects.Clear();
    }

    /// <summary>
    /// Ensures the given set of Сhunk indices is loaded.
    /// Loads missing Chunks and unloads Chunks outside the set
    /// </summary>
    public static void KeepLoaded(HashSet<int> desired)
    {
        if (BlanksLibrary == null || GenData == null || GenData.Chunks.Count == 0) return;

        // --- Unload Chunks outside desired set ---
        for (int i = LoadedChunks.Count - 1; i >= 0; i--)
        {
            int idx = LoadedChunks[i];
            if (!desired.Contains(idx))
            {
                Unload(idx);
                LoadedChunks.RemoveAt(i);
            }
        }

        // --- Load new Chunks inside desired set ---
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
