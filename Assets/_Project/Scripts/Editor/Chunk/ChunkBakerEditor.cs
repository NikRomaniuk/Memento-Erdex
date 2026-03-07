using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ChunkBaker))]
public class ChunkBakerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ChunkBaker baker = (ChunkBaker)target;

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
            EditorGUILayout.HelpBox("Assign a ChunkData ScriptableObject to bake into!", MessageType.Warning);
        }
    }
}
