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



    // Start is called before the first frame update
    void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}