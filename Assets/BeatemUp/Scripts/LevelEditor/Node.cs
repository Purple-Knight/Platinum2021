using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    Rect rect;
    public GUIStyle style;

    public Node(Vector2 position, float width, float heigth, GUIStyle defaultStyle)
    {
        rect = new Rect(position.x, position.y, width, heigth);
        style = defaultStyle;
    }

    public void Drag(Vector2 delta)
    {
        rect.position += delta;
    }

    public void Draw()
    {
        GUI.Box(rect, "", style);
    }

    public void SetStyle(GUIStyle nodeStyle)
    {
        style = nodeStyle;
    }
}
