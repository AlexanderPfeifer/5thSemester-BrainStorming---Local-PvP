using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(DisplayColorAttribute))]
public class DisplayColorDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GUI.backgroundColor = ((DisplayColorAttribute)attribute).Color;

        EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, position.height), property, label, true);
    }
}
