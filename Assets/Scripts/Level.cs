using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

public class Level : MonoBehaviour
{
    public static int Dimension;
    public static int NumWalls;  // interior

    public static Cell[,] cells;
    public static Wall[,] walls;

    // because of the assumed strict cell type scheme, "special" cells position is fixed and known a priori.

    public static Cell EnterCell
    {
        get
        {
            return cells[0, 0];

        }
    }

    public static Cell BossCell
    {
        get
        {
            return cells[Dimension - 1, Dimension - 1];
        }
    }

    public static Cell UpperLootCell
    {
        get
        {
            return cells[Dimension - 1, 0];
        }
    }

    public static Cell LowerLootCell
    {
        get
        {
            return cells[0, Dimension - 1];
        }
    }

    public static void DemarkAllCells()
    {
        foreach(Cell c in cells)
        {
            c.Marked = false;
        }
    }

    public static void CloseAllWalls()
    {
        foreach(Wall w in walls)
        {
            if(w != null)
            {
                w.Open = false;
            }
        }
    }

    public static void RemoveWallHighlights()
    {
        foreach (Wall w in walls)
        {
            if (w != null)
            {
                w.Highlighted = false;
            }
        }
    }

    private void Awake()
    {
        LevelBuilder builder = GetComponent<LevelBuilder>();
        LevelExplorer explorer = GetComponent<LevelExplorer>();
        Debug.Assert(builder != null && explorer != null);

        builder.waitDuration = 0f;
        builder.BuildAll();

        explorer.waitDuration = 0f;
        explorer.Traverse();
    }
}