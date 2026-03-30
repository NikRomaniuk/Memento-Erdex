using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Builder))]
public class BuilderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // --- Always visible ---
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_blankType"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_dataToBuild"));

        EditorGUILayout.Space(4);

        // --- Type-specific fields ---
        Builder builder = (Builder)target;
        switch (builder.BlankType)
        {
            case BlanksLibrary.BlankType.Trunk:
                EditorGUILayout.LabelField("Trunk Options", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_side"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_isYFlipped"));
                break;

            case BlanksLibrary.BlankType.Chunk:
                EditorGUILayout.LabelField("Chunk Options", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_currentHeight"));
                break;

            case BlanksLibrary.BlankType.Branch:
                EditorGUILayout.LabelField("Branch Options", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_branchOrientation"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_currentHeight"));
                break;
        }

        serializedObject.ApplyModifiedProperties();

        // --- Build button ---
        EditorGUILayout.Space(10);

        if (GUILayout.Button("Build from ScriptableObject", GUILayout.Height(30)))
        {
            builder.BuildFromScriptableObject();
        }

        EditorGUILayout.Space(5);

        // --- Clear button ---
        string clearLabel = builder.BlankType switch
        {
            BlanksLibrary.BlankType.Trunk => "Clear TrunkPart",
            BlanksLibrary.BlankType.Branch => "Clear Branch",
            _ => "Clear Chunk"
        };
        GUI.backgroundColor = new Color(1f, 0.6f, 0.6f);
        if (GUILayout.Button(clearLabel, GUILayout.Height(25)))
        {
            if (EditorUtility.DisplayDialog(clearLabel,
                $"Are you sure you want to {clearLabel.ToLower()}? This will reset all data to default.",
                "Clear", "Cancel"))
            {
                builder.Clear();
            }
        }
        GUI.backgroundColor = Color.white;

        EditorGUILayout.Space(5);

        // --- Source info ---
        if (builder.DataToBuild != null)
        {
            EditorGUILayout.HelpBox($"Current Source: {builder.DataToBuild.name}", MessageType.Info);
        }
        else
        {
            EditorGUILayout.HelpBox("Assign a ScriptableObject to build from!", MessageType.Warning);
        }
    }
}
