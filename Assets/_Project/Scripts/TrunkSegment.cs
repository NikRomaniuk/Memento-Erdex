using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D))]
public class TrunkSegment : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private TrunkData _trunkData;
    
    private SpriteRenderer _spriteRenderer;
    private BoxCollider2D _boxCollider;

    // [Header("Parameters")]
    // [Range(0f, 1f)] public float slipperyValue = 0;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _boxCollider = GetComponent<BoxCollider2D>();

        ApplyData();
    }

    private void ApplyData()
    {
        if (_trunkData == null) return;

        if (_spriteRenderer != null && _trunkData.sprite != null)
            _spriteRenderer.sprite = _trunkData.sprite;

        if (_boxCollider != null && _trunkData.collider != null)
        {
            _boxCollider.offset = _trunkData.collider.offset;
            _boxCollider.size = _trunkData.collider.size;
        }
    }

    /// <summary>
    /// Initialize segment with TrunkData (for Object Pooling)
    /// </summary>
    public void Initialize(TrunkData data)
    {
        _trunkData = data;
        ApplyData();
    }

    // Public accessor in case other systems need the data at runtime
    public TrunkData Data => _trunkData;
}
