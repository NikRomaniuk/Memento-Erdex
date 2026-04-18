using UnityEngine;

public static class ShapeLoader
{
    public static BlanksLibrary BlanksLibrary = TreeLoader.BlanksLibrary;

    /// <summary>
    /// Loads a <see cref="ShapeManager"/> into the scene
    /// </summary>
    public static ShapeManager Load(ShapeGen shape)
    {
        // --- Preparations ---
        // Get a blank from the Library
        var blank = (ShapeManager)BlanksLibrary.GetBlank(BlanksLibrary.BlankType.Shape);

        // --- Components ---
        Builder blankBuilder = blank.GetComponent<Builder>();
        SpriteRenderer spriteRenderer = blank._spriteRenderer;

        // --- Load ---
        // Initialize blank with data & extra settings
        blankBuilder.Initialize(shape.ShapeData, shape.Side, shape.IsXFlipped);

        // Set position
        blank.transform.position = new Vector3(shape.Pos.x, shape.Pos.y, blank.transform.position.z);

        // Set sorting order
        spriteRenderer.sortingOrder = shape.SpriteOrder;
        // Set sprite color
        spriteRenderer.color = shape.SpriteColor;
        // Set outline color
        blank.OutlineView?.ApplyColor(shape.OutlineColor);

        // --- Activate ---
        blank.gameObject.SetActive(true);

        return blank;
    }

    /// <summary>
    /// Unloads a <see cref="ShapeManager"/> from the scene
    /// </summary>
    public static void Unload(ShapeManager shape)
    {
        // --- Components ---
        Builder blankBuilder = shape.GetComponent<Builder>();
        SpriteRenderer spriteRenderer = shape._spriteRenderer;

        // --- Unload ---
        // Clear all loaded data from the blank
        blankBuilder.Clear();
        shape.transform.position = Vector3.zero; // Reset position
        shape.gameObject.SetActive(false); // Deactivate
        spriteRenderer.sortingOrder = 0; // Reset sorting order
        spriteRenderer.color = Color.white; // Reset sprite color
        shape.OutlineView?.ResetColor(); // Reset outline color

        // --- Return to Library ---
        BlanksLibrary.ReturnBlank(shape, BlanksLibrary.BlankType.Shape);
    }
}
