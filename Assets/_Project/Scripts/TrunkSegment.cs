using UnityEngine;

public enum Side
{
    Left,
    Right
}

public class TrunkSegment : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private TrunkData _trunkData;
    // --- References ---
    public SpriteRenderer _spriteRenderer;
    private SpriteRenderer _outlineRenderer;
    public BoxCollider2D _boxCollider;

    // --- General ---
    public Side _side = Side.Right; // On which side is this trunk segment

    // --- Maths ---
    // Points
    [Step(0.05f)] [SerializeField] private Vector2 _downNearPoint;
    [Step(0.05f)] [SerializeField] private Vector2 _downFarPoint;
    [Step(0.05f)] [SerializeField] private Vector2 _topNearPoint;
    [Step(0.05f)] [SerializeField] private Vector2 _topFarPoint;
    // Trunk widths
    private float _downWidth;
    private float _topWidth;

    private void Awake()
    {
        // --- Preparations ---

        // Get outline
        _outlineRenderer = _spriteRenderer.transform.GetChild(0).GetComponent<SpriteRenderer>();
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

#if UNITY_EDITOR
    // Draw points in editor
    private void OnDrawGizmosSelected()
    {
        // Private constants for gizmos
        const float iconWorldSize = 0.2f; // Size in world units
        Color nearColor = new Color(0.7f, 0.45f, 1f); // Purple
        Color farColor = new Color(0.45f, 0.4f, 1f); // Deep Purple-blue
        
        // Icon paths (place your textures in Resources folder or use direct asset paths)
        const string downNearIconPath = "Gizmos/Icons/DownNearPoint";
        const string downFarIconPath = "Gizmos/Icons/DownFarPoint";
        const string topNearIconPath = "Gizmos/Icons/TopNearPoint";
        const string topFarIconPath = "Gizmos/Icons/TopFarPoint";
        
        // Load icons
        Texture2D downNearIcon = Resources.Load<Texture2D>(downNearIconPath);
        Texture2D downFarIcon = Resources.Load<Texture2D>(downFarIconPath);
        Texture2D topNearIcon = Resources.Load<Texture2D>(topNearIconPath);
        Texture2D topFarIcon = Resources.Load<Texture2D>(topFarIconPath);

        // Draw near points (red)
        DrawIcon(transform.TransformPoint(_downNearPoint), downNearIcon, iconWorldSize, nearColor);
        DrawIcon(transform.TransformPoint(_topNearPoint), topNearIcon, iconWorldSize, nearColor);

        // Draw far points (purple-red)
        DrawIcon(transform.TransformPoint(_downFarPoint), downFarIcon, iconWorldSize, farColor);
        DrawIcon(transform.TransformPoint(_topFarPoint), topFarIcon, iconWorldSize, farColor);
    }

    private void DrawIcon(Vector3 worldPosition, Texture2D icon, float worldSize, Color color)
    {
        if (icon == null) return;

        // Get scene view camera
        Camera cam = UnityEditor.SceneView.lastActiveSceneView?.camera;
        if (cam == null) return;

        // Calculate screen size to maintain constant world size
        float screenSize;
        if (cam.orthographic)
        {
            // For orthographic camera
            float screenHeight = cam.pixelHeight;
            float worldHeight = cam.orthographicSize * 2;
            float pixelsPerWorldUnit = screenHeight / worldHeight;
            screenSize = worldSize * pixelsPerWorldUnit;
        }
        else
        {
            // For perspective camera
            float distance = Vector3.Distance(cam.transform.position, worldPosition);
            float frustumHeight = 2.0f * distance * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
            float pixelsPerWorldUnit = cam.pixelHeight / frustumHeight;
            screenSize = worldSize * pixelsPerWorldUnit;
        }

        UnityEditor.Handles.BeginGUI();
        
        Vector2 screenPos = UnityEditor.HandleUtility.WorldToGUIPoint(worldPosition);
        Rect rect = new Rect(screenPos.x - screenSize / 2, screenPos.y - screenSize / 2, screenSize, screenSize);
        
        Color oldColor = GUI.color;
        GUI.color = color;
        GUI.DrawTexture(rect, icon);
        GUI.color = oldColor;
        
        UnityEditor.Handles.EndGUI();
    }
#endif
}
