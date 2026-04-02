using System.Collections.Generic;
using UnityEngine;

public class BranchManager : MonoBehaviour, IBakeable, IBuildable
{
    [Header("Properties")]
    [SerializeField] private int _length;
    [SerializeField] private IslandSlot[] _islandSlots;

    [Header("Draw Settings")]
    [SerializeField] private bool _drawRight = true;

    [HideInInspector] public List<ShapeManager> LoadedShapes = new List<ShapeManager>(); // Active ShapeManagers loaded for this branch

    // --- IBakeable ---
    public void GatherData(IData data)
    {
        if (data is not BranchData branchData) return;

        branchData.length = Mathf.Max(0, _length); // Ensure length is non-negative
        branchData.islandSlots = _islandSlots != null ? (IslandSlot[])_islandSlots.Clone() : new IslandSlot[0];
    }

    // --- IBuildable ---
    public void SetData(IData data)
    {
        if (data is not BranchData branchData) return;
        SetData(branchData, Orientation.Right, transform.position.x); // Build with default values
    }

    public void SetData(BranchData data, Orientation orient, float currentHeight)
    {
        _length = Mathf.Max(0, data.length); // Ensure length is non-negative

        float xPos;
        // Set up xPos based on branch orientation
        switch (orient)
        {
            default:
            case Orientation.Right:
                xPos = Constants.BRANCH_SLOT_X_OFFSET;
                break;

            case Orientation.Left:
                xPos = -Constants.BRANCH_SLOT_X_OFFSET;
                break;

            case Orientation.Middle:
                xPos = 0f;
                break;
        }

        // --- Apply Transform ---
        // Set up Y position based on current height
        // Set up X position based on branch orientation
        transform.position = new Vector3(xPos, currentHeight, transform.position.z);

        // --- Apply Slots ---
        // Copy island slots array
        _islandSlots = data.islandSlots != null ? (IslandSlot[])data.islandSlots.Clone() : new IslandSlot[0];
    }

    public void Clear()
    {
        // --- Reset General ---
        _length = 0;

        // --- Reset Transform ---
        transform.position = Vector3.zero;

        // --- Reset Slots ---
        _islandSlots = new IslandSlot[0];

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
        UnityEditor.EditorUtility.SetDirty(gameObject);
#endif
        gameObject.SetActive(false);
        Debug.Log("BranchManager cleared to initial state");
    }

    public IslandSlot[] IslandSlots => _islandSlots;

    private void OnValidate()
    {
        _length = Mathf.Max(0, _length);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        const float islandIconWorldSize = 0.2f * 4f;
        const float branchIconWorldSize = 0.2f;

        Texture2D tinyIcon = Resources.Load<Texture2D>("Gizmos/Icons/TinyIslandIcon");
        Texture2D smallIcon = Resources.Load<Texture2D>("Gizmos/Icons/SmallIslandIcon");
        Texture2D mediumIcon = Resources.Load<Texture2D>("Gizmos/Icons/MediumIslandIcon");
        Texture2D branchStartIcon = Resources.Load<Texture2D>("Gizmos/Icons/StartBranch");
        Texture2D branchEndIcon = Resources.Load<Texture2D>("Gizmos/Icons/EndBranch");

        Color tinyColor = new Color(0.73f, 1f, 0.15f, 0.28f);      // Lime
        Color smallColor = new Color(0.18f, 0.85f, 0.22f, 0.28f);  // Green
        Color mediumColor = new Color(0.64f, 0.87f, 0.2f, 0.28f);  // Green-yellow
        Color branchStartColor = new Color(1f, 0.55f, 0.15f, 0.9f); // Orange
        Color branchEndColor = new Color(1f, 0.9f, 0.2f, 0.9f);     // Yellow

        DrawBranchIcons(branchIconWorldSize, branchStartColor, branchStartIcon, branchEndColor, branchEndIcon);

        if (_islandSlots == null || _islandSlots.Length == 0) return;

        foreach (IslandSlot slot in _islandSlots)
        {
            if (slot.isStatic)
            {
                if (slot.staticIslandData == null)
                {
                    continue;
                }

                switch (slot.staticIslandData.size)
                {
                    case Size.Tiny:
                        DrawSlotIcon(slot.xPoint, islandIconWorldSize, tinyColor, tinyIcon);
                        break;

                    case Size.Small:
                        DrawSlotIcon(slot.xPoint, islandIconWorldSize, smallColor, smallIcon);
                        break;

                    case Size.Medium:
                        DrawSlotIcon(slot.xPoint, islandIconWorldSize, mediumColor, mediumIcon);
                        break;
                }

                continue;
            }

            if (slot.allowTiny)
                DrawSlotIcon(slot.xPoint, islandIconWorldSize, tinyColor, tinyIcon);

            if (slot.allowSmall)
                DrawSlotIcon(slot.xPoint, islandIconWorldSize, smallColor, smallIcon);

            if (slot.allowMedium)
                DrawSlotIcon(slot.xPoint, islandIconWorldSize, mediumColor, mediumIcon);
        }
    }

