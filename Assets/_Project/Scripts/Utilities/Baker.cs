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
                var trunkData = _dataToEdit as TrunkData;
                var allTrunkData = UnityEditor.AssetDatabase.FindAssets("t:TrunkData")
                    .Select(guid => UnityEditor.AssetDatabase.GUIDToAssetPath(guid))
                    .Select(path => UnityEditor.AssetDatabase.LoadAssetAtPath<TrunkData>(path))
                    .Where(data => data != null)
                    .ToList();

                var trunkDuplicates = allTrunkData.Where(data => data != trunkData && data.id == _id).ToList();
                if (trunkDuplicates.Count > 0)
                {
                    Debug.LogError($"ID '{_id}' is already used by another TrunkData asset: {trunkDuplicates[0].name}. Please use a unique ID");
                    return false;
                }
                break;

            case BlanksLibrary.BlankType.Chunk:
                var chunkData = _dataToEdit as ChunkData;
                var allChunkData = UnityEditor.AssetDatabase.FindAssets("t:ChunkData")
                    .Select(guid => UnityEditor.AssetDatabase.GUIDToAssetPath(guid))
                    .Select(path => UnityEditor.AssetDatabase.LoadAssetAtPath<ChunkData>(path))
                    .Where(data => data != null)
                    .ToList();

                var chunkDuplicates = allChunkData.Where(data => data != chunkData && data.id == _id).ToList();
                if (chunkDuplicates.Count > 0)
                {
                    Debug.LogError($"ID '{_id}' is already used by another ChunkData asset: {chunkDuplicates[0].name}. Please use a unique ID");
                    return false;
                }
                break;
        }
#endif

        return true;
    }

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

            default:
                Debug.LogError($"Unknown BlankType: {_blankType}. Aborting BakeToScriptableObject");
                break;
        }
    }

    public BlanksLibrary.BlankType BlankType => _blankType;

    public ScriptableObject DataToEdit => _dataToEdit;
}
