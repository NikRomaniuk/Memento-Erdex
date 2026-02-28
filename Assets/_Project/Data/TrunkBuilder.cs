using UnityEngine;

public class TrunkBuilder : MonoBehaviour
{
    [Header("Reference Data")]
    [SerializeField] private TrunkData _dataToEdit;
    private SpriteRenderer _visual;
    private BoxCollider2D _physics;

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

    public void BakeToScriptableObject()
    {
        // --- Preparations ---
        // Try to get components from TrunkSegment
        if (!PrepareToBake())
        {
            Debug.LogError("Failed to prepare data for baking. Aborting BakeToScriptableObject");
            return;
        }

        // --- Bake visual data ---
        _dataToEdit.sprite = _visual.sprite;

        // --- Bake collider data ---
        _dataToEdit.collider = _physics;

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(_dataToEdit);
        UnityEditor.AssetDatabase.SaveAssets();
#endif

        Debug.Log($"Data successfully baked to <b>{_dataToEdit.name}</b> (ID: {_dataToEdit.id})");
    }

    public TrunkData DataToEdit => _dataToEdit;
}
