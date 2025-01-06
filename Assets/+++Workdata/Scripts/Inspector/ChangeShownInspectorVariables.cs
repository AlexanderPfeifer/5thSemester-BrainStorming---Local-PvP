using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(Health))]
public class Health_Editor : Editor
{
    //I created the script because I want to hide variables depending on if the bool IsPlayer is enabled or disabled
    SerializedProperty isPlayerProperty;

    private void OnEnable()
    {
        isPlayerProperty = serializedObject.FindProperty("IsPlayer");
    }

    public override void OnInspectorGUI()
    {
        //Added these lines of code because the script icon at the top to quick access it was hidden
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((Health)target), typeof(Health), false);
        EditorGUI.EndDisabledGroup();



        SerializedProperty property = serializedObject.GetIterator();
        property.NextVisible(true);

        while (property.NextVisible(false))
        {
            if (property.name == "graveSprite" && isPlayerProperty.boolValue)
                continue;

            EditorGUILayout.PropertyField(property, true);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif