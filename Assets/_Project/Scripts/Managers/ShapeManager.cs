using UnityEngine;

public class ShapeManager : MonoBehaviour, IBakeable, IBuildable
{
    [Header("References")]
    // --- References ---
    public SpriteRenderer _spriteRenderer;
    public SpriteRenderer _outlineRenderer;

    [Header("Properties")]
    // --- Maths ---
    // Points
    [Step(0.05f)] [SerializeField] private Vector2 _topNearPoint;
    [Step(0.05f)] [SerializeField] private Vector2 _downNearPoint;
    [Step(0.05f)] [SerializeField] private Vector2 _topFarPoint;
    [Step(0.05f)] [SerializeField] private Vector2 _downFarPoint;

    // Heights
    private float _leftHeight;
    private float _rightHeight;
    [SerializeField] private Color _defaultOutlineColor = Color.black;

    private SpriteView _spriteView;
    private OutlineView _outlineView;

    public SpriteView SpriteView => _spriteView;
    public OutlineView OutlineView => _outlineView;

    private void Awake()
    {
        EnsureViewsInitialized();
    }

    private void EnsureViewsInitialized()
    {
        _spriteView ??= new SpriteView(_spriteRenderer);
        _outlineView ??= new OutlineView(_outlineRenderer, _defaultOutlineColor);
    }

    /// <summary>
    /// Set points data and calculate heights
    /// </summary>
    public void SetPoints(Vector2 topNear, Vector2 downNear, Vector2 topFar, Vector2 downFar)
    {
        _topNearPoint = topNear;
        _downNearPoint = downNear;
        _topFarPoint = topFar;
        _downFarPoint = downFar;
        
        // Calculate heights
        _leftHeight = Vector2.Distance(_topNearPoint, _downNearPoint);
        _rightHeight = Vector2.Distance(_topFarPoint, _downFarPoint);
    }

    /// <summary>
    /// Returns the coordinates of all snap points
    /// </summary>
    /// <returns>Array containing [topNear, downNear, topFar, downFar] points</returns>
    public Vector2[] GetPoints()
    {
        return new Vector2[] 
        { 
            _topNearPoint, 
            _downNearPoint, 
            _topFarPoint, 
            _downFarPoint 
        };
    }

    // Public accessors
    public float LeftHeight => _leftHeight;
    public float RightHeight => _rightHeight;

    // --- IBakeable ---
    public void GatherData(IData data)
    {
        if (data is not ShapeData shapeData) return;
        EnsureViewsInitialized();

        // --- Bake visual data ---
        shapeData.sprite = _spriteRenderer.sprite;
        shapeData.spriteOffset = _spriteRenderer.transform.localPosition;
        shapeData.defaultOutlineColor = _outlineView.DefaultColor;

        // --- Bake points data ---
        var points = GetPoints();
        shapeData.topNearPoint = points[0];
        shapeData.downNearPoint = points[1];
        shapeData.topFarPoint = points[2];
        shapeData.downFarPoint = points[3];
    }

    // --- IBuildable ---
    public void SetData(IData data)
    {
        if (data is not ShapeData shapeData) return;
        SetData(shapeData, Side.Right, false);
    }

    public void SetData(ShapeData data, Side side, bool isXFlipped)
    {
        EnsureViewsInitialized();

        // Tip flip is auto-derived from side and overrides incoming flag
        if (data.type == ShapeData.Type.Tip)
        {
            isXFlipped = side == Side.Left;
        }

        bool shouldFlipX = isXFlipped;
        Vector3 spriteLocalPosition = data.spriteOffset;
        if (side == Side.Left)
        {
            spriteLocalPosition.x *= -1;
        }

        // --- Apply Visual Data ---
        _spriteView.SetData(data, shouldFlipX, spriteLocalPosition);
        _outlineView.SetData(data, shouldFlipX);

        // --- Apply Points Data ---
        Vector2 topNearPoint = data.topNearPoint;
        Vector2 downNearPoint = data.downNearPoint;
        Vector2 topFarPoint = data.topFarPoint;
        Vector2 downFarPoint = data.downFarPoint;

        if (side == Side.Left)
        {
            // Left side mirrors far points only on X axis
            downFarPoint.x *= -1;
            topFarPoint.x *= -1;

            // Left side rule: down points exchange Y coordinates
            float tmpY = downNearPoint.y;
            downNearPoint.y = downFarPoint.y;
            downFarPoint.y = tmpY;
        }

        if (isXFlipped)
        {
            // Explicit X-flip rule also swaps down points by Y
            float tmpY = downNearPoint.y;
            downNearPoint.y = downFarPoint.y;
            downFarPoint.y = tmpY;
        }

        SetPoints(topNearPoint, downNearPoint, topFarPoint, downFarPoint);

        // --- Apply Misc ---
        _defaultOutlineColor = _outlineView.DefaultColor;
    }

    public void Clear()
    {
        EnsureViewsInitialized();

        // --- Reset Visuals ---
        _spriteView.Clear();
        _outlineView.Clear();

        // --- Reset Points ---
        SetPoints(Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero);

        // --- Reset Misc ---
        _defaultOutlineColor = _outlineView.DefaultColor;

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
        UnityEditor.EditorUtility.SetDirty(gameObject);
#endif
        gameObject.SetActive(false);
        Debug.Log("Shape cleared to initial state");
    }

#if UNITY_EDITOR
    // Draw points in Editor
    private void OnDrawGizmosSelected()
    {
        // Private constants for gizmos
        const float iconWorldSize = 0.2f; // Size in world units
        Color nearColor = new Color(0.7f, 0.45f, 1f); // Purple
        Color farColor = new Color(0.45f, 0.4f, 1f); // Deep Purple-blue
        
        // Icon paths (Path starts from Resources folder)
        const string topNearIconPath = "Gizmos/Icons/TopNearPoint";
        const string downNearIconPath = "Gizmos/Icons/DownNearPoint";
        const string topFarIconPath = "Gizmos/Icons/TopFarPoint";
        const string downFarIconPath = "Gizmos/Icons/DownFarPoint";
        
        // Load icons
        Texture2D topNearIcon = Resources.Load<Texture2D>(topNearIconPath);
        Texture2D downNearIcon = Resources.Load<Texture2D>(downNearIconPath);
        Texture2D topFarIcon = Resources.Load<Texture2D>(topFarIconPath);
        Texture2D downFarIcon = Resources.Load<Texture2D>(downFarIconPath);

        // Draw near points (red)
        DrawIcon(transform.TransformPoint(_topNearPoint), topNearIcon, iconWorldSize, nearColor);
        DrawIcon(transform.TransformPoint(_downNearPoint), downNearIcon, iconWorldSize, nearColor);

        // Draw far points (purple-red)
        DrawIcon(transform.TransformPoint(_topFarPoint), topFarIcon, iconWorldSize, farColor);
        DrawIcon(transform.TransformPoint(_downFarPoint), downFarIcon, iconWorldSize, farColor);
    }

    // Helper method to draw icons in the scene view
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
