using UnityEngine;
using System.Linq;

public class ChunkBaker : MonoBehaviour
{
    [Header("Reference Data")]
    [SerializeField] private ChunkData _dataToEdit;

    [Header("General Data")]
    [SerializeField] private string _id;

    // --- Components ---
    private ChunkManager _manager;

    /// <summary>
    /// Loads references from ChunkManager on this GameObject
    /// </summary>
    public bool PrepareToBake()
    {
        _manager = GetComponent<ChunkManager>();
        if (_manager == null)
        {
            Debug.LogError("ChunkManager component not found on this GameObject!");
            return false;
        }

        Debug.Log("References loaded from ChunkManager");
        return true;
    }

    private bool ValidateId()
    {
        // Check if ID is empty
        if (string.IsNullOrWhiteSpace(_id))
        {
            Debug.LogError("ID cannot be empty! Please provide a valid ID");
            return false;
        }

#if UNITY_EDITOR
        // Find all ChunkData in the project
        string[] guids = UnityEditor.AssetDatabase.FindAssets("t:ChunkData");
        var allChunkData = guids
            .Select(guid => UnityEditor.AssetDatabase.GUIDToAssetPath(guid))
            .Select(path => UnityEditor.AssetDatabase.LoadAssetAtPath<ChunkData>(path))
            .Where(data => data != null)
            .ToList();

        // Check if ID is unique (excluding the current asset we're editing)
        var duplicates = allChunkData.Where(data => data != _dataToEdit && data.id == _id).ToList();
        
        if (duplicates.Count > 0)
        {
            Debug.LogError($"ID '{_id}' is already used by another ChunkData asset: {duplicates[0].name}. Please use a unique ID");
            return false;
        }
#endif

        return true;
    }

    /// <summary>
    /// Bakes current baker field values to the assigned <see cref="_dataToEdit"/> ScriptableObject
    /// </summary>
    public void BakeToScriptableObject()
    {
        // --- Preparations ---
        if (!PrepareToBake())
        {
            Debug.LogError("Failed to prepare data for baking. Aborting BakeToScriptableObject");
            return;
        }

        if (_dataToEdit == null)
        {
            Debug.LogError("No ChunkData assigned! Please assign a ChunkData ScriptableObject to bake into");
            return;
        }

        // --- Validate ID ---
        if (!ValidateId())
        {
            Debug.LogError("ID validation failed. Aborting BakeToScriptableObject");
            return;
        }

        // --- Bake general data ---
        _dataToEdit.id = _id;
        _dataToEdit.height = _manager.Height;

        // --- Bake slots data ---
        if (_manager.BranchSlots != null)
        {
            _dataToEdit.branchSlots = new BranchSlot[_manager.BranchSlots.Length];
            System.Array.Copy(_manager.BranchSlots, _dataToEdit.branchSlots, _manager.BranchSlots.Length);
        }
        else
        {
            _dataToEdit.branchSlots = new BranchSlot[0];
        }

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(_dataToEdit);
        UnityEditor.AssetDatabase.SaveAssets();
#endif

        Debug.Log($"Data successfully baked to <b>{_dataToEdit.name}</b>");
    }

    public ChunkData DataToEdit => _dataToEdit;
}
