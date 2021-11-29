using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RhythmManager))]
public class RhythmManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        //base.OnInspectorGUI();

        SerializedProperty easyPercentage = serializedObject.FindProperty("easyPercentage");
        SerializedProperty mediumPercentage = serializedObject.FindProperty("mediumPercentage");
        SerializedProperty hardPercentage = serializedObject.FindProperty("hardPercentage");

        //text above visual
        EditorGUI.LabelField( new Rect(20, 10, 100, 10), "Beat recieved");
        EditorGUI.LabelField(new Rect((Screen.width - 95) * hardPercentage.floatValue/100, 10, 50, 10), "Hard");
        EditorGUI.LabelField(new Rect((Screen.width - 95) * mediumPercentage.floatValue / 100, 10, 50, 10),"Medium");
        EditorGUI.LabelField(new Rect((Screen.width - 95) * easyPercentage.floatValue / 100, 10, 50, 15),"Easy");
        EditorGUI.LabelField(new Rect((Screen.width - 125) , 10, 100, 10),"Half beat");

        // Visuals
        EditorGUI.DrawRect(new Rect(20 , 30, Screen.width - 95, 30), Color.gray);
        EditorGUI.DrawRect(new Rect(20 , 30, (Screen.width - 95) * easyPercentage.floatValue /100 , 30), Color.green);
        EditorGUI.DrawRect(new Rect(20 , 30, (Screen.width - 95) * mediumPercentage.floatValue/100, 30), Color.yellow);
        EditorGUI.DrawRect(new Rect(20 , 30, (Screen.width - 95) * hardPercentage.floatValue/100, 30), Color.red);


        EditorGUILayout.Space( 60 );
        EditorGUILayout.LabelField("Set you values here");
        EditorGUILayout.LabelField("Easy Percentage :");

        easyPercentage.floatValue = EditorGUI.Slider(new Rect(20, 100, (Screen.width - 40), 20) , easyPercentage.floatValue, 1, 100);
        easyPercentage.floatValue = Mathf.Clamp(easyPercentage.floatValue, mediumPercentage.floatValue, 100);

        EditorGUILayout.Space(20);

        EditorGUILayout.LabelField("Medium Percentage :");
        mediumPercentage.floatValue = EditorGUI.Slider(new Rect(20, 140, (Screen.width - 40), 20), mediumPercentage.floatValue, 1, 100);
        mediumPercentage.floatValue = Mathf.Clamp(mediumPercentage.floatValue,  hardPercentage.floatValue, easyPercentage.floatValue);

        EditorGUILayout.Space(20);

        EditorGUILayout.LabelField("Hard Percentage :");
        hardPercentage.floatValue = EditorGUI.Slider(new Rect(20, 180, (Screen.width - 40), 20), hardPercentage.floatValue, 1, 100);
        hardPercentage.floatValue = Mathf.Clamp(hardPercentage.floatValue, 0 , mediumPercentage.floatValue);

        EditorGUILayout.Space(60);

        EditorGUI.DrawRect(new Rect(20, 220, Screen.width - 40, 20), Color.gray);
        EditorGUI.DrawRect(new Rect(20 + (Screen.width - 40) * (((100 - easyPercentage.floatValue) /2)/ 100), 220, (Screen.width - 40) * easyPercentage.floatValue / 100, 20), Color.green);
        EditorGUI.DrawRect(new Rect(20 + (Screen.width - 40) * (((100 - mediumPercentage.floatValue) / 2) / 100), 220, (Screen.width - 40) * mediumPercentage.floatValue / 100, 20), Color.yellow);
        EditorGUI.DrawRect(new Rect(20 + (Screen.width - 40) * (((100 - hardPercentage.floatValue) / 2) / 100), 220, (Screen.width - 40) * hardPercentage.floatValue / 100, 20), Color.red);
        EditorGUI.DrawRect(new Rect(20 + (Screen.width - 40)/ 2 - 2.5f, 210, 5, 40), Color.black);


        serializedObject.ApplyModifiedProperties();
    }
}
