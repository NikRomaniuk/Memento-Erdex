using UnityEditor;
using UnityEngine;

// IslandSlotDrawer handles conditional UI for IslandSlot anywhere it appears
// BranchDataEditor and default inspectors both rely on this drawer

[CustomPropertyDrawer(typeof(IslandSlot))]
public class IslandSlotDrawer : PropertyDrawer
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
        SerializedProperty staticIslandData = property.FindPropertyRelative("staticIslandData");

        // Foldout + isStatic + xPoint
        float height = line + space;
        height += line + space;
        height += line + space;

        if (isStatic.boolValue)
        {
            // IslandData field
            height += line + space;

            // Warning when IslandData is not assigned in static mode
            if (staticIslandData.objectReferenceValue == null)
            {
                height += EditorGUIUtility.singleLineHeight * 2f + space;
            }
        }
        else
        {
            // Tiny/Small/Medium toggles
            height += line + space;
            height += line + space;
            height += line + space;
        }

        return height;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Cache child properties once to keep drawing logic readable
        SerializedProperty isStatic = property.FindPropertyRelative("isStatic");
        SerializedProperty xPoint = property.FindPropertyRelative("xPoint");
        SerializedProperty staticIslandData = property.FindPropertyRelative("staticIslandData");
        SerializedProperty allowTiny = property.FindPropertyRelative("allowTiny");
        SerializedProperty allowSmall = property.FindPropertyRelative("allowSmall");
        SerializedProperty allowMedium = property.FindPropertyRelative("allowMedium");

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
        EditorGUI.PropertyField(row, xPoint, new GUIContent("X Point"));

        // Safety clamp in UI; data-level clamp also exists in BranchData.OnValidate
        xPoint.floatValue = Mathf.Max(0f, xPoint.floatValue);

        if (isStatic.boolValue)
        {
            // Static mode: user must assign a concrete IslandData asset
            row.y += line + space;
            EditorGUI.PropertyField(row, staticIslandData, new GUIContent("Island Data"));

            if (staticIslandData.objectReferenceValue == null)
            {
                row.y += line + space;
                Rect help = new Rect(row.x, row.y, row.width, EditorGUIUtility.singleLineHeight * 2f);
                EditorGUI.HelpBox(help, "Assign IslandData for static slot", MessageType.Warning);
            }
        }
        else
        {
            // Dynamic mode: choose allowed IslandData.Size values
            row.y += line + space;
            EditorGUI.PropertyField(row, allowTiny, new GUIContent(IslandData.Size.Tiny.ToString()));

            row.y += line + space;
            EditorGUI.PropertyField(row, allowSmall, new GUIContent(IslandData.Size.Small.ToString()));

            row.y += line + space;
            EditorGUI.PropertyField(row, allowMedium, new GUIContent(IslandData.Size.Medium.ToString()));
        }

        EditorGUI.indentLevel--;
    }
}