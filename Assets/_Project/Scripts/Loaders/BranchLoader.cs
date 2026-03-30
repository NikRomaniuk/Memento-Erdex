using UnityEngine;

public static class BranchLoader
{
    public static BlanksLibrary BlanksLibrary = TreeLoader.BlanksLibrary;

    /// <summary>
    /// Loads a <see cref="BranchManager"/> into the scene
    /// </summary>
    public static BranchManager Load(BranchGen branch)
    {
        // --- Preparations ---
        // Get a blank from the Library
        var blank = (BranchManager)BlanksLibrary.GetBlank(BlanksLibrary.BlankType.Branch);

        // --- Components ---
        Builder blankBuilder = blank.GetComponent<Builder>();

        // --- Load ---
        // Initialize blank with data & extra settings
        blankBuilder.Initialize(branch.BranchData, branch.Side, branch.Height);

        // --- Activate ---
        blank.gameObject.SetActive(true);

        return blank;
    }

    /// <summary>
    /// Unloads a <see cref="BranchManager"/> from the scene
    /// </summary>
    public static void Unload(BranchManager branch)
    {
        // --- Components ---
        Builder blankBuilder = branch.GetComponent<Builder>();

        // --- Unload ---
        // Clear all loaded data from the blank
        blankBuilder.Clear();
        branch.transform.position = Vector3.zero; // Reset position
        branch.gameObject.SetActive(false); // Deactivate

        // --- Return to Library ---
        BlanksLibrary.ReturnBlank(branch, BlanksLibrary.BlankType.Branch);
    }
}
