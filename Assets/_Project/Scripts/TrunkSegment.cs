using UnityEngine;

public enum Side
{
    Left,
    Right
}

[RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D))]
public class TrunkSegment : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private TrunkData _trunkData;
    // --- References ---
    private SpriteRenderer _spriteRenderer;
    private BoxCollider2D _boxCollider;

    // --- General ---
    public Side _side = Side.Right; // On which side is this trunk segment

    // --- Maths ---
    // Points
    [Step(0.05f)] private Vector2 _downNearPoint;
    [Step(0.05f)] private Vector2 _downFarPoint;
    [Step(0.05f)] private Vector2 _topNearPoint;
    [Step(0.05f)] private Vector2 _topFarPoint;
    // Trunk widths
    private float _downWidth;
    private float _topWidth;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _boxCollider = GetComponent<BoxCollider2D>();
    }

    private void ApplyData()
    {
        if (_trunkData == null) return;

        // --- Helpers ---
        var flipX = _side == Side.Left ? -1 : 1; // Flip some values if on the left side

        // --- Apply visual data ---
        _spriteRenderer.sprite = _trunkData.sprite; // Set sprite
        // Flip sprite visually
        if (_side == Side.Right){ _spriteRenderer.flipX = false; }
        else {_spriteRenderer.flipX = true; }
        // Flip sprite X coords
        var pos = _spriteRenderer.transform.localPosition;
        _spriteRenderer.transform.localPosition = new Vector3(pos.x * flipX, pos.y, pos.z); 

        // --- Apply collider data ---
        _boxCollider.offset = _trunkData.collider.offset;
        _boxCollider.size = _trunkData.collider.size;

        // --- Apply points data ---
        // Set up points
        _downNearPoint = _trunkData.downNearPoint;
        _downFarPoint = new Vector2(_trunkData.downFarPoint.x * flipX, _trunkData.downFarPoint.y);
        _topNearPoint = _trunkData.topNearPoint;
        _topFarPoint = new Vector2(_trunkData.topFarPoint.x * flipX, _trunkData.topFarPoint.y);
        // Calculate widths
        _downWidth = Vector2.Distance(_downNearPoint, _downFarPoint);
        _topWidth = Vector2.Distance(_topNearPoint, _topFarPoint);
    }

    /// <summary>
    /// Initialize segment with TrunkData (for Object Pooling)
    /// </summary>
    public void Initialize(TrunkData data, Side side)
    {
        _trunkData = data;
        _side = side;
        ApplyData();
    }

    // Public accessor in case other systems need the data at runtime
    public TrunkData Data => _trunkData;
    public float DownWidth => _downWidth;
    public float TopWidth => _topWidth;
}
