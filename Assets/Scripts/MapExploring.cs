using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

public class MapExploring : MonoBehaviour
{
    public int numWallsToOpen;

    int openWalls;

    public void TraverseMethod()
    {
        Prepare();

        TraverseFromTo(0, 0, Level.Dimension - 1, Level.Dimension - 1);
        TraverseFromTo(0, Level.Dimension - 1, Level.Dimension - 1, 0);
        TraverseFromTo(Level.Dimension - 1, 0, 0, Level.Dimension - 1);

        OpenMandatoryWalls();

        Debug.Assert(openWalls <= numWallsToOpen);

        if (openWalls < numWallsToOpen)
        {
            OpenRemainingWalls();
        }

        Debug.Assert(openWalls == numWallsToOpen);
    }

    void TraverseFromTo(int sourceRow, int sourceCol, int targetRow, int targetCol)
    {
        int verticalDirection = (int)Mathf.Sign(targetRow - sourceRow);
        int horizontalDirection = (int)Mathf.Sign(targetCol - sourceCol);

        int currentRow = sourceRow;
        int currentCol = sourceCol;

        while (!(currentRow == targetRow && currentCol == targetCol) && !Level.cells[currentRow, currentCol].Marked)
        {
            if (currentRow == targetRow)
            {
                ChangeCell(currentRow, currentCol, currentRow, currentCol + horizontalDirection);
                currentCol += horizontalDirection;
            }
            else if (currentCol == targetCol)
            {
                ChangeCell(currentRow, currentCol, currentRow + verticalDirection, currentCol);
                currentRow += verticalDirection;
            }
            else
            {
                if (Random.value < 0.5f)
                {
                    ChangeCell(currentRow, currentCol, currentRow, currentCol + horizontalDirection);
                    currentCol += horizontalDirection;
                }
                else
                {
                    ChangeCell(currentRow, currentCol, currentRow + verticalDirection, currentCol);
                    currentRow += verticalDirection;
                }
            }
        }

        Level.cells[currentRow, currentCol].Marked = true;
    }

    void ChangeCell(int currentRow, int currentCol, int nextRow, int nextCol)
    {
        WallIndexes wallIndexes = WallIndexing.IndexesOfWallInBetween(currentRow, currentCol, nextRow, nextCol);
        Wall wallInBetween = Level.walls[wallIndexes.row, wallIndexes.col];

        wallInBetween.Open = true;
        openWalls++;

        Level.cells[currentRow, currentCol].Marked = true;
    }

    void OpenMandatoryWalls()
    {
        WallIndexes wallIndexes;

        wallIndexes = WallIndexing.IndexesOfWallInBetween(0, 0, 0, 1);
        if(!Level.walls[wallIndexes.row, wallIndexes.col].Open)
        {
            Level.walls[wallIndexes.row, wallIndexes.col].Open = true;
            openWalls++;
        }

        wallIndexes = WallIndexing.IndexesOfWallInBetween(0, 0, 1, 0);
        if (!Level.walls[wallIndexes.row, wallIndexes.col].Open)
        {
            Level.walls[wallIndexes.row, wallIndexes.col].Open = true;
            openWalls++;
        }

        wallIndexes = WallIndexing.IndexesOfWallInBetween(Level.Dimension-1, Level.Dimension-1, Level.Dimension-1, Level.Dimension-2);
        if (!Level.walls[wallIndexes.row, wallIndexes.col].Open)
        {
            Level.walls[wallIndexes.row, wallIndexes.col].Open = true;
            openWalls++;
        }

        wallIndexes = WallIndexing.IndexesOfWallInBetween(Level.Dimension-1, Level.Dimension-1, Level.Dimension-2, Level.Dimension-1);
        if (!Level.walls[wallIndexes.row, wallIndexes.col].Open)
        {
            Level.walls[wallIndexes.row, wallIndexes.col].Open = true;
            openWalls++;
        }
    }

    void OpenRemainingWalls()
    {
        List<Wall> closedWalls = new List<Wall>();
        foreach(Wall w in Level.walls)
        {
            if (w != null && !w.Open)
            {
                closedWalls.Add(w);
            }
        }

        while(openWalls < numWallsToOpen && openWalls < Level.NumWalls)
        {
            Wall randomClosedWall = closedWalls[Random.Range(0, closedWalls.Count)];
            randomClosedWall.Open = true;
            openWalls++;
            closedWalls.Remove(randomClosedWall);
        }
    }

    void CloseAllWalls()
    {
        foreach (Wall w in Level.walls)
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
        foreach (Cell c in Level.cells)
        {
            c.Marked = false;
        }
    }

    void Prepare()
    {
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
