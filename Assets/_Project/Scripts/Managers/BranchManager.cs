using UnityEngine;

public class BranchManager : MonoBehaviour, IBakeable, IBuildable
{
    [Header("Properties")]
    [SerializeField] private int _length;
    [SerializeField] private bool _drawRight = true;
    [SerializeField] private IslandSlot[] _islandSlots;

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
        SetData(branchData);
    }

    public void SetData(BranchData data)
    {
        _length = Mathf.Max(0, data.length); // Ensure length is non-negative
        _islandSlots = data.islandSlots != null ? (IslandSlot[])data.islandSlots.Clone() : new IslandSlot[0];
    }

    public void Clear()
    {
        _length = 0;
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
        if (_islandSlots == null || _islandSlots.Length == 0) return;

        const float TINY_WIDTH = 0.8f;
        const float iconWorldSize = 0.2f * 4;

        Texture2D tinyIcon = Resources.Load<Texture2D>("Gizmos/Icons/TinyIslandIcon");
        Texture2D smallIcon = Resources.Load<Texture2D>("Gizmos/Icons/SmallIslandIcon");
        Texture2D mediumIcon = Resources.Load<Texture2D>("Gizmos/Icons/MediumIslandIcon");

        Color tinyColor = new Color(0.73f, 1f, 0.15f, 0.28f);      // Lime
        Color smallColor = new Color(0.18f, 0.85f, 0.22f, 0.28f);    // Green
        Color mediumColor = new Color(0.64f, 0.87f, 0.2f, 0.28f);    // Green-yellow

        foreach (IslandSlot slot in _islandSlots)
        {
            if (slot.allowTiny)
                DrawSlotIcon(slot.xPoint, TINY_WIDTH, iconWorldSize, tinyColor, tinyIcon);

            if (slot.allowSmall)
                DrawSlotIcon(slot.xPoint, TINY_WIDTH * 2f, iconWorldSize, smallColor, smallIcon);

            if (slot.allowMedium)
                DrawSlotIcon(slot.xPoint, TINY_WIDTH * 3f, iconWorldSize, mediumColor, mediumIcon);
        }
    }

    private void DrawSlotIcon(float xPoint, float width, float iconWorldSize, Color iconColor, Texture2D icon)
    {
        // At drawRight=true xPoint is left-top corner; at false -xPoint is right-top corner.
        float iconLeftX = _drawRight ? xPoint : -xPoint - width;
        Vector3 iconTopLeftWorld = transform.TransformPoint(new Vector3(iconLeftX, 0f, 0f));
        DrawIconFromTopLeft(iconTopLeftWorld, icon, iconWorldSize, iconColor);
    }

    private void DrawIconFromTopLeft(Vector3 worldTopLeft, Texture2D icon, float worldSize, Color color)
    {
        if (icon == null) return;

        Camera cam = UnityEditor.SceneView.lastActiveSceneView?.camera;
        if (cam == null) return;

        float screenSize;
        if (cam.orthographic)
        {
            float screenHeight = cam.pixelHeight;
            float worldHeight = cam.orthographicSize * 2;
            float pixelsPerWorldUnit = screenHeight / worldHeight;
            screenSize = worldSize * pixelsPerWorldUnit;
        }
        else
        {
            float distance = Vector3.Distance(cam.transform.position, worldTopLeft);
            float frustumHeight = 2.0f * distance * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
            float pixelsPerWorldUnit = cam.pixelHeight / frustumHeight;
            screenSize = worldSize * pixelsPerWorldUnit;
        }

        // Keep the sprite's original aspect ratio to avoid squeezing rectangular icons.
        float aspect = icon.height > 0 ? (float)icon.width / icon.height : 1f;
        float screenWidth = screenSize * aspect;
        float rectHeight = screenSize;

        UnityEditor.Handles.BeginGUI();

        Vector2 screenPos = UnityEditor.HandleUtility.WorldToGUIPoint(worldTopLeft);
        Rect rect = new Rect(screenPos.x, screenPos.y, screenWidth, rectHeight);

        Color oldColor = GUI.color;
        GUI.color = color;
        GUI.DrawTexture(rect, icon);
        GUI.color = oldColor;

        UnityEditor.Handles.EndGUI();
    }
#endif
}
