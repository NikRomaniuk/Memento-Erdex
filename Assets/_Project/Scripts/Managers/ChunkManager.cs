using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour, IBakeable, IBuildable
{
    [Header("Properties")]
    [Step(0.05f)] [SerializeField] private float _height = 0f;
    [SerializeField] private BranchSlot[] _branchSlots;
    private const float BRANCH_SLOT_X_OFFSET = 0.5f;

    // --- Runtime ---
    [HideInInspector] public List<TrunkSegment> LoadedTrunks = new List<TrunkSegment>(); // Active TrunkSegments loaded for this chunk

    // --- IBakeable ---
    public void GatherData(IData data)
    {
        if (data is not ChunkData chunkData) return;

        chunkData.height = _height;
        chunkData.branchSlots = _branchSlots != null ? (BranchSlot[])_branchSlots.Clone() : new BranchSlot[0];
    }

    // --- IBuildable ---

    /// <summary>
    /// Set up private fields with given data
    /// </summary>
    public void SetData(IData data)
    {
        if (data is not ChunkData chunkData) return;
        SetData(chunkData, 0);
    }
    public void SetData(ChunkData data, float currentHeight)
    {
        // --- Apply General ---
        _height = data.height;

        // --- Apply Transform ---
        // Set up Y position based on current height
        transform.position = new Vector3(transform.position.x, currentHeight, transform.position.z);
        

        // --- Apply Slots ---
        // Copy branch slots array
        _branchSlots = data.branchSlots != null ? (BranchSlot[])data.branchSlots.Clone() : new BranchSlot[0];
    }
    
    public void Clear()
    {
        // --- Reset General ---
        _height = 0f;

        // --- Reset Transform ---
        transform.position = Vector3.zero;
        
        // --- Reset Slots ---
        _branchSlots = new BranchSlot[0];

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
        UnityEditor.EditorUtility.SetDirty(gameObject);
#endif
        gameObject.SetActive(false);
        Debug.Log("ChunkManager cleared to initial state");
    }

    // --- Public accessors ---
    public float Height => _height;
    public BranchSlot[] BranchSlots => _branchSlots;

#if UNITY_EDITOR
    // Draw points in Editor
    private void OnDrawGizmosSelected()
    {
        // Private constants for gizmos
        const float iconWorldSize = 0.2f; // Size in world units
        Color branchColorRight = new Color(1f, 0.95f, 0.3f); // Banana
        Color branchColorLeft = new Color(1f, 0.45f, 0.35f); // Terracotta
        Color branchColorMiddle = new Color(1f, 0.6f, 0.35f); // Orange
        
        // Icon path (Path starts from Resources folder)
        const string branchIconPath = "Gizmos/Icons/BranchSlot";
        
        // Load icon
        Texture2D branchIcon = Resources.Load<Texture2D>(branchIconPath);

        // Draw branch slots
        foreach (BranchSlot slot in _branchSlots)
        {
            float offsetX = 0f;
            Color slotColor = branchColorMiddle;

            // Set up variables based on branch slot orientation
            switch (slot.branchOrientation)
            {
                case BranchOrientation.Right:
                    offsetX = BRANCH_SLOT_X_OFFSET;
                    slotColor = branchColorRight;
                    break;
                case BranchOrientation.Left:
                    offsetX = -BRANCH_SLOT_X_OFFSET;
                    slotColor = branchColorLeft;
                    break;
                case BranchOrientation.Middle:
                    // Skip
                    break;
                default:
                    // This is impossible to happen :)
                    break;
            }

            Vector3 worldPosition = transform.TransformPoint(new Vector2(transform.position.x + offsetX, slot.yPoint));
            DrawIcon(worldPosition, branchIcon, iconWorldSize, slotColor);
        }
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


