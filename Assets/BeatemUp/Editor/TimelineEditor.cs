using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Timeline))]
public class TimelineEditor : Editor
{
    public static string[] excludedProperties = { "guiDebugArea" };

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawPropertiesExcluding(serializedObject, excludedProperties);

        #region FindProperty
        SerializedProperty boolDebug = serializedObject.FindProperty("guiDebug");
        SerializedProperty windowPos = serializedObject.FindProperty("guiDebugArea");
        #endregion

        if (boolDebug.boolValue)
        {
            EditorGUILayout.PropertyField(windowPos);
        }

        serializedObject.ApplyModifiedProperties();
       
    }
}
