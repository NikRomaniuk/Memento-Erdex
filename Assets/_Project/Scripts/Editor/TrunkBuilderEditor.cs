using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TrunkBuilder))]
public class TrunkBuilderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TrunkBuilder builder = (TrunkBuilder)target;

        EditorGUILayout.Space(10);
        
        if (GUILayout.Button("Build from ScriptableObject", GUILayout.Height(30)))
        {
            builder.BuildFromScriptableObject();
        }

        EditorGUILayout.Space(5);

        // Clear button with warning color
        GUI.backgroundColor = new Color(1f, 0.6f, 0.6f); // Light red
        if (GUILayout.Button("Clear TrunkPart", GUILayout.Height(25)))
        {
            if (EditorUtility.DisplayDialog("Clear TrunkPart",
                "Are you sure you want to clear this TrunkPart? This will reset all data to default.",
                "Clear", "Cancel"))
            {
                builder.Clear();
            }
        }
        GUI.backgroundColor = Color.white;

        EditorGUILayout.Space(5);

        if (builder.DataToBuild != null)
        {
            EditorGUILayout.HelpBox($"Current Source: {builder.DataToBuild.name} (ID: {builder.DataToBuild.id})", MessageType.Info);
        }
        else
        {
            EditorGUILayout.HelpBox("Assign a TrunkData ScriptableObject to build from!", MessageType.Warning);
        }
    }
}
