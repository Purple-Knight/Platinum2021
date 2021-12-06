using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class WeaponLibraryEditor : EditorWindow
{
    string _dataPath = "";
    bool button1 = true;

    Object WLprefab;
    WeaponLibrary WL;
    bool validLibrary = false;

    [MenuItem("Tools/Weapon Library")]
    private static void OpenWindow()
    {
        GetWindow<WeaponLibraryEditor>("Weapon Library Editor");
    }

    void OnGUI()
    {
        //EditorGUILayout.TextField("Path", Application.persistentDataPath);

        /*if (button1)
        {
            if(GUILayout.Button("1"))
            {
                button1 = false;
            }
        }
        else
        {
            if(GUILayout.Button("2"))
            {
                button1 = true;
            }
        }*/

        WLprefab = EditorGUILayout.ObjectField("Weapon Library", WLprefab, typeof(GameObject), true);

        validLibrary = (WLprefab != null && ((GameObject)WLprefab).TryGetComponent<WeaponLibrary>(out WL)); // ? true : false;

        if(validLibrary)
        {
            GUILayout.Space(30);

            SerializedObject serializedObject = new SerializedObject(WL);
            SerializedProperty libraryValues = serializedObject.FindProperty("valueList");
            //EditorGUILayout.PropertyField(libraryValues, true);

            for (int i = 0; i < libraryValues.arraySize; i++)
            {
                Weapon w;
                SerializedProperty weaponProp = libraryValues.GetArrayElementAtIndex(i);
                GUILayout.Label(weaponProp.ToString());
            }
        }
        else
        {
            GUILayout.Space(10);
            GUILayout.Label("   INVALID LIBRARY !  USE WEAPON LIBRARY", EditorStyles.boldLabel);
        }
    }

    [MenuItem("Tools/Weapon Library/Command &d")]
    private static void SpecialCommand()
    {
        Debug.Log("Input Ctrl+P");
    }
}
