using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlayerWeapon))]
public class WeaponEditor : Editor
{
    public override void OnInspectorGUI()
    {
        /*serializedObject.Update();

        SerializedProperty bulletList = serializedObject.FindProperty("bullets");*/

        base.OnInspectorGUI();

        /*foreach (BulletInfo bullet in bulletList)
        {
            EditorGUILayout.PropertyField(bullet);
        }

        serializedObject.ApplyModifiedProperties();*/
    }
}
