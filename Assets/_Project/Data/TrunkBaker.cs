using UnityEngine;
using System.Linq;

public class TrunkBaker : MonoBehaviour
{
    [Header("Reference Data")]
    [SerializeField] private TrunkData _dataToEdit;
    private SpriteRenderer _visual;
    private BoxCollider2D _physics;
    

    [Header("General Data")]
    [SerializeField] private string _id;
    [SerializeField] private TrunkAvaliableSide _avaliableSide = TrunkAvaliableSide.Both;
    [SerializeField] private bool _canBeFlippedVertically = true;

    public bool PrepareToBake()
    {
        var segment = GetComponent<TrunkSegment>();
        if (segment == null)
        {
            Debug.LogError("TrunkSegment component not found on this GameObject!");
            return false;
        }

        _visual = segment._spriteRenderer;
        _physics = segment._boxCollider;

        Debug.Log("References loaded from TrunkSegment");
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
        // Find all TrunkData in the project
        string[] guids = UnityEditor.AssetDatabase.FindAssets("t:TrunkData");
        var allTrunkData = guids
            .Select(guid => UnityEditor.AssetDatabase.GUIDToAssetPath(guid))
            .Select(path => UnityEditor.AssetDatabase.LoadAssetAtPath<TrunkData>(path))
            .Where(data => data != null)
            .ToList();

        // Check if ID is unique (excluding the current asset we're editing)
        var duplicates = allTrunkData.Where(data => data != _dataToEdit && data.id == _id).ToList();
        
        if (duplicates.Count > 0)
        {
            Debug.LogError($"ID '{_id}' is already used by another TrunkData asset: {duplicates[0].name}. Please use a unique ID");
            return false;
        }
#endif

        return true;
    }

    public void BakeToScriptableObject()
    {
        // --- Preparations ---
        // Try to get components from TrunkSegment
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

        // --- Bake general data ---
        _dataToEdit.id = _id;
        _dataToEdit.avaliableSide = _avaliableSide;
        _dataToEdit.canBeYFlipped = _canBeFlippedVertically;

        // --- Bake visual data ---
        _dataToEdit.sprite = _visual.sprite;

        // --- Bake collider data ---
        _dataToEdit.colliderSize = _physics.size;

        // --- Bake points data ---
        var segment = GetComponent<TrunkSegment>();
        if (segment != null)
        {
            var points = segment.GetPoints();
            _dataToEdit.downNearPoint = points[0];
            _dataToEdit.downFarPoint = points[1];
            _dataToEdit.topNearPoint = points[2];
            _dataToEdit.topFarPoint = points[3];
        }

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(_dataToEdit);
        UnityEditor.AssetDatabase.SaveAssets();
#endif

        Debug.Log($"Data successfully baked to <b>{_dataToEdit.name}</b> (ID: {_dataToEdit.id})");
    }

    public TrunkData DataToEdit => _dataToEdit;
}

