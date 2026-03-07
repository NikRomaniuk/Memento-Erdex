using UnityEngine;

public class ChunkBuilder : MonoBehaviour
{
    [Header("Reference Data")]
    [SerializeField] private ChunkData _dataToBuild;

    // --- Components ---
    private ChunkManager _chunkManager;

    /// <summary>
    /// Loads references from ChunkManager on this GameObject
    /// </summary>
    public bool PrepareToBuild()
    {
        _chunkManager = GetComponent<ChunkManager>();
        if (_chunkManager == null)
        {
            Debug.LogError("ChunkManager component not found on this GameObject!");
            return false;
        }

        Debug.Log("References loaded from ChunkManager");
        return true;
    }

    /// <summary>
    /// Builds this chunk from the assigned <see cref="_dataToBuild"/> ScriptableObject
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
            Debug.LogError("No ChunkData assigned! Please assign a ChunkData ScriptableObject");
            return;
        }

        // --- Initialize ChunkManager with data ---
        Initialize(_dataToBuild);

        Debug.Log($"Chunk successfully built from <b>{_dataToBuild.name}</b>");
    }

    private void ApplyData()
    {
        if (_dataToBuild == null) return;

        // --- Prepare Data ---
        // Get height from ChunkData
        float height = _dataToBuild.height;

        // Copy branch slots from ChunkData
        BranchSlot[] branchSlots;
        if (_dataToBuild.branchSlots != null)
        {
            branchSlots = new BranchSlot[_dataToBuild.branchSlots.Length];
            System.Array.Copy(_dataToBuild.branchSlots, branchSlots, _dataToBuild.branchSlots.Length);
        }
        else
        {
            branchSlots = new BranchSlot[0];
        }

        // --- Apply Data ---
        _chunkManager.SetData(height, branchSlots);
    }

    /// <summary>
    /// Initialize ChunkManager with ChunkData
    /// </summary>
    public void Initialize(ChunkData data)
    {
        if (_chunkManager == null) return;

        // --- Validations ---
        if (data == null)
        {
            Debug.LogWarning("Cannot initialize with null ChunkData");
            return;
        }

        // --- Assign data ---
        _dataToBuild = data;

        // --- Apply data ---
        ApplyData();

#if UNITY_EDITOR
        // Mark scene as dirty to save changes
        UnityEditor.EditorUtility.SetDirty(_chunkManager);
        UnityEditor.EditorUtility.SetDirty(gameObject);
#endif
    }

    /// <summary>
    /// Clear ChunkManager and reset to initial state
    /// </summary>
    public void Clear()
    {
        // --- Preparations ---
        if (!PrepareToBuild())
        {
            Debug.LogError("Failed to prepare components. Aborting Clear");
            return;
        }

        // --- Reset General Data ---
        _chunkManager.SetData(0f, new BranchSlot[0]);

#if UNITY_EDITOR
        // Mark scene as dirty to save changes
        UnityEditor.EditorUtility.SetDirty(_chunkManager);
        UnityEditor.EditorUtility.SetDirty(gameObject);
#endif

        // Deactivate GameObject
        _chunkManager.gameObject.SetActive(false);

        Debug.Log("Chunk cleared to initial state");
    }

    public ChunkData DataToBuild => _dataToBuild;
}
