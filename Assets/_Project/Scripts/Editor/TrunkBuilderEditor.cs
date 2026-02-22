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
        
        if (GUILayout.Button("Bake to ScriptableObject", GUILayout.Height(30)))
        {
            builder.BakeToScriptableObject();
        }

        if (builder.DataToEdit != null)
        {
            EditorGUILayout.HelpBox($"Current Target: {builder.DataToEdit.name}", MessageType.Info);
        }
        else
        {
            EditorGUILayout.HelpBox("Assign a TrunkData ScriptableObject to bake!", MessageType.Warning);
        }
    }
}
