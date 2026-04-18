using System.Collections.Generic;
using UnityEngine;

public class IslandGenerator : MonoBehaviour
{
    private const float POS_STEP = 0.05f;
    private const float Y_OFFSET = 0.05f;
    private const short ISLAND_SPRITE_ORDER = 100;

    [Header("Island Pool")]
    [SerializeField] private IslandData[] _islandDataPool;

    [Header("Visual Settings")]
    [SerializeField] private Color _outlineColor = Color.black;
    [SerializeField] private bool _useDefaultOutlineColor = true;

    public Color OutlineColor => _outlineColor;
    public bool UseDefaultOutlineColor => _useDefaultOutlineColor;

    // --- Components ---
    // Prop generation is chained to Island generation
    private PropGenerator _propGenerator;

    // Splitted pools for each size category
    private IslandData[] _tinyPool;
    private IslandData[] _smallPool;
    private IslandData[] _mediumPool;

    /// <summary>
    /// Cache components and split island pool by size
    /// </summary>
    private void Awake()
    {
        _propGenerator = GetComponent<PropGenerator>();
        Split();
    }

    /// <summary>
    /// Generate Islands for all Island Slots in a Branch
    /// </summary>
    public void GenerateIslands(System.Random random, BranchGen branchGen)
    {
        // --- Validations ---
        IslandSlot[] islandSlots = branchGen.BranchData.islandSlots;
        if (islandSlots == null || islandSlots.Length == 0)
        {
            Debug.Log($"Branch '{branchGen.BranchData.id}' has no Island Slots");
            return;
        }

        // Regenerate from scratch
        branchGen.Islands.Clear();

        int generatedCount = 0;

        foreach (IslandSlot slot in islandSlots)
        {
            if (TryGenerateIslandForSlot(random, branchGen, slot, out IslandGen islandGen))
            {
                branchGen.Islands.Add(islandGen);
                generatedCount++;
            }
        }

        Debug.Log($"Generated {generatedCount} Islands for Branch '{branchGen.BranchData.id}' at height {branchGen.Pos.y + Y_OFFSET}");
    }

    /// <summary>
    /// Try generate one Island for one Island Slot
    /// </summary>
    private bool TryGenerateIslandForSlot(System.Random random, BranchGen branchGen, IslandSlot slot, out IslandGen islandGen)
    {
        islandGen = null;

        IslandData selectedIslandData;
        Size selectedSize;
        Size maxAllowedSize;

        if (slot.isStatic)
        {
            // Static -> Only one possible Island Data
            selectedIslandData = slot.staticIslandData;

            // --- Validations ---
            if (selectedIslandData == null)
            {
                Debug.LogWarning($"Static Island Slot at x = {slot.xPoint} has no assigned IslandData");
                return false;
            }

            if (!CanUseIslandOnSide(selectedIslandData, branchGen.Side))
            {
                Debug.LogWarning($"Static Island '{selectedIslandData.name}' is not allowed on side {branchGen.Side}");
                return false;
            }

            // Static -> Only one possible Island Data -> Only one possible size!
            selectedSize = selectedIslandData.size;
            maxAllowedSize = selectedSize;
        }
        else
        {
            if (!TryGetRandomDynamicIslandData(random, slot, branchGen.Side, out selectedIslandData, out selectedSize, out maxAllowedSize))
            {
                Debug.LogWarning($"No suitable dynamic Island candidates for Slot at x = {slot.xPoint} on side {branchGen.Side}");
                return false;
            }
        }

        float localX = GetLocalXForSlot(random, slot, selectedSize, maxAllowedSize);
        float worldX = GetWorldXFromBranch(branchGen, localX);
        Vector2 pos = QuantizePos(new Vector2(worldX, branchGen.Pos.y + Y_OFFSET));

        bool isXFlipped = selectedIslandData.canBeXFlipped && random.Next(0, 2) == 1;
        Color resolvedOutlineColor = _useDefaultOutlineColor
            ? selectedIslandData.defaultOutlineColor
            : _outlineColor;

        islandGen = new IslandGen(selectedIslandData, pos, isXFlipped, ISLAND_SPRITE_ORDER, resolvedOutlineColor);
        return true;
    }

    /// <summary>
    /// Pick dynamic Island Data from allowed sizes and side rules
    /// </summary>
    private bool TryGetRandomDynamicIslandData(
        System.Random random,
        IslandSlot slot,
        Orientation side,
        out IslandData islandData,
        out Size selectedSize,
        out Size maxAllowedSize)
    {
        islandData = null;
        selectedSize = Size.Tiny;
        maxAllowedSize = Size.Tiny;

        List<Size> allowedSizes = GetAllowedSizes(slot);
        if (allowedSizes.Count == 0)
        {
            return false;
        }

        maxAllowedSize = GetMaxSize(allowedSizes);

        List<Size> viableSizes = new List<Size>();
        foreach (Size size in allowedSizes)
        {
            if (HasSideValidCandidate(size, side))
            {
                viableSizes.Add(size);
            }
        }

        if (viableSizes.Count == 0)
        {
            return false;
        }

        selectedSize = viableSizes[random.Next(0, viableSizes.Count)];
        IslandData[] pool = GetPoolBySize(selectedSize);
        if (pool == null || pool.Length == 0)
        {
            return false;
        }

        List<IslandData> sideValidCandidates = new List<IslandData>();
        foreach (IslandData candidate in pool)
        {
            if (candidate == null)
            {
                continue;
            }

            if (CanUseIslandOnSide(candidate, side))
            {
                sideValidCandidates.Add(candidate);
            }
        }

        if (sideValidCandidates.Count == 0)
        {
            return false;
        }

        islandData = sideValidCandidates[random.Next(0, sideValidCandidates.Count)];
        return true;
    }

