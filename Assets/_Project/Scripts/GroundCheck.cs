using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    // --- References ---
    [SerializeField] private CharacterManager _characterManager;

    // --- Ground Check ---
    [SerializeField] private string _surfaceTag = "Surface";
    [SerializeField] private LayerMask _surfaceMask = Physics2D.DefaultRaycastLayers;
    [SerializeField, Min(0.01f)] private float _rayDistance = 0.6f;
    [SerializeField] private float _rayStartYOffset = 0.05f;
    [SerializeField, Min(0.01f)] private float _boxHeight = 0.1f;
    [SerializeField] private float maxSlopeAngle = 45f;

    // --- Debug ---
    [SerializeField] private bool _drawDebug = true;
    [SerializeField] private bool _debug = false;

    // --- Cache ---
    private string _lastDebug;
    private Collider2D _collider;

    /// <summary>
    /// Caches required components
    /// </summary>
    private void Awake()
    {
        _collider = _characterManager.Collider;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    /// <summary>
    /// Casts a box down from the collider base and validates Surface tag
    /// </summary>
    public bool HasSurfaceBelow()
    {
        RaycastHit2D[] hits = Physics2D.BoxCastAll(GetBoxOrigin(), GetBoxSize(), 0f, Vector2.down, _rayDistance, _surfaceMask);

        for (int i = 0; i < hits.Length; i++)
        {
            Collider2D hitCollider = hits[i].collider;
            if (hitCollider == null) { continue; }

            float angle = Vector2.Angle(hits[i].normal, Vector2.up);
            if (angle > maxSlopeAngle) { continue; } // Too steep slope -> Skip

            // Ignore character colliders on this object and children
            if (hitCollider.transform == transform || hitCollider.transform.IsChildOf(transform))
            {
                continue;
            }

            if (hitCollider.CompareTag(_surfaceTag)) // Touched Surface? True
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Returns origin (center) for the box cast
    /// </summary>
    private Vector2 GetBoxOrigin()
    {
        if (_collider == null)
        {
            _collider = _characterManager.Collider;
        }

        Bounds bounds = _collider.bounds;
        return new Vector2(bounds.center.x, bounds.min.y + _rayStartYOffset + _boxHeight * 0.5f);
    }

    /// <summary>
    /// Returns size of the box cast
    /// </summary>
    private Vector2 GetBoxSize()
    {
        if (_collider == null)
        {
            return new Vector2(1f, _boxHeight);
        }

        Bounds bounds = _collider.bounds;
        return new Vector2(bounds.size.x, _boxHeight);
    }

    /// <summary>
    /// Prints Debug messages
    /// </summary>
    private void D(string message)
    {
        if (!_debug) { return; }
        if (_lastDebug == message) { return; } // Same message -> Skip (Used to avoid spamming)

        _lastDebug = message;
        Debug.Log($"[CharacterMovement] {message}", this);
    }

    /// <summary>
    /// Draws BoxCast used for ground detection
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (!_drawDebug) { return; }

        Vector2 origin = GetBoxOrigin();
        Vector2 size = GetBoxSize();

        // Draw starting box
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(origin, size);

        // Draw ending box
        Gizmos.color = new Color(1f, 1f, 0f, 0.35f);
        Gizmos.DrawWireCube(origin + Vector2.down * _rayDistance, size);

        // Connect corners with lines
        Gizmos.color = Color.yellow;
        Vector2 halfSize = size * 0.5f;
        Vector2 br = origin + new Vector2(halfSize.x, -halfSize.y);
        Vector2 bl = origin + new Vector2(-halfSize.x, -halfSize.y);
        Vector2 tr = origin + new Vector2(halfSize.x, halfSize.y);
        Vector2 tl = origin + new Vector2(-halfSize.x, halfSize.y);

        Gizmos.DrawLine(br, br + Vector2.down * _rayDistance);
        Gizmos.DrawLine(bl, bl + Vector2.down * _rayDistance);
        Gizmos.DrawLine(tr, tr + Vector2.down * _rayDistance);
        Gizmos.DrawLine(tl, tl + Vector2.down * _rayDistance);
    }
}
