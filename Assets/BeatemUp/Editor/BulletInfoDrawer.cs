using UnityEngine;
using UnityEditor;
/*
[CanEditMultipleObjects]
[CustomPropertyDrawer(typeof(BulletInfo))]
public class BulletInfoDrawer : PropertyDrawer
{
    private SerializedProperty _property;
    private GUIContent _label;
    int lineCount;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        _property = property;
        _label = label;

        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;
        lineCount = 0;

        EditorGUI.BeginProperty(position, label, property);

        position = EditorGUI.PrefixLabel(position, label);

        Rect lengthRect = new Rect(position.x, position.y, position.width - 10, LineHeight);
        SerializedProperty lengthProperty = property.FindPropertyRelative("length");
        EditorGUI.PropertyField(lengthRect, lengthProperty);
        ++lineCount;

        SerializedProperty lockProperty = property.FindPropertyRelative("lockDirection");

        if (GUI.Button(new Rect(position.x, position.y + LineHeight, position.width / 2, LineHeight), "Follow Input"))
        {
            lockProperty.boolValue = false;
        }
        else if (GUI.Button(new Rect(position.x + position.width / 2, position.y + LineHeight, position.width / 2, LineHeight), "Lock Direction"))
        {
            lockProperty.boolValue = true;

            if (GUI.Button(new Rect(position.x, position.y + 2 * LineHeight, position.width / 3, LineHeight), "Close"))
            {
                Rect lengthRect2 = new Rect(position.x, position.y + 3 * LineHeight, position.width - 10, LineHeight);
                EditorGUI.PropertyField(lengthRect2, lengthProperty);
                ++lineCount;
            }
        }
        ++lineCount;

        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();
    }
    private float Height { get => GetHeight(_property, _label); }
    private float LineHeight { get => EditorGUIUtility.singleLineHeight; }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return base.GetPropertyHeight(property, label) * lineCount;
    }

    public float GetHeight(SerializedProperty property, GUIContent label)
    {
        float H = EditorGUIUtility.singleLineHeight * lineCount + EditorGUIUtility.standardVerticalSpacing * (lineCount - 1);
        Debug.Log(H + " x " + lineCount);
        return (H >= 0) ? H : 0;
    }
}*/
