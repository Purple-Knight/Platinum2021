using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class GridLevelEditor : EditorWindow
{
    //ceci est safe a changer :eyes:
    float levelWidth = 18;
    float levelHeight = 10;

    //don't touch >:( 


    float cellSize = 35;
    Vector2 inGameGridSize = new Vector2(1, .5f);

    Vector2 offset;
    Vector2 drag;

    List<List<Node>> nodes;
    GUIStyle emptyStyle;
    Vector2 nodePos;

    StyleManager styleManager;
    bool isErasing;

    Rect menuBar;
    GUIStyle currentStyle;

    GameObject theMap;
    List<List<PartScript>> parts;

    [MenuItem("Tools/Level Editor")]
    private static void OpenWindow()
    {
        GridLevelEditor window = GetWindow<GridLevelEditor>();
        window.titleContent = new GUIContent("Level Editor");
    }

    private void OnEnable()
    {
        SetUpStyles();
        SetUpMap();
        
    }

    private void SetUpMap()
    {
        try
        {
            theMap = GameObject.FindGameObjectWithTag("Map");
            if (theMap != null)
            {
                SetUpNodesAndParts();
                RestoreTheMap(theMap);
            }
        }
        catch (Exception exception)
        {
            Debug.LogError(exception);
            throw;
        }

        if (theMap == null)
        {
            theMap = new GameObject("Map");
            theMap.tag = "Map";
            SetUpNodesAndParts();
        }
    }

    private void RestoreTheMap(GameObject Map)
    {
        if (Map.transform.childCount > 0)
        {
            for (int i = 0; i < Map.transform.childCount; i++)
            {
                int partRow = Map.transform.GetChild(i).GetComponent<PartScript>().row;
                int partCol = Map.transform.GetChild(i).GetComponent<PartScript>().col;
                DestroyImmediate(parts[partRow][partCol].gameObject);

                GUIStyle gUIStyle = Map.transform.GetChild(i).GetComponent<PartScript>().style;
                nodes[partRow][partCol].SetStyle(gUIStyle);

                parts[partRow][partCol] = Map.transform.GetChild(i).GetComponent<PartScript>();
                parts[partRow][partCol].part = Map.transform.GetChild(i).gameObject;
                parts[partRow][partCol].name = Map.transform.GetChild(i).name;
                parts[partRow][partCol].row = partRow;
                parts[partRow][partCol].col = partCol;
            }
        }
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

        emptyStyle = styleManager.buttonStyles[0].NodeStyle;
        currentStyle = styleManager.buttonStyles[0].NodeStyle;
    }

    private void SetUpNodesAndParts()
    {
        nodes = new List<List<Node>>();
        parts = new List<List<PartScript>>();

        for (int i = 0; i < levelHeight; i++)
        {
            nodes.Add(new List<Node>());
            parts.Add(new List<PartScript>());

            for (int j = 0; j < levelWidth; j++)
            {
                nodePos.Set(j * cellSize, i * cellSize);
                nodes[i].Add(new Node(nodePos, cellSize, cellSize, emptyStyle));
                parts[i].Add(null); 
                InstantiateTile(i,j , emptyStyle);
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
        int Col = (int)((@event.mousePosition.x - offset.x) / cellSize);
        int Row = (int)((@event.mousePosition.y - offset.y) / cellSize);

        if ((@event.mousePosition.x - offset.x) < 0 || (@event.mousePosition.x - offset.x) > cellSize * levelWidth||
            (@event.mousePosition.y - offset.y) < 0 || (@event.mousePosition.y - offset.y) > cellSize * levelHeight)
        { }
        else
        {
            if (@event.type == EventType.MouseDown)
            {
                if (nodes[Row][Col].style.normal.background.name == currentStyle.normal.background.name)
                {
                    isErasing = true;
                }
                else
                {
                    isErasing = false;
                }
                Debug.Log(isErasing);
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
            Debug.Log("row : " + row + "   col : " + col);
            if (parts[row][col].name == currentStyle.normal.background.name) 
            {
                nodes[row][col].SetStyle(emptyStyle);

                DestroyImmediate(parts[row][col].gameObject);
                InstantiateTile(row, col, emptyStyle); 

                GUI.changed = true;
            }
        }
        else
        {
            if (parts[row][col].name != currentStyle.normal.background.name) 
            {
                nodes[row][col].SetStyle(currentStyle);

                DestroyImmediate(parts[row][col].gameObject);
                InstantiateTile(row, col, currentStyle);

                GUI.changed = true;
            }
        }
    }

    private void InstantiateTile(int row, int col, GUIStyle style)
    {
        GameObject gameObject = Instantiate(Resources.Load("MapParts/" + style.normal.background.name)) as GameObject;
        gameObject.name = style.normal.background.name;
        gameObject.transform.position = new Vector3(col * inGameGridSize.x, -row * inGameGridSize.y, 0);
        gameObject.transform.parent = theMap.transform;

        parts[row][col] = gameObject.GetComponent<PartScript>();
        parts[row][col].part = gameObject;
        parts[row][col].name = gameObject.name;
        parts[row][col].row = row;
        parts[row][col].col = col;
        parts[row][col].style = style;
    }

    private void DrawNodes()
    {
        for (int i = 0; i < levelHeight; i++)
        {
            for (int j = 0; j < levelWidth; j++)
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

        for (int i = 0; i < levelHeight; i++) //drag Level Elements
        {
            for (int j = 0; j < levelWidth; j++)
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
