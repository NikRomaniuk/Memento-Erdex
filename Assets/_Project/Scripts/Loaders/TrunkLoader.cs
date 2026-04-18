using UnityEngine;

public static class TrunkLoader
{
    public static BlanksLibrary BlanksLibrary = TreeLoader.BlanksLibrary;

    /// <summary>
    /// Loads a Trunk Part into the scene
    /// </summary>
    public static TrunkSegment Load(TrunkGen trunkGen)
    {
        // --- Preparations ---
        // Get a blank from the Library
        var blank = (TrunkSegment)BlanksLibrary.GetBlank(BlanksLibrary.BlankType.Trunk);

        // --- Components ---
        Builder blankBuilder = blank.GetComponent<Builder>();
        SpriteRenderer spriteRenderer = blank._spriteRenderer;
        OutlineView outlineView = blank.OutlineView;

        // --- Load ---
        // Initialize blank with data & extra settings
        blankBuilder.Initialize(trunkGen.TrunkData, trunkGen.Side, trunkGen.IsYFlipped);

        // Set position
        blank.transform.position = new Vector3(0, trunkGen.Height, 0);
        // Set sorting order
        spriteRenderer.sortingOrder = trunkGen.SpriteOrder;
        // Set sprite color
        spriteRenderer.color = trunkGen.SpriteColor;
        // Set outline color
        outlineView?.ApplyColor(trunkGen.OutlineColor);

        // --- Activate ---
        blank.gameObject.SetActive(true);

        return blank;
    }

    /// <summary>
    /// Unloads a Trunk Part from the scene
    /// </summary>
    public static void Unload(TrunkSegment trunkPart)
    {
        // --- Components ---
        Builder blankBuilder = trunkPart.GetComponent<Builder>();
        SpriteRenderer spriteRenderer = trunkPart._spriteRenderer;
        OutlineView outlineView = trunkPart.OutlineView;

        // --- Unload ---
        // Clear all loaded data from the blank
        blankBuilder.Clear();

        // Reset position
        trunkPart.transform.position = Vector3.zero;
        // Reset sorting order
        spriteRenderer.sortingOrder = 0;
        // Reset sprite color
        spriteRenderer.color = Color.white;
        // Reset outline color
        outlineView?.ResetColor();

        // --- Deactivate ---
        trunkPart.gameObject.SetActive(false);

        // --- Return to Library ---
        BlanksLibrary.ReturnBlank(trunkPart, BlanksLibrary.BlankType.Trunk);
    }
}
