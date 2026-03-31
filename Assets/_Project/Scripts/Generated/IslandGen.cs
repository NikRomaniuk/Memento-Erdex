using UnityEngine;

/// <summary>
/// <para> A class that holds generated data for a <see cref="IslandManager"/> from the <see cref="IslandGenerator"/> </para>
/// <para> Contains: </para>
/// <list>
///   <item> - IslandData: The ScriptableObject containing STATIC Data </item>
///   <item> - Settings: Extra DYNAMIC Data needed for loading the <see cref="IslandManager"/> into Scene </item>
/// </list>
/// </summary>
public class IslandGen
{
    // --- Island Data ---
    public IslandData IslandData { get; private set; }

    // --- Settings ---
	public Orientation Orientation { get; private set; }
    public Vector2 Pos { get; private set; }
    public bool IsXFlipped { get; private set; }

    public IslandGen(IslandData islandData, Orientation orientation, Vector2 pos, bool isXFlipped)
    {
        IslandData = islandData;
        Orientation = orientation;
        Pos = pos;
        IsXFlipped = isXFlipped;
    }
}
