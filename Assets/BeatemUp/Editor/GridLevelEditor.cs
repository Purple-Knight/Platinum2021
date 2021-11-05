using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class GridLevelEditor : EditorWindow
{
    float levelWidth = 10;
    float levelHeight = 10;
    float cellSize = 30;

    Vector2 offset;
    Vector2 drag;

    List<List<Node>> nodes;
    GUIStyle emptyStyle;
    Vector2 nodePos;

    StyleManager styleManager;
    bool isErasing;

    Rect menuBar;
    GUIStyle currentStyle;

    [MenuItem("Tools/Level Editor")]
    private static void OpenWindow()
    {
        GridLevelEditor window = GetWindow<GridLevelEditor>();
        window.titleContent = new GUIContent("Level Editor");
    }

    private void OnEnable()
    {
        SetUpStyles();
        emptyStyle = new GUIStyle();
        Texture2D icon = Resources.Load("IconLevelEditorTex/Empty") as Texture2D;
        emptyStyle.normal.background = icon;

        SetUpNodes();
        currentStyle = styleManager.buttonStyles[0].NodeStyle;
    }

    private void SetUpStyles()
    {
        try
        {
            styleManager = GameObject.FindGameObjectWithTag("StyleManager").GetComponent<StyleManager>();

            for (int i = 0; i < styleManager.buttonStyles.Length; i++)
            {
                styleManager.buttonStyles[i].NodeStyle = new GUIStyle();
                styleManager.buttonStyles[i].NodeStyle.normal.background = styleManager.buttonStyles[i].Icon;

            }
        }
        catch (Exception exception)
        {
            Debug.LogError(exception);
            throw;
        }
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
        DrawMenuBar();
        ProcessNodes(Event.current);
        ProcessGrid(Event.current);

        if (GUI.changed)
        {
            Repaint();
        }
    }

    private void DrawMenuBar()
    {
        menuBar = new Rect(0, 0, position.width, 20);
        GUILayout.BeginArea(menuBar, EditorStyles.toolbar);
        GUILayout.BeginHorizontal();

        for (int i = 0; i < styleManager.buttonStyles.Length; i++)
        {
            if (GUILayout.Toggle((currentStyle == styleManager.buttonStyles[i].NodeStyle),
                new GUIContent(styleManager.buttonStyles[i].ButtonTex),
                EditorStyles.toolbarButton, GUILayout.Width(80)))
            {
                currentStyle = styleManager.buttonStyles[i].NodeStyle;
            }
        }

        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }

    private void ProcessNodes(Event @event)
    {
        int Row = (int)((@event.mousePosition.x - offset.x) / cellSize);
        int Col = (int)((@event.mousePosition.y - offset.y) / cellSize);

        if ((@event.mousePosition.x - offset.x) < 0 || (@event.mousePosition.x - offset.x) > cellSize * levelWidth ||
            (@event.mousePosition.y - offset.y) < 0 || (@event.mousePosition.y - offset.y) > cellSize * levelHeight)
        { }
        else
        {
            if (@event.type == EventType.MouseDown)
            {
                if (nodes[Row][Col].style.normal.background.name == "Empty")
                {
                    isErasing = false;
                }
                else
                {
                    isErasing = true;
                }
                PaintNodes(Row, Col);
            }
            if (@event.type == EventType.MouseDrag)
            {
                PaintNodes(Row, Col);
                @event.Use();
            }
        }
    }

    private void PaintNodes(int row, int col)
    {
        if (isErasing)
        {
            nodes[row][col].SetStyle(emptyStyle);
            GUI.changed = true;
        }
        else
        {
            nodes[row][col].SetStyle(currentStyle);
            GUI.changed = true;
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
