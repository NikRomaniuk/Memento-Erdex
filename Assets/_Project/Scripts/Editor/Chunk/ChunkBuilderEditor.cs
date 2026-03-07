using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ChunkBuilder))]
public class ChunkBuilderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ChunkBuilder builder = (ChunkBuilder)target;

        EditorGUILayout.Space(10);

        if (GUILayout.Button("Build from ScriptableObject", GUILayout.Height(30)))
        {
            builder.BuildFromScriptableObject();
        }

        EditorGUILayout.Space(5);

        // Clear button with warning color
        GUI.backgroundColor = new Color(1f, 0.6f, 0.6f); // Light red
        if (GUILayout.Button("Clear Chunk", GUILayout.Height(25)))
        {
            if (EditorUtility.DisplayDialog("Clear Chunk",
                "Are you sure you want to clear this Chunk? This will reset all data to default.",
                "Clear", "Cancel"))
            {
                builder.Clear();
            }
        }
        GUI.backgroundColor = Color.white;

        EditorGUILayout.Space(5);

        if (builder.DataToBuild != null)
        {
            EditorGUILayout.HelpBox($"Current Source: {builder.DataToBuild.name}", MessageType.Info);
        }
        else
        {
            EditorGUILayout.HelpBox("Assign a ChunkData ScriptableObject to build from!", MessageType.Warning);
        }
    }
}
