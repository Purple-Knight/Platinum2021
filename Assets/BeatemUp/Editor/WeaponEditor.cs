using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Weapon_old))]
public class WeaponEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update(); // Update last values

#region SerializedProperties
        SerializedProperty _bulletInfo = serializedObject.FindProperty("bulletInfo");
        SerializedProperty _bulletType = _bulletInfo.FindPropertyRelative("type");
        SerializedProperty _direction = _bulletInfo.FindPropertyRelative("direction");
        SerializedProperty _maxTileLength = _bulletInfo.FindPropertyRelative("maxTileLength");
        SerializedProperty _ignoreWalls = _bulletInfo.FindPropertyRelative("ignoreWalls");
        SerializedProperty _chargeBeats = serializedObject.FindProperty("chargeBeats");
        SerializedProperty _waitBeforeReload = serializedObject.FindProperty("waitBeforeReload");
        SerializedProperty _bulletPrefab = serializedObject.FindProperty("bulletPrefab");
        SerializedProperty _noChargeTime = serializedObject.FindProperty("noChargeTime");
        SerializedProperty _noReload = serializedObject.FindProperty("noReload");
        #endregion

        EditorGUILayout.LabelField("Bullet Info");
        EditorGUILayout.PropertyField(_bulletType);
        EditorGUILayout.PropertyField(_direction);
        EditorGUILayout.PropertyField(_maxTileLength);
        EditorGUILayout.PropertyField(_ignoreWalls);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Weapon Info");
        EditorGUILayout.PropertyField(_noChargeTime);
        if (!_noChargeTime.boolValue)
        {
            EditorGUILayout.PropertyField(_chargeBeats);
        }
        EditorGUILayout.PropertyField(_noReload);
        if (!_noReload.boolValue)
        {
            EditorGUILayout.PropertyField(_waitBeforeReload);
        }

        serializedObject.ApplyModifiedProperties(); // Save the serializedObject
    }
}
