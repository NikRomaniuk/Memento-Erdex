using UnityEngine;

public class Builder : MonoBehaviour
{
    [Header("General Data")]
    [SerializeField] private BlanksLibrary.BlankType _blankType;
    [SerializeField] private ScriptableObject _dataToBuild;

    // --- Trunk Data ---
    [SerializeField] private Side _side = Side.Right;
    [SerializeField] private bool _isYFlipped = false;

    // --- Chunk Data ---
    [SerializeField] private float _currentHeight = 0f;

    // --- Branch Data ---
    [SerializeField] private BranchOrientation _branchOrientation = BranchOrientation.Right;

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
                Initialize(branchData, _branchOrientation, _currentHeight);
                Debug.Log($"Branch successfully built from <b>{branchData.name}</b> (ID: {branchData.id})");
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
    public void Initialize(BranchData data, BranchOrientation orient, float currentHeight)
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
            isOrientationValid = orient == BranchOrientation.Left || orient == BranchOrientation.Right;
        }
        else
        {
            isOrientationValid = (data.avaliableSide == BranchData.AvailableSide.Left && orient == BranchOrientation.Left) ||
                                 (data.avaliableSide == BranchData.AvailableSide.Right && orient == BranchOrientation.Right) ||
                                 (data.avaliableSide == BranchData.AvailableSide.Middle && orient == BranchOrientation.Middle);
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
