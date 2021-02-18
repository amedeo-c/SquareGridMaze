using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

public class MapExploring : MonoBehaviour
{
    int mapDimension;

    public Wall[,] walls;
    public Cell[,] cells;

    int openWalls;

    void Traverse()
    {
        int row = 0;
        int col = 0;

        //cells[0, 0].Marked = true;

        while(!(row == mapDimension-1 && col == mapDimension - 1))
        {
            if(row == mapDimension - 1)
            {
                ChangeCell(row, col, row, col + 1);
                col++;
            }
            else if(col == mapDimension - 1)
            {
                ChangeCell(row, col, row + 1, col);
                row++;
            }
            else
            {
                if(Random.value < 0.5f)
                {
                    ChangeCell(row, col, row, col + 1);
                    col++;
                }
                else
                {
                    ChangeCell(row, col, row + 1, col);
                    row++;
                }
            }
        }

        cells[mapDimension - 1, mapDimension - 1].Marked = true;
    }

    void LootTraverse()
    {
        // from upper left to lower right
        int row = mapDimension - 1;
        int col = 0;

        while(!cells[row, col].Marked)
        {
            if(row == 0)
            {
                ChangeCell(row, col, row, col + 1);
                col++;
            }
            else if(col == mapDimension - 1)
            {
                ChangeCell(row, col, row-1, col);
                row--;
            }
            else
            {
                if(Random.value < 0.5f)
                {
                    ChangeCell(row, col, row, col + 1);
                    col++;
                }
                else
                {
                    ChangeCell(row, col, row - 1, col);
                    row--;
                }
            }
        }

        // from lower right to upper left
        row = 0;
        col = mapDimension - 1;

        while(!cells[row, col].Marked)
        {
            if(row == mapDimension - 1)
            {
                ChangeCell(row, col, row, col - 1);
                col--;
            }
            else if(col == 0)
            {
                ChangeCell(row, col, row + 1, col);
                row++;
            }
            else
            {
                if(Random.value < 0.5f)
                {
                    ChangeCell(row, col, row, col - 1);
                    col--;
                }
                else
                {
                    ChangeCell(row, col, row + 1, col);
                    row++;
                }
            }
        }
    }

    public void TraverseMethod()
    {
        Prepare();

        Traverse();
        OpenEnterWalls();
        LootTraverse();

        Debug.Log("open walls: " + openWalls.ToString());
    }

    void ChangeCell(int currentRow, int currentCol, int nextRow, int nextCol)
    {
        WallIndexes wallIndexes = WallIndexing.IndexesOfWallInBetween(currentRow, currentCol, nextRow, nextCol);
        Wall wallInBetween = walls[wallIndexes.row, wallIndexes.col];

        wallInBetween.Open = true;
        openWalls++;

        cells[currentRow, currentCol].Marked = true;
    }

    void OpenEnterWalls()
    {
        WallIndexes wallIndexes;

        wallIndexes = WallIndexing.IndexesOfWallInBetween(0, 0, 0, 1);
        if(!walls[wallIndexes.row, wallIndexes.col].Open)
        {
            walls[wallIndexes.row, wallIndexes.col].Open = true;
            openWalls++;
        }

        wallIndexes = WallIndexing.IndexesOfWallInBetween(0, 0, 1, 0);
        if (!walls[wallIndexes.row, wallIndexes.col].Open)
        {
            walls[wallIndexes.row, wallIndexes.col].Open = true;
            openWalls++;
        }

        wallIndexes = WallIndexing.IndexesOfWallInBetween(mapDimension-1, mapDimension-1, mapDimension-1, mapDimension-2);
        if (!walls[wallIndexes.row, wallIndexes.col].Open)
        {
            walls[wallIndexes.row, wallIndexes.col].Open = true;
            openWalls++;
        }

        wallIndexes = WallIndexing.IndexesOfWallInBetween(mapDimension-1, mapDimension-1, mapDimension-2, mapDimension-1);
        if (!walls[wallIndexes.row, wallIndexes.col].Open)
        {
            walls[wallIndexes.row, wallIndexes.col].Open = true;
            openWalls++;
        }
    }

    void CloseAllWalls()
    {
        foreach (Wall w in walls)
        {
            if (w != null)
            {
                w.Open = false;
            }
        }

        openWalls = 0;
    }

    void DemarkAllCells()
    {
        foreach (Cell c in cells)
        {
            c.Marked = false;
        }
    }

    void FindLevel()
    {
        Level l = FindObjectOfType<Level>();

        Debug.Assert(l != null);

        walls = l.walls;
        cells = l.cells;
        mapDimension = l.squareDimension;
    }

    void Prepare()
    {
        FindLevel();
        DemarkAllCells();
        CloseAllWalls();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
