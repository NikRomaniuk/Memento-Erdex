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
        SpriteRenderer shapeRenderer = blank._shapeRenderer;
        SpriteRenderer borderRenderer = blank._borderRenderer;

        // --- Load ---
        // Initialize blank with data & extra settings
        blankBuilder.Initialize(shape.ShapeData, shape.Side, shape.IsXFlipped);

        // --- Load Clutter ---
        if (shape.ClutterList != null)
        {
            foreach (ClutterGen clutter in shape.ClutterList)
            {
                ClutterManager loadedClutter = ClutterLoader.Load(clutter);
                blank.LoadedClutter.Add(loadedClutter);
            }
        }

        // Set position
        blank.transform.position = new Vector3(shape.Pos.x, shape.Pos.y, blank.transform.position.z);

        // Set sorting order
        shapeRenderer.sortingOrder = shape.SpriteOrder;
        borderRenderer.sortingOrder = shape.SpriteOrder;
        // Set sprite color
        shapeRenderer.color = shape.SpriteColor;
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
        // --- Unload Clutter ---
        foreach (ClutterManager clutter in shape.LoadedClutter)
            ClutterLoader.Unload(clutter);
        shape.LoadedClutter.Clear();

        // --- Components ---
        Builder blankBuilder = shape.GetComponent<Builder>();
        SpriteRenderer shapeRenderer = shape._shapeRenderer;
        SpriteRenderer borderRenderer = shape._borderRenderer;
        // --- Unload ---
        // Clear all loaded data from the blank
        blankBuilder.Clear();
        shape.transform.position = Vector3.zero; // Reset position
        shape.gameObject.SetActive(false); // Deactivate
        shapeRenderer.sortingOrder = 0; // Reset sorting order
        borderRenderer.sortingOrder = 0; // Reset sorting order
        shapeRenderer.color = Color.white; // Reset sprite color
        shape.OutlineView?.ResetColor(); // Reset outline color

        // --- Return to Library ---
        BlanksLibrary.ReturnBlank(shape, BlanksLibrary.BlankType.Shape);
    }
}
