using System.Collections.Generic;
using UnityEngine;

public class ClutterGenerator : MonoBehaviour
{
    private const string SORTING_LAYER_CLUTTER = "Clutter";
    private const string SORTING_LAYER_TRUNK_CLUTTER = "Trunk Clutter";
    private const string SORTING_LAYER_SHAPE_CLUTTER = "Shape Clutter";
    private const string SORTING_LAYER_TREE_OUTLINE = "Tree Outline";

    [Header("Clutter Database")]
    [SerializeField] private DataDatabase _clutterDb;

    [Header("Visual Settings")]
    [SerializeField] private Color _outlineColor = Color.black;
    [SerializeField] private bool _useDefaultOutlineColor = true;

    public Color OutlineColor => _outlineColor;
    public bool UseDefaultOutlineColor => _useDefaultOutlineColor;
    
    // --- Cache ---
    // Splitted DBs for each size category
    private readonly Dictionary<Size, List<ClutterData>> _clutterBySize = new Dictionary<Size, List<ClutterData>>();

    private int _sortingLayerIdClutter;
    private int _sortingLayerIdTrunkClutter;
    private int _sortingLayerIdShapeClutter;
    private int _sortingLayerIdTreeOutline;

    /// <summary>
    /// Cache and split Clutter DB by size
    /// </summary>
    private void Awake()
    {
        RebuildCache();
    }

    /// <summary>
    /// Generate Clutter for all Clutter Slots in a Trunk
    /// </summary>
    public void GenerateTrunkClutter(System.Random random, TrunkGen trunkGen)
    {
        // --- Validations ---
        if (trunkGen == null || trunkGen.TrunkData == null) { return; }

        ClutterSlot[] slots = trunkGen.TrunkData.clutterSlots;
        if (slots == null || slots.Length == 0) { return; }

        float trunkHeight = trunkGen.TrunkData.Height;
        int sideFlipX = trunkGen.Side == Side.Left ? -1 : 1;
        Vector2 origin = new Vector2(0f, trunkGen.Height);
        bool baseClutterFlipX = trunkGen.Side == Side.Left;

        GenerateClutter(
            random,
            slots,
            trunkGen.ClutterList,
            origin,
            false,
            0f,
            sideFlipX,
            trunkGen.IsYFlipped,
            trunkHeight,
            baseClutterFlipX,
            _sortingLayerIdTrunkClutter);
    }

    /// <summary>
    /// Generate Clutter for all Clutter Slots in a Shape
    /// </summary>
    public void GenerateShapeClutter(System.Random random, ShapeGen shapeGen)
    {
        // --- Validations ---
        if (shapeGen == null || shapeGen.ShapeData == null) { return; }

        ClutterSlot[] slots = shapeGen.ShapeData.clutterSlots;
        if (slots == null || slots.Length == 0) { return; }

        int sideFlipX = shapeGen.Side == Side.Left ? -1 : 1;
        bool baseClutterFlipX = (shapeGen.Side == Side.Left) ^ shapeGen.IsXFlipped;
        Vector2 origin = shapeGen.Pos;
        float shapeLength = shapeGen.ShapeData.Length;

        GenerateClutter(
            random,
            slots,
            shapeGen.ClutterList,
            origin,
            shapeGen.IsXFlipped,
            shapeLength,
            sideFlipX,
            false,
            0f,
            baseClutterFlipX,
            _sortingLayerIdShapeClutter);
    }

