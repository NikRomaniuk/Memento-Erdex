using UnityEngine;

public static class IslandLoader
{
	public static BlanksLibrary BlanksLibrary = TreeLoader.BlanksLibrary;

	/// <summary>
	/// Loads an <see cref="IslandManager"/> into the scene
	/// </summary>
	public static IslandManager Load(IslandGen island)
	{
		// --- Preparations ---
		// Get a blank from the Library
		var blank = (IslandManager)BlanksLibrary.GetBlank(BlanksLibrary.BlankType.Island);

		// --- Components ---
		Builder blankBuilder = blank.GetComponent<Builder>();
		SpriteRenderer spriteRenderer = blank._spriteRenderer;

		// --- Load Itself ---
		// Initialize blank with data & extra settings
		blankBuilder.Initialize(island.IslandData, island.IsXFlipped);

		// Set position
		blank.transform.position = new Vector3(island.Pos.x, island.Pos.y, blank.transform.position.z);
		// Set sorting order
		spriteRenderer.sortingOrder = island.SpriteOrder;
		// Set outline color
		blank.OutlineView?.ApplyColor(island.OutlineColor);

		// --- Activate ---
		blank.gameObject.SetActive(true);

		return blank;
	}

	/// <summary>
	/// Unloads an <see cref="IslandManager"/> from the scene
	/// </summary>
	public static void Unload(IslandManager island)
	{
		// --- Components ---
		Builder blankBuilder = island.GetComponent<Builder>();
		SpriteRenderer spriteRenderer = island._spriteRenderer;

		// --- Unload Itself ---
		// Clear all loaded data from the blank
		blankBuilder.Clear();
		island.transform.position = Vector3.zero; // Reset position
        spriteRenderer.sortingOrder = 0; // Reset sorting order
		island.OutlineView?.ResetColor(); // Reset outline color
		island.gameObject.SetActive(false); // Deactivate
		

		// --- Return to Library ---
		BlanksLibrary.ReturnBlank(island, BlanksLibrary.BlankType.Island);
	}
}
