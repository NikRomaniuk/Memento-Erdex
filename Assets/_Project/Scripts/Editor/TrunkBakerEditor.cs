using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TrunkBaker))]
public class TrunkBakerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TrunkBaker baker = (TrunkBaker)target;

        EditorGUILayout.Space(10);
        
        if (GUILayout.Button("Bake to ScriptableObject", GUILayout.Height(30)))
        {
            baker.BakeToScriptableObject();
        }

        if (baker.DataToEdit != null)
        {
            EditorGUILayout.HelpBox($"Current Target: {baker.DataToEdit.name}", MessageType.Info);
        }
        else
        {
            EditorGUILayout.HelpBox("Assign a TrunkData ScriptableObject to bake!", MessageType.Warning);
        }
    }
}
