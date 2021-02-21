using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

public class Level : MonoBehaviour
{
    public static int Dimension;
    public static int NumWalls;

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

    private void Awake()
    {
        LevelBuilder builder = GetComponent<LevelBuilder>();
        MapExploring explorer = GetComponent<MapExploring>();
        Debug.Assert(builder != null && explorer != null);

        builder.waitDuration = 0f;
        builder.BuildAll();

        explorer.waitDuration = 0f;
        explorer.Traverse();
    }
}