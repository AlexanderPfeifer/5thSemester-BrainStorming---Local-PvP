using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(DisplayColorAttribute))]
public class DisplayColorDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        DisplayColorAttribute displayColorAbove = (DisplayColorAttribute)attribute;

        // Set color of the input field
        Color inputColor = displayColorAbove.color;

        // Set up the style for the input field
        GUIStyle style = new GUIStyle(GUI.skin.textField);
        style.normal.background = Texture2D.whiteTexture; // Ensure background is visible
        style.normal.textColor = Color.black; // Text color

        // Set the background color of the input field
        Color previousColor = GUI.backgroundColor; // Save the current background color
        GUI.backgroundColor = inputColor; // Apply the specified color to the background

        // Draw the property field
        Rect propertyRect = new Rect(position.x, position.y, position.width, position.height);
        EditorGUI.PropertyField(propertyRect, property, label, true);

        GUI.backgroundColor = previousColor; // Reset the background color to its original state
    }
}
