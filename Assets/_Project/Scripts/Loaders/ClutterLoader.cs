using UnityEngine;

public static class ClutterLoader
{
    public static BlanksLibrary BlanksLibrary = TreeLoader.BlanksLibrary;

    /// <summary>
    /// Loads a <see cref="ClutterManager"/> into the scene
    /// </summary>
    public static ClutterManager Load(ClutterGen clutter)
    {
        // --- Preparations ---
        // Get a blank from the Library
        var blank = (ClutterManager)BlanksLibrary.GetBlank(BlanksLibrary.BlankType.Clutter);

        // --- Components ---
        Builder blankBuilder = blank.GetComponent<Builder>();
        SpriteRenderer spriteRenderer = blank._spriteRenderer;
        SpriteRenderer outlineRenderer = blank._outlineRenderer;

        // --- Load Itself ---
        // Initialize blank with data & extra settings
        blankBuilder.Initialize(clutter.ClutterData, clutter.IsXFlipped, clutter.IsYFlipped);

        // Set position
        blank.transform.position = new Vector3(clutter.Pos.x, clutter.Pos.y, blank.transform.position.z);
        // Set sorting order
        spriteRenderer.sortingOrder = clutter.SpriteOrder;
        // Set sorting layer
        spriteRenderer.sortingLayerID = clutter.SortingLayerId;
        //outlineRenderer.sortingLayerID = clutter.SortingLayerId; // Keep Outline on the same layer
        // Set outline color
        blank.OutlineView?.ApplyColor(clutter.OutlineColor);

        // --- Activate ---
        blank.gameObject.SetActive(true);

        return blank;
    }

    /// <summary>
    /// Unloads a <see cref="ClutterManager"/> from the scene
    /// </summary>
    public static void Unload(ClutterManager clutter)
    {
        // --- Components ---
        Builder blankBuilder = clutter.GetComponent<Builder>();
        SpriteRenderer spriteRenderer = clutter._spriteRenderer;

        // --- Unload Itself ---
        // Clear all loaded data from the blank
        blankBuilder.Clear();
        clutter.transform.position = Vector3.zero; // Reset position
        spriteRenderer.sortingOrder = 0; // Reset sorting order
        clutter.OutlineView?.ResetColor(); // Reset outline color
        clutter.gameObject.SetActive(false); // Deactivate
        
        // --- Return to Library ---
        BlanksLibrary.ReturnBlank(clutter, BlanksLibrary.BlankType.Clutter);
    }
}
