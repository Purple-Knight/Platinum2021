using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(PlayerWeapon))]
public class PlayerWeaponEditor : Editor
{

    public override void OnInspectorGUI()
    {

        //Update serializedObject to get th lest values
        serializedObject.Update();

        #region FindProperty
        //Find all the property 
        SerializedProperty weapon = serializedObject.FindProperty("weapon");

        SerializedProperty onDebug = serializedObject.FindProperty("debug");
        SerializedProperty newWeapon = serializedObject.FindProperty("newWeaponTarget");
        SerializedProperty debugGUI = serializedObject.FindProperty("debugGUI");
        SerializedProperty guiDebugArea = serializedObject.FindProperty("guiDebugArea");
        #endregion

        EditorGUILayout.PropertyField(weapon);

        

        EditorGUILayout.PropertyField(onDebug);

        if (onDebug.boolValue)
        {
            EditorGUILayout.PropertyField(newWeapon);
            EditorGUILayout.PropertyField(debugGUI);
            if (debugGUI.boolValue)
            {
                EditorGUILayout.PropertyField(guiDebugArea);
            }
        }
        
        //Save the serializedObject
        serializedObject.ApplyModifiedProperties();
    }
}
