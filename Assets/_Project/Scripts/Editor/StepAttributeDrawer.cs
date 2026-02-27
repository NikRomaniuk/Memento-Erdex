using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(StepAttribute))]
public class StepAttributeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        StepAttribute stepAttribute = (StepAttribute)attribute;
        float step = stepAttribute.Step;

        EditorGUI.BeginChangeCheck();
        
        if (property.propertyType == SerializedPropertyType.Vector2)
        {
            Vector2 value = EditorGUI.Vector2Field(position, label, property.vector2Value);
            
            if (EditorGUI.EndChangeCheck())
            {
                value.x = Mathf.Round(value.x / step) * step;
                value.y = Mathf.Round(value.y / step) * step;
                property.vector2Value = value;
            }
        }
        else if (property.propertyType == SerializedPropertyType.Float)
        {
            float value = EditorGUI.FloatField(position, label, property.floatValue);
            
            if (EditorGUI.EndChangeCheck())
            {
                value = Mathf.Round(value / step) * step;
                property.floatValue = value;
            }
        }
        else
        {
            EditorGUI.EndChangeCheck();
            EditorGUI.PropertyField(position, property, label);
        }
    }
}
