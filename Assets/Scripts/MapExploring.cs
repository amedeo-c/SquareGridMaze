using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

public class MapExploring : MonoBehaviour
{
    public int numWallsToOpen;

    public int maxTries;

    int openWalls;

    Cell currentCell;
    Cell previousCell;

    public void RandomTraverseMethod()
    {
        int tries = 0;

        do
        {
            Prepare();

            RandomTraverseFromTo(Level.EnterCell, Level.BossCell, true, false);
            RandomTraverseFromTo(Level.UpperLootCell, Level.LowerLootCell, false, true);
            RandomTraverseFromTo(Level.LowerLootCell, Level.UpperLootCell, false, true);

            OpenMandatoryWalls();

            tries++;

        } while (openWalls > numWallsToOpen && tries < maxTries); // the control on the number of open walls can be done while traversing..

        if(openWalls < numWallsToOpen)
        {
            OpenRemainingWalls();
        }

        Debug.Log(openWalls);
    }

    void RandomTraverseFromTo(Cell sourceCell, Cell targetCell, bool markCells, bool stopOnMarked)
    {
        currentCell = sourceCell;
        previousCell = sourceCell;

        while(currentCell != targetCell)
        {
            var availableCells = currentCell.AdjacentCells();
            availableCells.Remove(previousCell);

            if(availableCells.Count > 0)
            {
                Cell nextCell = availableCells[Random.Range(0, availableCells.Count)];
                ChangeCell(nextCell, markCells);

                if (stopOnMarked && currentCell.Marked)
                {
                    break;
                }

            }
            else
            {
                Debug.Log("dead end");
                break;
            }
        }

        currentCell.Marked = true;
    }

    public void DirectTraverseMethod()
    {
        Prepare();

        DirectTraverseFromTo(0, 0, Level.Dimension - 1, Level.Dimension - 1);
        DirectTraverseFromTo(0, Level.Dimension - 1, Level.Dimension - 1, 0);
        DirectTraverseFromTo(Level.Dimension - 1, 0, 0, Level.Dimension - 1);

        OpenMandatoryWalls();

        Debug.Assert(openWalls <= numWallsToOpen);

        if (openWalls < numWallsToOpen)
        {
            OpenRemainingWalls();
        }

        Debug.Assert(openWalls == numWallsToOpen);
    }

    void DirectTraverseFromTo(int sourceRow, int sourceCol, int targetRow, int targetCol)
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

    void ChangeCell(Cell nextCell, bool mark)
    {
        Wall wallInBetween = WallIndexing.WallInBetween(currentCell, nextCell);

        if (!wallInBetween.Open)
        {
            wallInBetween.Open = true;
            openWalls++;
        }
        previousCell = currentCell;
        currentCell = nextCell;

        if (mark)
        {
            previousCell.Marked = true;
        }
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
