using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Feel))]
public class FlGEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        #region FIND PROPERTIES 

        SerializedProperty toActivate = serializedObject.FindProperty("launch");
        SerializedProperty timeToDo = serializedObject.FindProperty("timeToDo");

        SerializedProperty changePos = serializedObject.FindProperty("changePos");
        SerializedProperty posPourcent = serializedObject.FindProperty("posNeed");
        SerializedProperty posPourcentB = serializedObject.FindProperty("posNeedBack");

        SerializedProperty changeScale = serializedObject.FindProperty("changeScale");
        SerializedProperty scalePourcent = serializedObject.FindProperty("scaleNeed");
        SerializedProperty scalePourcentB = serializedObject.FindProperty("scaleNeedBack");

        #endregion



        EditorGUILayout.PropertyField(toActivate);
        EditorGUILayout.PropertyField(timeToDo);

        EditorGUILayout.PropertyField(changePos);
        if (changePos.boolValue)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("------------- Aller -------------");
            EditorGUILayout.PropertyField(posPourcent);
            EditorGUILayout.LabelField("------------- Retour -------------");
            EditorGUILayout.PropertyField(posPourcentB);
            EditorGUILayout.Space();
        }

        EditorGUILayout.PropertyField(changeScale);
        if (changeScale.boolValue)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("------------- Aller -------------");
            EditorGUILayout.PropertyField(scalePourcent);
            EditorGUILayout.LabelField("------------- Retour -------------");
            EditorGUILayout.PropertyField(scalePourcentB);
            EditorGUILayout.Space();
        }

        serializedObject.ApplyModifiedProperties();
    }
     
}