    private void DrawBranchIcons(float iconWorldSize, Color startColor, Texture2D startIcon, Color endColor, Texture2D endIcon)
    {
        // Branch start is at local X=0. Branch end is offset by signed branch length
        float startX = 0f;
        float signedLength = _drawRight ? _length : -_length;
        float endX = startX + signedLength * 0.8f;

        DrawIcon(startX, 0f, iconWorldSize, startColor, startIcon);
        DrawIcon(endX, 0f, iconWorldSize, endColor, endIcon);
    }

    private void DrawSlotIcon(float xPoint, float iconWorldSize, Color iconColor, Texture2D icon)
    {
        // xPoint now represents slot center. Mirror by inverting x only.
        float iconCenterX = xPoint;

        if (!_drawRight)
            iconCenterX = -iconCenterX;

        // Shift icon down by half its height so slot point matches top-center anchor
        DrawIcon(iconCenterX, -Constants.UNIT_SIZE/2f, iconWorldSize, iconColor, icon);
    }

    private void DrawIcon(float localX, float localY, float worldSize, Color color, Texture2D icon)
    {
        if (icon == null) return;

        Vector3 worldPoint = transform.TransformPoint(new Vector3(localX, localY, 0f));
        Camera cam = UnityEditor.SceneView.lastActiveSceneView?.camera;
        if (cam == null) return;

        float screenSize = ComputeScreenSize(cam, worldPoint, worldSize);

        // Keep the sprite's original aspect ratio to avoid squeezing rectangular icons
        float aspect = icon.height > 0 ? (float)icon.width / icon.height : 1f;
        float screenWidth = screenSize * aspect;
        float rectHeight = screenSize;

        UnityEditor.Handles.BeginGUI();

        Vector2 screenPos = UnityEditor.HandleUtility.WorldToGUIPoint(worldPoint);
        float rectX = screenPos.x - screenWidth * 0.5f;
        float rectY = screenPos.y - rectHeight * 0.5f;
        Rect rect = new Rect(rectX, rectY, screenWidth, rectHeight);

        Color oldColor = GUI.color;
        GUI.color = color;
        GUI.DrawTexture(rect, icon);
        GUI.color = oldColor;

        UnityEditor.Handles.EndGUI();
    }

    private float ComputeScreenSize(Camera cam, Vector3 worldPoint, float worldSize)
    {
        if (cam.orthographic)
        {
            float screenHeight = cam.pixelHeight;
            float worldHeight = cam.orthographicSize * 2;
            float pixelsPerWorldUnit = screenHeight / worldHeight;
            return worldSize * pixelsPerWorldUnit;
        }

        float distance = Vector3.Distance(cam.transform.position, worldPoint);
        float frustumHeight = 2.0f * distance * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
        float pixelsPerWorldUnitPerspective = cam.pixelHeight / frustumHeight;
        return worldSize * pixelsPerWorldUnitPerspective;
    }
#endif
}
