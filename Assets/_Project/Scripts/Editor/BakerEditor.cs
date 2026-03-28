using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Baker))]
public class BakerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // --- Always visible ---
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_blankType"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_id"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_dataToEdit"));

        EditorGUILayout.Space(4);

        // --- Type-specific ---
        Baker baker = (Baker)target;
        switch (baker.BlankType)
        {
            case BlanksLibrary.BlankType.Trunk:
                EditorGUILayout.LabelField("Trunk Options", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_avaliableSide"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_canBeFlippedVertically"));
                break;

            case BlanksLibrary.BlankType.Branch:
                EditorGUILayout.LabelField("Branch Options", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_branchAvaliableSide"));
                break;
        }

        serializedObject.ApplyModifiedProperties();

        // --- Bake button ---
        EditorGUILayout.Space(10);

        if (GUILayout.Button("Bake to ScriptableObject", GUILayout.Height(30)))
        {
            baker.BakeToScriptableObject();
        }

        // --- Target info ---
        if (baker.DataToEdit != null)
        {
            EditorGUILayout.HelpBox($"Current Target: {baker.DataToEdit.name}", MessageType.Info);
        }
        else
        {
            EditorGUILayout.HelpBox("Assign a ScriptableObject to bake into!", MessageType.Warning);
        }
    }
}
