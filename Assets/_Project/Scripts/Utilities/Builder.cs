using UnityEngine;

public class Builder : MonoBehaviour
{
    [Header("General Data")]
    [SerializeField] private BlanksLibrary.BlankType _blankType;
    [SerializeField] private ScriptableObject _dataToBuild;

    // --- Trunk Data ---
    [SerializeField] private Side _side = Side.Right; // Also used for Shapes
    [SerializeField] private bool _isYFlipped = false;

    // --- Chunk Data ---
    [SerializeField] private float _currentHeight = 0f;

    // --- Branch Data ---
    [SerializeField] private Orientation _orientation = Orientation.Right; // Also used for Islands

    // --- Shape Data ---
    [SerializeField] private bool _isXFlipped = false;

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
            Debug.LogError($"No IBuildable component found on this GameObject! Expected a {_blankType} component");
            return false;
        }
        Debug.Log($"References loaded from {_buildable.GetType().Name}");
        return true;
    }

    /// <summary>
    /// Builds this object from the assigned <see cref="_dataToBuild"/> ScriptableObject
    /// </summary>
    public void BuildFromScriptableObject()
    {
        // --- Preparations ---
        if (!PrepareToBuild())
        {
            Debug.LogError("Failed to prepare components. Aborting BuildFromScriptableObject");
            return;
        }

        if (_dataToBuild == null)
        {
            Debug.LogError("No data assigned! Please assign a ScriptableObject to build from");
            return;
        }

        switch (_blankType)
        {
            case BlanksLibrary.BlankType.Trunk:
                var trunkData = _dataToBuild as TrunkData;
                if (trunkData == null)
                {
                    Debug.LogError("_dataToBuild is not a TrunkData! Please assign a TrunkData ScriptableObject");
                    return;
                }
                // Initialize TrunkSegment with data & extra settings
                Initialize(trunkData, _side, _isYFlipped);
                Debug.Log($"TrunkPart successfully built from <b>{trunkData.name}</b> (ID: {trunkData.id})");
                break;

            case BlanksLibrary.BlankType.Chunk:
                var chunkData = _dataToBuild as ChunkData;
                if (chunkData == null)
                {
                    Debug.LogError("_dataToBuild is not a ChunkData! Please assign a ChunkData ScriptableObject");
                    return;
                }
                // Initialize ChunkManager with data
                Initialize(chunkData, _currentHeight);
                Debug.Log($"Chunk successfully built from <b>{chunkData.name}</b>");
                break;

            case BlanksLibrary.BlankType.Branch:
                var branchData = _dataToBuild as BranchData;
                if (branchData == null)
                {
                    Debug.LogError("_dataToBuild is not a BranchData! Please assign a BranchData ScriptableObject");
                    return;
                }
                // Initialize BranchManager with data & extra settings
                Initialize(branchData, _orientation, _currentHeight);
                Debug.Log($"Branch successfully built from <b>{branchData.name}</b> (ID: {branchData.id})");
                break;

            case BlanksLibrary.BlankType.Shape:
                var shapeData = _dataToBuild as ShapeData;
                if (shapeData == null)
                {
                    Debug.LogError("_dataToBuild is not a ShapeData! Please assign a ShapeData ScriptableObject");
                    return;
                }
                // Initialize ShapeManager with data & extra settings
                Initialize(shapeData, _side, _isXFlipped);
                Debug.Log($"Shape successfully built from <b>{shapeData.name}</b> (ID: {shapeData.id})");
                break;

            case BlanksLibrary.BlankType.Island:
                var islandData = _dataToBuild as IslandData;
                if (islandData == null)
                {
                    Debug.LogError("_dataToBuild is not an IslandData! Please assign an IslandData ScriptableObject");
                    return;
                }
                // Initialize IslandManager with data & extra settings
                Initialize(islandData, _isXFlipped);
                Debug.Log($"Island successfully built from <b>{islandData.name}</b> (ID: {islandData.id})");
                break;

            default:
                Debug.LogError($"Unknown BlankType: {_blankType}. Aborting BuildFromScriptableObject");
                break;
        }
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

    public BlanksLibrary.BlankType BlankType => _blankType;
    public ScriptableObject DataToBuild => _dataToBuild;
}
