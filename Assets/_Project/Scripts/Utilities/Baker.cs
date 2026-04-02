using UnityEngine;
using System.Linq;

public class Baker : MonoBehaviour
{
    [Header("General Data")]
    [SerializeField] private BlanksLibrary.BlankType _blankType;
    [SerializeField] private string _id;
    [SerializeField] private ScriptableObject _dataToEdit;

    // --- Trunk Data ---
    [SerializeField] private TrunkAvaliableSide _avaliableSide = TrunkAvaliableSide.Both;
    [SerializeField] private bool _canBeFlippedVertically = true;

    // --- Branch Data ---
    [SerializeField] private BranchData.AvailableSide _branchAvaliableSide = BranchData.AvailableSide.Both;

    // --- Shape Data ---
    [SerializeField] private ShapeData.Type _shapeType = ShapeData.Type.Base;
    [SerializeField] private bool _canBeFlippedHorizontally = true;

    // --- Island Data ---
    [SerializeField] private bool _islandCanBeXFlipped = true;
    [SerializeField] private bool _islandAllowLeft = true;
    [SerializeField] private bool _islandAllowRight = true;
    [SerializeField] private bool _islandAllowMiddle = true;

    // --- Cached Component ---
    private IBakeable _bakeable;

    /// <summary>
    /// Loads the <see cref="IBakeable"/> component from this GameObject
    /// </summary>
    public bool PrepareToBake()
    {
        _bakeable = GetComponent<IBakeable>();
        if (_bakeable == null)
        {
            Debug.LogError($"No IBakeable component found on this GameObject! Expected a {_blankType} component");
            return false;
        }
        Debug.Log($"References loaded from {_bakeable.GetType().Name}");
        return true;
    }

    /// <summary>
    /// Validates the ID field
    /// </summary>
    private bool ValidateId()
    {
        if (string.IsNullOrWhiteSpace(_id))
        {
            Debug.LogError("ID cannot be empty! Please provide a valid ID");
            return false;
        }

#if UNITY_EDITOR
        switch (_blankType)
        {
            case BlanksLibrary.BlankType.Trunk:
                return ValidateUniqueIdForType<TrunkData>(_dataToEdit);

            case BlanksLibrary.BlankType.Chunk:
                return ValidateUniqueIdForType<ChunkData>(_dataToEdit);

            case BlanksLibrary.BlankType.Branch:
                return ValidateUniqueIdForType<BranchData>(_dataToEdit);

            case BlanksLibrary.BlankType.Shape:
                return ValidateUniqueIdForType<ShapeData>(_dataToEdit);

            case BlanksLibrary.BlankType.Island:
                return ValidateUniqueIdForType<IslandData>(_dataToEdit);
        }
#endif

        return true;
    }

#if UNITY_EDITOR
    /// <summary>
    /// Helper method to validate that the provided ID is Unique
    /// </summary>
    private bool ValidateUniqueIdForType<T>(ScriptableObject currentData) where T : ScriptableObject, IData
    {
        var typedCurrentData = currentData as T;
        var allData = UnityEditor.AssetDatabase.FindAssets($"t:{typeof(T).Name}")
            .Select(guid => UnityEditor.AssetDatabase.GUIDToAssetPath(guid))
            .Select(path => UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path))
            .Where(data => data != null)
            .ToList();

        var duplicate = allData.FirstOrDefault(data => data != typedCurrentData && data.id == _id);
        if (duplicate != null)
        {
            Debug.LogError($"ID '{_id}' is already used by another {typeof(T).Name} asset: {duplicate.name}. Please use a unique ID");
            return false;
        }

        return true;
    }
