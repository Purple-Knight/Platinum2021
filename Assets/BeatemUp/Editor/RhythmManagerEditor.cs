using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RhythmManager))]
public class RhythmManagerEditor : Editor
{
    public static string[] EXCLUDE_PROPERTIES = { "numberOfBeat" , "beatDuration", "InstantiateBeat", "EndOfMusic", "halfBeatTime" , "hardPercentage" , "mediumPercentage", "easyPercentage", "BPM"};

    float beatDuration;
        float inspectorOffset = 270;
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        //base.OnInspectorGUI();

        DrawPropertiesExcluding(serializedObject, EXCLUDE_PROPERTIES);
        SerializedProperty easyPercentage = serializedObject.FindProperty("easyPercentage");
        SerializedProperty mediumPercentage = serializedObject.FindProperty("mediumPercentage");
        SerializedProperty hardPercentage = serializedObject.FindProperty("hardPercentage");
        SerializedProperty BPM = serializedObject.FindProperty("BPM");

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("BPM : "); // visualise in seconds the time lapse with BPM
        BPM.floatValue = EditorGUILayout.FloatField(BPM.floatValue);
        EditorGUILayout.EndHorizontal();

        BPM.floatValue = Mathf.Clamp(BPM.floatValue, 1, 400);
        beatDuration = 60/BPM.floatValue;


        inspectorOffset = GUILayoutUtility.GetLastRect().position.y +20; 
        //text above visual
        EditorGUI.LabelField( new Rect(20, inspectorOffset, 100, 10), "Beat recieved");
        EditorGUI.LabelField(new Rect((Screen.width - 95) * hardPercentage.floatValue/100, 10 + inspectorOffset, 50, 10), "Hard");
        EditorGUI.LabelField(new Rect((Screen.width - 95) * mediumPercentage.floatValue / 100, 10+ inspectorOffset, 50, 10),"Medium");
        EditorGUI.LabelField(new Rect((Screen.width - 95) * easyPercentage.floatValue / 100, 10 + inspectorOffset, 50, 15),"Easy");
        EditorGUI.LabelField(new Rect((Screen.width - 125) , inspectorOffset, 100, 10),"Half beat");
        

        // Visuals color rectangles, half beat
        EditorGUI.DrawRect(new Rect(20 , 30 + inspectorOffset, Screen.width - 95, 30), Color.gray);
        EditorGUI.DrawRect(new Rect(20 , 30 + inspectorOffset, (Screen.width - 95) * easyPercentage.floatValue /100 , 30), Color.green);
        EditorGUI.DrawRect(new Rect(20 , 30 + inspectorOffset, (Screen.width - 95) * mediumPercentage.floatValue/100, 30), Color.yellow);
        EditorGUI.DrawRect(new Rect(20 , 30 + inspectorOffset, (Screen.width - 95) * hardPercentage.floatValue/100, 30), Color.red);

        //seconds of each difficulties 
        EditorGUI.LabelField(new Rect((Screen.width - 95) * hardPercentage.floatValue/100, 70 + inspectorOffset, 100, 10), (beatDuration * hardPercentage.floatValue /100 ).ToString() + "s");
        EditorGUI.LabelField(new Rect((Screen.width - 95) * mediumPercentage.floatValue / 100, 70+ inspectorOffset, 100, 10), (beatDuration * mediumPercentage.floatValue / 100).ToString() + "s");
        EditorGUI.LabelField(new Rect((Screen.width - 95) * easyPercentage.floatValue / 100, 70 + inspectorOffset, 100, 15), (beatDuration * easyPercentage.floatValue / 100).ToString() + "s");
        EditorGUI.LabelField(new Rect((Screen.width - 100) , 70 + inspectorOffset, 100, 10),beatDuration.ToString() +"s");

        EditorGUILayout.Space( 80 );
        EditorGUILayout.LabelField("Set your values here");
        EditorGUILayout.LabelField("Easy Percentage :");

        //Sliders for each level
        easyPercentage.floatValue = EditorGUI.Slider(new Rect(20, 120+ inspectorOffset, (Screen.width - 40), 20) , easyPercentage.floatValue, 1, 100);
        easyPercentage.floatValue = Mathf.Clamp(easyPercentage.floatValue, mediumPercentage.floatValue, 100);

        EditorGUILayout.Space(20);

        EditorGUILayout.LabelField("Medium Percentage :");
        mediumPercentage.floatValue = EditorGUI.Slider(new Rect(20, 160 + inspectorOffset, (Screen.width - 40), 20), mediumPercentage.floatValue, 1, 100);
        mediumPercentage.floatValue = Mathf.Clamp(mediumPercentage.floatValue,  hardPercentage.floatValue, easyPercentage.floatValue);

        EditorGUILayout.Space(20);

        EditorGUILayout.LabelField("Hard Percentage :");
        hardPercentage.floatValue = EditorGUI.Slider(new Rect(20, 200 + inspectorOffset, (Screen.width - 40), 20), hardPercentage.floatValue, 1, 100);
        hardPercentage.floatValue = Mathf.Clamp(hardPercentage.floatValue, 0 , mediumPercentage.floatValue);

        EditorGUILayout.Space(60);

        //visual for an entire beat time
        EditorGUI.DrawRect(new Rect(20, 240 + inspectorOffset, Screen.width - 40, 20), Color.gray);
        EditorGUI.DrawRect(new Rect(20 + (Screen.width - 40) * (((100 - easyPercentage.floatValue) /2)/ 100), 240 + inspectorOffset, (Screen.width - 40) * easyPercentage.floatValue / 100, 20), Color.green);
        EditorGUI.DrawRect(new Rect(20 + (Screen.width - 40) * (((100 - mediumPercentage.floatValue) / 2) / 100), 240 + inspectorOffset, (Screen.width - 40) * mediumPercentage.floatValue / 100, 20), Color.yellow);
        EditorGUI.DrawRect(new Rect(20 + (Screen.width - 40) * (((100 - hardPercentage.floatValue) / 2) / 100), 240 + inspectorOffset, (Screen.width - 40) * hardPercentage.floatValue / 100, 20), Color.red);
        EditorGUI.DrawRect(new Rect(20 + (Screen.width - 40)/ 2 - 2.5f, 230 + inspectorOffset, 5, 40), Color.black);


        serializedObject.ApplyModifiedProperties();
    }
}
