using UnityEngine;

public static class ChunkLoader
{
    public static BlanksLibrary BlanksLibrary = TreeLoader.BlanksLibrary;

    /// <summary>
    /// Loads a Chunk into the scene: gets a blank <see cref="ChunkManager"/>, loads all its Trunk Parts, activates it, and returns the instance.
    /// </summary>
    public static ChunkManager Load(ChunkGen chunk)
    {
        // --- Get blank ---
        var blank = (ChunkManager)BlanksLibrary.GetBlank(BlanksLibrary.BlankType.Chunk);

        // --- Components ---
        Builder blankBuilder = blank.GetComponent<Builder>();

        // --- Load Trunk Parts ---
        foreach (TrunkGen trunk in chunk.TrunkParts)
        {
            TrunkSegment loadedTrunk = TrunkLoader.Load(trunk);
            blank.LoadedTrunks.Add(loadedTrunk);
        }

        // --- Load itself ---
        // Initialize blank with data & extra settings
        blankBuilder.Initialize(chunk.ChunkData, chunk.CurrentHeight);

        // --- Activate ---
        blank.gameObject.SetActive(true);

        return blank;
    }

    /// <summary>
    /// Unloads a Chunk from the scene
    /// </summary>
    public static void Unload(ChunkManager chunkManager)
    {
        // --- Unload Trunk Parts ---
        foreach (TrunkSegment trunk in chunkManager.LoadedTrunks)
            TrunkLoader.Unload(trunk);
        chunkManager.LoadedTrunks.Clear();

        // --- Unload Itself ---
        // Reset position
        chunkManager.transform.position = Vector3.zero;

        // --- Deactivate ---
        chunkManager.gameObject.SetActive(false);

        // --- Return to Library ---
        BlanksLibrary.ReturnBlank(chunkManager, BlanksLibrary.BlankType.Chunk);
    }
}
