using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class GridLevelEditor : EditorWindow
{
    float levelWidth = 10;
    float levelHeight = 5;
    float cellSize = 30;


    Vector2 offset;
    Vector2 drag;

    List<List<Node>> nodes;
    GUIStyle emptyStyle;
    Vector2 nodePos;

    [MenuItem("Tools/Level Editor")]
    private static void OpenWindow()
    {
        GridLevelEditor window = GetWindow<GridLevelEditor>();
        window.titleContent = new GUIContent("Level Editor");
    }

    private void OnEnable()
    {
        emptyStyle = new GUIStyle();
        Texture2D icon = Resources.Load("IconLevelEditorTex/Empty") as Texture2D;
        emptyStyle.normal.background = icon;

        SetUpNodes();
    }

    private void SetUpNodes()
    {
        nodes = new List<List<Node>>();

        for (int i = 0; i < levelWidth; i++)
        {
            nodes.Add(new List<Node>());

            for (int j = 0; j < levelHeight; j++)
            {
                nodePos.Set(i * cellSize, j * cellSize);
                nodes[i].Add(new Node(nodePos, cellSize, cellSize, emptyStyle));
            }
        }
    }

    private void OnGUI()
    {
        DrawGrid();
        DrawNodes();
        ProcessGrid(Event.current);

        if (GUI.changed)
        {
            Repaint();
        }
    }

    private void DrawNodes()
    {
        for (int i = 0; i < levelWidth; i++)
        {
            for (int j = 0; j < levelHeight; j++)
            {
                nodes[i][j].Draw();
            }
        }
    }

    private void ProcessGrid(Event @event)
    {
        drag = Vector2.zero;

        switch (@event.type)
        {
            case EventType.MouseDrag:
                if (@event.button == 0)
                {
                    OnMouseDrag(@event.delta);
                }
                break;
        }
    }

    private void OnMouseDrag(Vector2 delta)
    {
        drag = delta; //drag Grid

        for (int i = 0; i < levelWidth; i++) //drag Level Elements
        {
            for (int j = 0; j < levelHeight; j++)
            {
                nodes[i][j].Drag(delta);
            }
        }

        GUI.changed = true;
    }

    private void DrawGrid()
    {
        int widthDivider = Mathf.CeilToInt(position.width / cellSize);
        int heightDivider = Mathf.CeilToInt(position.height / cellSize);

        Handles.BeginGUI();
        Handles.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);

        offset += drag;
        Vector3 newOffset = new Vector3(offset.x % cellSize, offset.y % cellSize, 0);

        for (int i = 0; i < widthDivider; i++)
        {
            Handles.DrawLine(new Vector3(cellSize * i, -cellSize, 0) + newOffset, new Vector3(cellSize * i, position.height, 0) + newOffset);
        }
        for (int j = 0; j < heightDivider; j++)
        {
            Handles.DrawLine(new Vector3(-cellSize, cellSize * j, 0) + newOffset, new Vector3(position.width, cellSize * j, 0) + newOffset);
        }

        Handles.color = Color.white;
        Handles.EndGUI();
    }
}
