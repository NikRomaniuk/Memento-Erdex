using UnityEngine;

public class TrunkBuilder : MonoBehaviour
{
    [Header("Reference Data")]
    [SerializeField] private TrunkData _dataToBuild;

    [Header("Build Settings")]
    [SerializeField] private Side _side = Side.Right;
    [SerializeField] private bool _isYFlipped = false;

    private TrunkSegment _trunkPart;
    private SpriteRenderer _visual;
    private SpriteRenderer _visualOutline;
    private BoxCollider2D _collider;

    public bool PrepareToBuild()
    {
        _trunkPart = GetComponent<TrunkSegment>();
        if (_trunkPart == null)
        {
            Debug.LogError("TrunkSegment component not found on this GameObject!");
            return false;
        }

        _visual = _trunkPart._spriteRenderer;
        _collider = _trunkPart._boxCollider;
        _visualOutline = _trunkPart._spriteRenderer.transform.GetChild(0).GetComponent<SpriteRenderer>(); // Get outline

        Debug.Log("References loaded from TrunkSegment");
        return true;
    }

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
            Debug.LogError("No TrunkData assigned! Please assign a TrunkData ScriptableObject");
            return;
        }

        // --- Initialize TrunkSegment with data ---
        Initialize(_dataToBuild, _side, _isYFlipped);

        Debug.Log($"TrunkPart successfully built from <b>{_dataToBuild.name}</b> (ID: {_dataToBuild.id})");
    }

    private void ApplyVisuals(int flipX)
    {
        // ==============
        // --- Sprite ---
        // ==============
        _visual.sprite = _dataToBuild.sprite; // Set sprite

        // Flip sprite visually
        // Horizontal
        if (_side == Side.Right){ _visual.flipX = false; }
        else { _visual.flipX = true; }
        // Vertical
        _visual.flipY = _isYFlipped;


        // Set up sprite position
        _visual.transform.localPosition = new Vector3(_dataToBuild.spriteOffset.x * flipX, _dataToBuild.spriteOffset.y, 0); 
        
        // ===============
        // --- Outline ---
        // ===============
        _visualOutline.sprite = _dataToBuild.sprite; // Set outline sprite

        // Flip sprite visually
        // Horizontal
        if (_side == Side.Right){ _visualOutline.flipX = false; }
        else {_visualOutline.flipX = true; }
        // Vertical
        _visualOutline.flipY = _isYFlipped;
    }

    private void ApplyData()
    {
        if (_dataToBuild == null) return;

        // --- Helpers ---
        var flipX = _side == Side.Left ? -1 : 1; // Flip some values if on the left side

        // --- Apply visual data ---
        ApplyVisuals(flipX);

        // --- Apply collider data ---
        //_trunkPart._boxCollider.offset = _dataToBuild.collider.offset;
        _collider.size = _dataToBuild.colliderSize;
        _collider.offset = _dataToBuild.colliderOffset;

        // --- Apply points data ---
        // Set up points
        Vector2 downNearPoint = _dataToBuild.downNearPoint;
        Vector2 downFarPoint = new Vector2(_dataToBuild.downFarPoint.x * flipX, _dataToBuild.downFarPoint.y);
        Vector2 topNearPoint = _dataToBuild.topNearPoint;
        Vector2 topFarPoint = new Vector2(_dataToBuild.topFarPoint.x * flipX, _dataToBuild.topFarPoint.y);
        
        _trunkPart.SetPoints(downNearPoint, downFarPoint, topNearPoint, topFarPoint);
    }

    /// <summary>
    /// Initialize segment with TrunkData
    /// </summary>
    private void Initialize(TrunkData data, Side side, bool isYFlipped)
    {
        if (_trunkPart == null) return;

        _dataToBuild = data;
        _side = side;
        _isYFlipped = isYFlipped;
        
        ApplyData();

#if UNITY_EDITOR
        // Mark scene as dirty to save changes
        UnityEditor.EditorUtility.SetDirty(_trunkPart);
        UnityEditor.EditorUtility.SetDirty(gameObject);
#endif
    }

    /// <summary>
    /// Clear TrunkPart and reset to initial state
    /// </summary>
    public void Clear()
    {
        // --- Preparations ---
        if (!PrepareToBuild())
        {
            Debug.LogError("Failed to prepare components. Aborting Clear");
            return;
        }

        // --- Reset sprites ---
        _visual.sprite = null;
        _visualOutline.sprite = null;

        // --- Reset transforms ---
        _visual.flipX = false;
        _visual.flipY = false;
        _visualOutline.flipX = false;
        _visualOutline.flipY = false;
        _visual.transform.localPosition = Vector3.zero;

        // --- Reset collider ---
        _collider.size = Vector2.zero;
        _collider.offset = Vector2.zero;

        // --- Reset points ---
        _trunkPart.SetPoints(Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero);

#if UNITY_EDITOR
        // Mark scene as dirty to save changes
        UnityEditor.EditorUtility.SetDirty(_trunkPart);
        UnityEditor.EditorUtility.SetDirty(gameObject);
#endif

        Debug.Log("TrunkPart cleared to initial state");
    }

    public TrunkData DataToBuild => _dataToBuild;
}
