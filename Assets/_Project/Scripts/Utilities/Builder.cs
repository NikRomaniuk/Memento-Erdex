using UnityEngine;

public class Builder : MonoBehaviour
{
    // --- Cached Component ---
    private IBuildable _buildable;

    /// <summary>
    /// Loads the <see cref="IBuildable"/> component from this GameObject
    /// </summary>
    public bool PrepareToBuild()
    {
        _buildable = GetComponent<IBuildable>();
        if (_buildable == null)
        {
            Debug.LogError("No IBuildable component found on this GameObject!");
            return false;
        }
        Debug.Log($"References loaded from {_buildable.GetType().Name}");
        return true;
    }

    /// <summary>
    /// Initialize TrunkSegment with the given data
    /// </summary>
    public void Initialize(TrunkData data, Side side, bool isYFlipped)
    {
        if (!PrepareToBuild()) return;

        // --- Validations ---
        bool isSideValid = data.avaliableSide == TrunkAvaliableSide.Both ||
                           (data.avaliableSide == TrunkAvaliableSide.Left && side == Side.Left) ||
                           (data.avaliableSide == TrunkAvaliableSide.Right && side == Side.Right);

        if (!isSideValid)
        {
            Debug.LogWarning($"Trunk '{data.name}' (ID: {data.id}) cannot be placed on {side} side. Available side: {data.avaliableSide}");
            return;
        }

        if (isYFlipped && !data.canBeYFlipped)
        {
            Debug.LogWarning($"Trunk '{data.name}' (ID: {data.id}) cannot be flipped vertically");
            return;
        }

        ((TrunkSegment)_buildable).SetData(data, side, isYFlipped);

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(gameObject);
#endif
    }

    /// <summary>
    /// Initialize ChunkManager with the given data
    /// </summary>
    public void Initialize(ChunkData data, float currentHeight)
    {
        if (!PrepareToBuild()) return;

        if (data == null)
        {
            Debug.LogWarning("Cannot initialize with null ChunkData");
            return;
        }

        ((ChunkManager)_buildable).SetData(data, currentHeight);

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(gameObject);
#endif
    }

    /// <summary>
    /// Initialize BranchManager with the given data
    /// </summary>
    public void Initialize(BranchData data, Orientation orient, float currentHeight)
    {
        if (!PrepareToBuild()) return;

        if (data == null)
        {
            Debug.LogWarning("Cannot initialize with null BranchData");
            return;
        }

        bool isOrientationValid;
        if (data.avaliableSide == BranchData.AvailableSide.Both)
        {
            isOrientationValid = orient == Orientation.Left || orient == Orientation.Right;
        }
        else
        {
            isOrientationValid = (data.avaliableSide == BranchData.AvailableSide.Left && orient == Orientation.Left) ||
                                 (data.avaliableSide == BranchData.AvailableSide.Right && orient == Orientation.Right) ||
                                 (data.avaliableSide == BranchData.AvailableSide.Middle && orient == Orientation.Middle);
        }

        if (!isOrientationValid)
        {
            Debug.LogWarning($"Branch '{data.name}' (ID: {data.id}) cannot be placed with {orient} orientation. Available side: {data.avaliableSide}");
            return;
        }

        ((BranchManager)_buildable).SetData(data, orient, currentHeight);

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(gameObject);
#endif
    }

    /// <summary>
    /// Initialize ShapeManager with the given data
    /// </summary>
    public void Initialize(ShapeData data, Side side, bool isXFlipped)
    {
        if (!PrepareToBuild()) return;

        if (data == null)
        {
            Debug.LogWarning("Cannot initialize with null ShapeData");
            return;
        }

        if (isXFlipped && !data.canBeXFlipped)
        {
            Debug.LogWarning($"Shape '{data.name}' (ID: {data.id}) cannot be flipped horizontally");
            return;
        }

        ((ShapeManager)_buildable).SetData(data, side, isXFlipped);

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(gameObject);
#endif
    }

    /// <summary>
    /// Initialize IslandManager with the given data
    /// </summary>
    public void Initialize(IslandData data, bool isXFlipped)
    {
        if (!PrepareToBuild()) return;
        if (data == null) { Debug.LogWarning("Cannot initialize with null IslandData"); return; }

        if (isXFlipped && !data.canBeXFlipped)
        {
            Debug.LogWarning($"Island '{data.name}' (ID: {data.id}) cannot be flipped horizontally");
            return;
        }

        ((IslandManager)_buildable).SetData(data, isXFlipped);

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(gameObject);
#endif
    }

    /// <summary>
    /// Initialize ClutterManager with the given data
    /// </summary>
    public void Initialize(ClutterData data, bool isXFlipped, bool isYFlipped)
    {
        if (!PrepareToBuild()) return;

        if (data == null)
        {
            Debug.LogWarning("Cannot initialize with null ClutterData");
            return;
        }

        ((ClutterManager)_buildable).SetData(data, isXFlipped, isYFlipped);

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(gameObject);
#endif
    }

    /// <summary>
    /// Clears the component and resets to initial state
    /// </summary>
    public void Clear()
    {
        if (!PrepareToBuild())
        {
            Debug.LogError("Failed to prepare components. Aborting Clear");
            return;
        }

        _buildable.Clear();
    }
}