    private void GenerateClutter(
        System.Random random,
        ClutterSlot[] slots,
        List<ClutterGen> targetList,
        Vector2 origin,
        bool applyXFlip,
        float xFlipLength,
        int finalFlipX,
        bool applyYFlip,
        float yFlipHeight,
        bool baseClutterFlipX,
        int sortingLayerId)
    {
        if (slots == null || slots.Length == 0) { return; }
        if (targetList == null) { return; }

        if (_clutterDb == null)
        {
            Debug.LogWarning("Clutter_DB is not assigned!");
            return;
        }

        RebuildCache(); // Split Clutter DB by size
        targetList.Clear(); // Regenerate from scratch

        for (int i = 0; i < slots.Length; i++)
        {
            ClutterSlot slot = slots[i];

            if (!TryResolveClutterData(random, slot, out ClutterData data))
            {
                continue;
            }

            Vector2 localPos = slot.pos;
            if (applyXFlip)
            {
                localPos.x = xFlipLength - localPos.x;
            }
            localPos.x *= finalFlipX;
            if (applyYFlip)
            {
                localPos.y = yFlipHeight - localPos.y;
            }

            Vector2 worldPos = new Vector2(localPos.x + origin.x, localPos.y + origin.y);

            bool isXFlipped = baseClutterFlipX;
            if (data.canBeXFlipped && random.Next(0, 2) == 1)
            {
                isXFlipped = !isXFlipped;
            }

            bool isYFlipped = data.canBeYFlipped && random.Next(0, 2) == 1;
            short spriteOrder = ComputeSpriteOrder(worldPos.x, worldPos.y);
            Color resolvedOutlineColor = _useDefaultOutlineColor
                ? data.defaultOutlineColor
                : _outlineColor;

            targetList.Add(new ClutterGen(
                data,
                worldPos,
                isXFlipped,
                isYFlipped,
                spriteOrder,
                resolvedOutlineColor,
                sortingLayerId));
        }
    }

    private bool TryResolveClutterData(System.Random random, ClutterSlot slot, out ClutterData data)
    {
        data = null;

        // Static -> Use static ClutterData
        if (slot.isStatic && slot.staticClutterData != null)
        {
            data = slot.staticClutterData;
            return true;
        }

        // Dynamic -> Try to find a random ClutterData matching the size
        if (!TryGetRandomClutterDataBySize(random, slot.size, out data))
        {
            Debug.LogWarning($"No dynamic ClutterData candidates for size {slot.size}. Skipping");
            return false;
        }

        return true;
    }

    private bool TryGetRandomClutterDataBySize(System.Random random, Size size, out ClutterData data)
    {
        data = null;

        if (!_clutterBySize.TryGetValue(size, out List<ClutterData> pool) || pool.Count == 0)
        {
            return false;
        }

        int maxAttempts = pool.Count * 2;
        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            int index = random.Next(0, pool.Count);
            ClutterData candidate = pool[index];
            if (candidate != null)
            {
                data = candidate;
                return true;
            }
        }

        return false;
    }

    private short ComputeSpriteOrder(float xPos, float yPos)
    {
        int yPart = Mathf.Abs(Mathf.RoundToInt(yPos * 20)) % 100;
        int xPart = Mathf.Abs(Mathf.RoundToInt(xPos * 20)) % 100;
        int order = (yPart * 100) + xPart;
        return (short)order;
    }

    private void RebuildCache()
    {
        _clutterBySize.Clear();

        _sortingLayerIdClutter = SortingLayer.NameToID(SORTING_LAYER_CLUTTER);
        _sortingLayerIdTrunkClutter = SortingLayer.NameToID(SORTING_LAYER_TRUNK_CLUTTER);
        _sortingLayerIdShapeClutter = SortingLayer.NameToID(SORTING_LAYER_SHAPE_CLUTTER);
        _sortingLayerIdTreeOutline = SortingLayer.NameToID(SORTING_LAYER_TREE_OUTLINE);

        if (_clutterDb == null) { return; }

        foreach (ClutterData data in _clutterDb.GetData<ClutterData>())
        {
            if (data == null) { continue; }

            if (!_clutterBySize.TryGetValue(data.size, out List<ClutterData> pool))
            {
                pool = new List<ClutterData>();
                _clutterBySize.Add(data.size, pool);
            }

            pool.Add(data);
        }
    }
}
