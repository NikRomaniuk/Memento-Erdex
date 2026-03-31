using UnityEngine;

public class TrunkSegment : MonoBehaviour, IBakeable, IBuildable
{
    [Header("References")]
    // --- References ---
    public SpriteRenderer _spriteRenderer;
    public SpriteRenderer _outlineRenderer;
    public BoxCollider2D _boxCollider;

    [Header("Properties")]
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
    }

    /// <summary>
    /// Set points data and calculate widths
    /// </summary>
    public void SetPoints(Vector2 downNear, Vector2 downFar, Vector2 topNear, Vector2 topFar)
    {
        _downNearPoint = downNear;
        _downFarPoint = downFar;
        _topNearPoint = topNear;
        _topFarPoint = topFar;
        
        // Calculate widths
        _downWidth = Vector2.Distance(_downNearPoint, _downFarPoint);
        _topWidth = Vector2.Distance(_topNearPoint, _topFarPoint);
    }

    // Public accessors
    public float DownWidth => _downWidth;
    public float TopWidth => _topWidth;

    // --- IBakeable ---
    public void GatherData(IData data)
    {
        if (data is not TrunkData trunkData) return;

        // --- Bake visual data ---
        trunkData.sprite = _spriteRenderer.sprite;
        trunkData.spriteOffset = _spriteRenderer.transform.localPosition;

        // --- Bake collider data ---
        trunkData.colliderSize = _boxCollider.size;
        trunkData.colliderOffset = _boxCollider.offset;

        // --- Bake points data ---
        var points = GetPoints();
        trunkData.downNearPoint = points[0];
        trunkData.downFarPoint = points[1];
        trunkData.topNearPoint = points[2];
        trunkData.topFarPoint = points[3];
    }

    // --- IBuildable ---
    public void SetData(IData data)
    {
        if (data is not TrunkData trunkData) return;
        SetData(trunkData, Side.Right, false);
    }

    public void SetData(TrunkData data, Side side, bool isYFlipped)
    {
        int flipX = side == Side.Left ? -1 : 1;

        // --- Apply visual data ---
        _spriteRenderer.sprite = data.sprite;
        _spriteRenderer.flipX = side != Side.Right;
        _spriteRenderer.flipY = isYFlipped;
        _spriteRenderer.transform.localPosition = new Vector3(data.spriteOffset.x * flipX, data.spriteOffset.y, 0);

        // --- Apply outline ---
        _outlineRenderer.sprite = data.sprite;
        _outlineRenderer.flipX = side != Side.Right;
        _outlineRenderer.flipY = isYFlipped;

        // --- Apply collider data ---
        _boxCollider.size = data.colliderSize;
        _boxCollider.offset = new Vector2(data.colliderOffset.x * flipX, data.colliderOffset.y);

        // --- Apply points data ---
        Vector2 downNearPoint = data.downNearPoint;
        Vector2 downFarPoint = new Vector2(data.downFarPoint.x * flipX, data.downFarPoint.y);
        Vector2 topNearPoint = data.topNearPoint;
        Vector2 topFarPoint = new Vector2(data.topFarPoint.x * flipX, data.topFarPoint.y);

        if (isYFlipped)
        {
            float tmpX = downFarPoint.x;
            downFarPoint.x = topFarPoint.x;
            topFarPoint.x = tmpX;
        }

        SetPoints(downNearPoint, downFarPoint, topNearPoint, topFarPoint);
    }

    public void Clear()
    {
        // --- Reset sprites ---
        _spriteRenderer.sprite = null;
        _outlineRenderer.sprite = null;

        // --- Reset transforms ---
        _spriteRenderer.flipX = false;
        _spriteRenderer.flipY = false;
        _outlineRenderer.flipX = false;
        _outlineRenderer.flipY = false;
        _spriteRenderer.transform.localPosition = Vector3.zero;

        // --- Reset collider ---
        _boxCollider.size = Vector2.zero;
        _boxCollider.offset = Vector2.zero;

        // --- Reset points ---
        SetPoints(Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero);

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
        UnityEditor.EditorUtility.SetDirty(gameObject);
#endif
        gameObject.SetActive(false);
        Debug.Log("TrunkPart cleared to initial state");
    }

    /// <summary>
    /// Returns the coordinates of all snap points
    /// </summary>
    /// <returns>Array containing [downNear, downFar, topNear, topFar] points</returns>
    public Vector2[] GetPoints()
    {
        return new Vector2[] 
        { 
            _downNearPoint, 
            _downFarPoint, 
            _topNearPoint, 
            _topFarPoint 
        };
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
