using UnityEngine;

/// <summary>
/// <para> A class that holds generated data for a <see cref="ClutterManager"/> from the <see cref="ClutterGenerator"/> </para>
/// <para> Contains: </para>
/// <list>
///   <item> - ClutterData: The ScriptableObject containing STATIC Data </item>
///   <item> - Settings: Extra DYNAMIC Data needed for loading the <see cref="ClutterManager"/> into Scene </item>
/// </list>
/// </summary>
public class ClutterGen
{
    // --- ClutterData ---
    public ClutterData ClutterData { get; private set; }

    // --- Settings ---
    public Vector2 Pos { get; private set; }
    public bool IsXFlipped { get; private set; }
    public bool IsYFlipped { get; private set; }
    public short SpriteOrder { get; private set; }
    public Color OutlineColor { get; private set; }
    public int SortingLayerId { get; private set; }

    public ClutterGen(
        ClutterData clutterData,
        Vector2 pos,
        bool isXFlipped,
        bool isYFlipped,
        short spriteOrder,
        Color outlineColor,
        int sortingLayerId)
    {
        ClutterData = clutterData;
        Pos = pos;
        IsXFlipped = isXFlipped;
        IsYFlipped = isYFlipped;
        SpriteOrder = spriteOrder;
        OutlineColor = outlineColor;
        SortingLayerId = sortingLayerId;
    }
}