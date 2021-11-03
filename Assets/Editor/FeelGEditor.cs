using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FeelGood))]
public class FeelGEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();


        #region FIND PROPERTIES 
        SerializedProperty timeToDo = serializedObject.FindProperty("timeToDo");
        SerializedProperty changeScale = serializedObject.FindProperty("changeScale");
        SerializedProperty xPourcent = serializedObject.FindProperty("xPourcentScale");
        SerializedProperty yPourcent = serializedObject.FindProperty("yPourcentScale");


        #endregion



        EditorGUILayout.PropertyField(timeToDo);
        EditorGUILayout.PropertyField(changeScale);

        if (changeScale.boolValue)
        {
        EditorGUILayout.PropertyField(xPourcent);
        EditorGUILayout.PropertyField(yPourcent);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
