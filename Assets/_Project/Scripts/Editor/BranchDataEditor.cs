using UnityEditor;
using UnityEngine;

// BranchDataEditor owns the top-level BranchData inspector
// IslandSlot element rendering is delegated to IslandSlotDrawer via PropertyField(..., true)

[CustomEditor(typeof(BranchData))]
public class BranchDataEditor : Editor
{
    private SerializedProperty _id;
    private SerializedProperty _avaliableSide;
    private SerializedProperty _length;
    private SerializedProperty _islandSlots;

    private void OnEnable()
    {
        _id = serializedObject.FindProperty("id");
        _avaliableSide = serializedObject.FindProperty("avaliableSide");
        _length = serializedObject.FindProperty("length");
        _islandSlots = serializedObject.FindProperty("islandSlots");
    }

    public override void OnInspectorGUI()
    {
        // Sync serialized state before drawing controls
        serializedObject.Update();

        // Draw always-visible BranchData fields
        EditorGUILayout.PropertyField(_id);
        EditorGUILayout.PropertyField(_avaliableSide);
        EditorGUILayout.PropertyField(_length);
        EditorGUILayout.Space(6);

        if (_islandSlots == null)
        {
            EditorGUILayout.HelpBox("Property 'islandSlots' not found", MessageType.Error);
        }
        else
        {
            // Draw array recursively; each IslandSlot is drawn by IslandSlotDrawer
            EditorGUILayout.PropertyField(_islandSlots, true);
        }

        // Apply changes from inspector back to serialized object
        serializedObject.ApplyModifiedProperties();
    }
}
