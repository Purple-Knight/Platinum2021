using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlayerMovement))]
public class PlayerMovementEditor : Editor
{
    public static string[] EXCLUDE_PROPERTIES = { "sprite" , "PlayerHit", "bufferTime", "playerColor", "_guiDebug" , "_guiDebugArea"};


    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        //base.OnInspectorGUI();

        DrawPropertiesExcluding(serializedObject, EXCLUDE_PROPERTIES);
        SerializedProperty colorProperty = serializedObject.FindProperty("playerColor");
        SerializedProperty guiDebugEnable = serializedObject.FindProperty("_guiDebug");
        SerializedProperty guiDebugRect = serializedObject.FindProperty("_guiDebugArea");
        

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Player Color");
        colorProperty.colorValue = EditorGUILayout.ColorField(colorProperty.colorValue);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("GUI debug");
        guiDebugEnable.boolValue = EditorGUILayout.Toggle(guiDebugEnable.boolValue);
        EditorGUILayout.EndHorizontal();

        if (guiDebugEnable.boolValue)
        {
            guiDebugRect.rectValue = EditorGUILayout.RectField(guiDebugRect.rectValue);
        }


        //spriteProperty.colorValue = colorProperty.colorValue;
        serializedObject.ApplyModifiedProperties();


        Color colorValue = colorProperty.colorValue;
        SerializedProperty spriteRendererProperty = serializedObject.FindProperty("sprite");
        SpriteRenderer spriteRenderer = spriteRendererProperty.objectReferenceValue as SpriteRenderer;

        if (null != spriteRenderer)
        {
            spriteRenderer.color = colorValue;
        }

    }
}
