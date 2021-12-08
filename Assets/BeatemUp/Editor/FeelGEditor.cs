using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
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

        SerializedProperty changeScale = serializedObject.FindProperty("changeScale");
        SerializedProperty scalePourcent = serializedObject.FindProperty("scaleNeed");

        SerializedProperty colorChange = serializedObject.FindProperty("changeColor");
        SerializedProperty colorPourcent = serializedObject.FindProperty("colorNeed");

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

        EditorGUILayout.PropertyField(colorChange);
        if (colorChange.boolValue)
        {
            EditorGUILayout.PropertyField(colorPourcent);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
