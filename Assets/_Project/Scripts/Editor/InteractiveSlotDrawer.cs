using UnityEditor;
using UnityEngine;

// InteractiveSlotDrawer handles conditional UI for InteractiveSlot anywhere it appears
[CustomPropertyDrawer(typeof(InteractiveSlot))]
public class InteractiveSlotDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float line = EditorGUIUtility.singleLineHeight;
        float space = EditorGUIUtility.standardVerticalSpacing;

        if (!property.isExpanded)
        {
            return line;
        }

        SerializedProperty isStatic = property.FindPropertyRelative("isStatic");
        SerializedProperty staticData = property.FindPropertyRelative("staticData");

        // Foldout + isStatic + pos
        float height = line + space;
        height += line + space;
        height += line + space;

        if (isStatic.boolValue)
        {
            // InteractiveData field
            height += line + space;

            // Warning when InteractiveData is not assigned in static mode
            if (staticData.objectReferenceValue == null)
            {
                height += EditorGUIUtility.singleLineHeight * 2f + space;
            }
        }

        return height;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Cache child properties once to keep drawing logic readable
        SerializedProperty isStatic = property.FindPropertyRelative("isStatic");
        SerializedProperty pos = property.FindPropertyRelative("pos");
        SerializedProperty staticData = property.FindPropertyRelative("staticData");

        float line = EditorGUIUtility.singleLineHeight;
        float space = EditorGUIUtility.standardVerticalSpacing;

        Rect row = new Rect(position.x, position.y, position.width, line);

        // Foldout controls whether slot internals are shown
        property.isExpanded = EditorGUI.Foldout(row, property.isExpanded, label, true);
        if (!property.isExpanded)
        {
            return;
        }

        EditorGUI.indentLevel++;

        row.y += line + space;
        EditorGUI.PropertyField(row, isStatic, new GUIContent("Is Static"));

        row.y += line + space;
        EditorGUI.PropertyField(row, pos, new GUIContent("Position"));

        if (isStatic.boolValue)
        {
            // Static mode: must assign a concrete InteractiveData asset
            row.y += line + space;
            EditorGUI.PropertyField(row, staticData, new GUIContent("Interactive Data"));

            if (staticData.objectReferenceValue == null)
            {
                row.y += line + space;
                Rect help = new Rect(row.x, row.y, row.width, EditorGUIUtility.singleLineHeight * 2f);
                EditorGUI.HelpBox(help, "Assign InteractiveData for static slot", MessageType.Warning);
            }
        }
        else
        {
            // Reserved for future dynamic mode fields when isStatic == false
        }

        EditorGUI.indentLevel--;
    }
}