#endif

    /// <summary>
    /// Bakes current baker field values to the assigned ScriptableObject based on <see cref="_blankType"/>
    /// </summary>
    public void BakeToScriptableObject()
    {
        // --- Preparations ---
        if (!PrepareToBake())
        {
            Debug.LogError("Failed to prepare data for baking. Aborting BakeToScriptableObject");
            return;
        }

        // --- Validate ID ---
        if (!ValidateId())
        {
            Debug.LogError("ID validation failed. Aborting BakeToScriptableObject");
            return;
        }

        switch (_blankType)
        {
            case BlanksLibrary.BlankType.Trunk:
                var trunkData = _dataToEdit as TrunkData;
                if (trunkData == null)
                {
                    Debug.LogError("_dataToEdit is not a TrunkData! Please assign a TrunkData ScriptableObject");
                    return;
                }

                // --- Bake Metadata ---
                trunkData.id = _id;
                trunkData.avaliableSide = _avaliableSide;
                trunkData.canBeYFlipped = _canBeFlippedVertically;

                // --- Bake Component Data ---
                _bakeable.GatherData(trunkData);

#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(trunkData);
                UnityEditor.AssetDatabase.SaveAssets();
#endif
                Debug.Log($"Data successfully baked to <b>{trunkData.name}</b> (ID: {trunkData.id})");
                break;

            case BlanksLibrary.BlankType.Chunk:
                var chunkData = _dataToEdit as ChunkData;
                if (chunkData == null)
                {
                    Debug.LogError("_dataToEdit is not a ChunkData! Please assign a ChunkData ScriptableObject");
                    return;
                }

                // --- Bake Metadata ---
                chunkData.id = _id;

                // --- Bake Component Data ---
                _bakeable.GatherData(chunkData);

#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(chunkData);
                UnityEditor.AssetDatabase.SaveAssets();
#endif
                Debug.Log($"Data successfully baked to <b>{chunkData.name}</b> (ID: {chunkData.id})");
                break;

            case BlanksLibrary.BlankType.Branch:
                var branchData = _dataToEdit as BranchData;
                if (branchData == null)
                {
                    Debug.LogError("_dataToEdit is not a BranchData! Please assign a BranchData ScriptableObject");
                    return;
                }

                // --- Bake Metadata ---
                branchData.id = _id;
                branchData.avaliableSide = _branchAvaliableSide;

                // --- Bake Component Data ---
                _bakeable.GatherData(branchData);

#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(branchData);
                UnityEditor.AssetDatabase.SaveAssets();
#endif
                Debug.Log($"Data successfully baked to <b>{branchData.name}</b> (ID: {branchData.id})");
                break;

            case BlanksLibrary.BlankType.Shape:
                var shapeData = _dataToEdit as ShapeData;
                if (shapeData == null)
                {
                    Debug.LogError("_dataToEdit is not a ShapeData! Please assign a ShapeData ScriptableObject");
                    return;
                }

                // --- Bake Metadata ---
                shapeData.id = _id;
                shapeData.type = _shapeType;
                // Tips cannot be flipped horizontally -> canBeXFlipped is false for Tips
                shapeData.canBeXFlipped = _shapeType != ShapeData.Type.Tip && _canBeFlippedHorizontally;

                // --- Bake Component Data ---
                _bakeable.GatherData(shapeData);

#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(shapeData);
                UnityEditor.AssetDatabase.SaveAssets();
#endif
                Debug.Log($"Data successfully baked to <b>{shapeData.name}</b> (ID: {shapeData.id})");
                break;

            case BlanksLibrary.BlankType.Island:
                var islandData = _dataToEdit as IslandData;
                if (islandData == null)
                {
                    Debug.LogError("_dataToEdit is not an IslandData! Please assign an IslandData ScriptableObject");
                    return;
                }

                // --- Bake Metadata ---
                islandData.id = _id;
                islandData.canBeXFlipped = _islandCanBeXFlipped;
                islandData.allowLeft = _islandAllowLeft;
                islandData.allowRight = _islandAllowRight;
                islandData.allowMiddle = _islandAllowMiddle;

                // --- Bake Component Data ---
                _bakeable.GatherData(islandData);

#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(islandData);
                UnityEditor.AssetDatabase.SaveAssets();
#endif
                Debug.Log($"Data successfully baked to <b>{islandData.name}</b> (ID: {islandData.id})");
                break;

            default:
                Debug.LogError($"Unknown BlankType: {_blankType}. Aborting BakeToScriptableObject");
                break;
        }
    }

    public BlanksLibrary.BlankType BlankType => _blankType;

    public ScriptableObject DataToEdit => _dataToEdit;
}