    /// <summary>
    /// Return allowed sizes from slot flags
    /// </summary>
    private List<Size> GetAllowedSizes(IslandSlot slot)
    {
        List<Size> allowedSizes = new List<Size>();

        if (slot.allowTiny)
            allowedSizes.Add(Size.Tiny);

        if (slot.allowSmall)
            allowedSizes.Add(Size.Small);

        if (slot.allowMedium)
            allowedSizes.Add(Size.Medium);

        return allowedSizes;
    }

    /// <summary>
    /// Return max size from allowed sizes by width value
    /// </summary>
    private Size GetMaxSize(List<Size> sizes)
    {
        Size maxSize = Size.Tiny;
        for (int i = 0; i < sizes.Count; i++)
        {
            if (Constants.SIZE_VALUES[sizes[i]] > Constants.SIZE_VALUES[maxSize])
            {
                maxSize = sizes[i];
            }
        }

        return maxSize;
    }

    /// <summary>
    /// Check if size pool has at least one side valid Island
    /// </summary>
    private bool HasSideValidCandidate(Size size, Orientation side)
    {
        IslandData[] pool = GetPoolBySize(size);
        if (pool == null || pool.Length == 0)
        {
            return false;
        }

        foreach (IslandData islandData in pool)
        {
            if (islandData != null && CanUseIslandOnSide(islandData, side))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Return split pool by size
    /// </summary>
    private IslandData[] GetPoolBySize(Size size)
    {
        switch (size)
        {
            case Size.Tiny:
                return _tinyPool;

            case Size.Small:
                return _smallPool;

            case Size.Medium:
                return _mediumPool;

            default:
                return null;
        }
    }

    /// <summary>
    /// Check if Island Data is allowed on Branch side
    /// </summary>
    private bool CanUseIslandOnSide(IslandData data, Orientation side)
    {
        switch (side)
        {
            case Orientation.Left:
                return data.allowLeft;

            case Orientation.Right:
                return data.allowRight;

            case Orientation.Middle:
                return data.allowMiddle;

            default:
                return false;
        }
    }

    /// <summary>
    /// Compute local X with random offset for smaller size
    /// </summary>
    private float GetLocalXForSlot(System.Random random, IslandSlot slot, Size selectedSize, Size maxAllowedSize)
    {
        if (slot.isStatic || selectedSize == maxAllowedSize)
        {
            return slot.xPoint;
        }

        float selectedWidth = Constants.SIZE_VALUES[selectedSize];
        float maxAllowedWidth = Constants.SIZE_VALUES[maxAllowedSize];
        float maxOffset = Mathf.Max(0f, (maxAllowedWidth - selectedWidth) / 2f);

        float offset = (float)random.NextDouble() * maxOffset;
        int sign = random.Next(0, 2) == 0 ? -1 : 1;
        return slot.xPoint + (offset * sign);
    }

    /// <summary>
    /// Convert branch local X to world X by Branch side
    /// </summary>
    private float GetWorldXFromBranch(BranchGen branchGen, float localX)
    {
        switch (branchGen.Side)
        {
            case Orientation.Left:
                return branchGen.Pos.x - localX;

            case Orientation.Right:
                return branchGen.Pos.x + localX;

            case Orientation.Middle:
            default:
                return branchGen.Pos.x + localX;
        }
    }

    // --- Helper Methods ---

    /// <summary>
    /// Split source island pool into tiny small and medium pools
    /// </summary>
    private void Split()
    {
        if (_islandDataPool == null || _islandDataPool.Length == 0)
        {
            _tinyPool = new IslandData[0];
            _smallPool = new IslandData[0];
            _mediumPool = new IslandData[0];
            return;
        }

        List<IslandData> tinyPool = new List<IslandData>();
        List<IslandData> smallPool = new List<IslandData>();
        List<IslandData> mediumPool = new List<IslandData>();

        foreach (IslandData islandData in _islandDataPool)
        {
            if (islandData == null) continue;

            switch (islandData.size)
            {
                case Size.Tiny:
                    tinyPool.Add(islandData);
                    break;

                case Size.Small:
                    smallPool.Add(islandData);
                    break;

                case Size.Medium:
                    mediumPool.Add(islandData);
                    break;
            }
        }

        _tinyPool = tinyPool.ToArray();
        _smallPool = smallPool.ToArray();
        _mediumPool = mediumPool.ToArray();
    }

    /// <summary>
    /// Quantize world position to configured Step
    /// </summary>
    private Vector2 QuantizePos(Vector2 pos)
    {
        return new Vector2(QuantizeStep(pos.x), QuantizeStep(pos.y));
    }

    /// <summary>
    /// Quantize scalar value to configured Step
    /// </summary>
    private float QuantizeStep(float value)
    {
        return Mathf.Round(value / POS_STEP) * POS_STEP;
    }
}
