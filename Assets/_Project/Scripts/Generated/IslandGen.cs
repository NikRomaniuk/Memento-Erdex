using System.Collections.Generic;
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
    // --- Data ---
    public IslandData IslandData { get; private set; }

    // --- Settings ---
    public Vector2 Pos { get; private set; }
    public bool IsXFlipped { get; private set; }
    public short SpriteOrder { get; private set; }

    // --- Childs ---
    public List<PropGen> Props { get; private set; }

    public IslandGen(IslandData islandData, Vector2 pos, bool isXFlipped, short spriteOrder)
    {
        IslandData = islandData;
        Pos = pos;
        IsXFlipped = isXFlipped;
        SpriteOrder = spriteOrder;
        Props = new List<PropGen>();
    }
}
