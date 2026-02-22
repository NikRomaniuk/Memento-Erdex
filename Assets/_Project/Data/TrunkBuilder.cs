using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D))]
public class TrunkBuilder : MonoBehaviour
{
    [Header("Reference Data")]
    [SerializeField] private TrunkData _dataToEdit;
    
    [Header("Scene Components")]
    [SerializeField] private SpriteRenderer _visual;
    [SerializeField] private BoxCollider2D _physics;

    public void BakeToScriptableObject()
    {
        // Bake visual data
        _dataToEdit.sprite = _visual.sprite;

        // Bake collider data
        _dataToEdit.collider = _physics;

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(_dataToEdit);
        UnityEditor.AssetDatabase.SaveAssets();
#endif

        Debug.Log($"Data successfully baked to <b>{_dataToEdit.name}</b> (ID: {_dataToEdit.id})");
    }

    public TrunkData DataToEdit => _dataToEdit;
}
