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

        SerializedProperty action = serializedObject.FindProperty("playOnAwake");
        SerializedProperty timeToDo = serializedObject.FindProperty("timeToDo");

        SerializedProperty lenum = serializedObject.FindProperty("evenOrOdd"); 

        SerializedProperty changePos = serializedObject.FindProperty("changePos");
        SerializedProperty posPourcent = serializedObject.FindProperty("posNeed");

        SerializedProperty scalePourcent = serializedObject.FindProperty("scaleNeed");
        SerializedProperty changeScale = serializedObject.FindProperty("changeScale");

        #endregion



        EditorGUILayout.PropertyField(action);
        EditorGUILayout.PropertyField(timeToDo);

        EditorGUILayout.PropertyField(lenum);

        EditorGUILayout.PropertyField(changePos);
        if (changePos.boolValue)
        {
            EditorGUILayout.PropertyField(posPourcent);
        }

        EditorGUILayout.PropertyField(changeScale);
        if (changeScale.boolValue)
        {
            EditorGUILayout.PropertyField(scalePourcent);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
