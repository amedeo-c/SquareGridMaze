using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;
using System.Threading.Tasks;

public class LevelExplorer : MonoBehaviour
{
    [Tooltip("number of doors that is required for the level")]
    public int numWallsToOpen;

    [Tooltip("determines if it's possible to generate doors between unreachable cells")]
    public bool isolatedPathsAllowed;

    [Tooltip("true for play mode")]
    public bool deactivateOpenWalls;

    [Tooltip("time interval between generation steps")]
    public float waitDuration;

    [Tooltip("method used to generate the path in the level")]
    public TraverseMode traverseMode;

    [HideInInspector]
    [Tooltip("number of attempts for random traversing method, before switching to direct")]
    public int maxTries; // random traversing is not guaranteed to successfully generate a level satisfying the requirements.

    int openWalls;

    Cell currentCell;
    Cell previousCell;

    public async Task RandomTraverseMethod()
    {
        bool ready = Prepare();

        if (ready)
        {
            int tries = 0;

            do
            {
                Prepare();

                await RandomTraverseFromTo(Level.EnterCell, Level.BossCell, true, false);
                // after the first traverse, for successive traverse we stop at marked cells, that is cells that are path of the first traverse path.
                // for this reason, we should not mark cells during successive traverses otherwise could stop before reaching the first traverse path.
                // this happens since for the random method loops for the same path are allowed, while in the case of direct traverse this problem does not arise.
                await RandomTraverseFromTo(Level.UpperLootCell, Level.LowerLootCell, false, true);
                await RandomTraverseFromTo(Level.LowerLootCell, Level.UpperLootCell, false, true);

                OpenMandatoryWalls();

                tries++;

            } while (openWalls > numWallsToOpen && tries < maxTries); // the control on the number of open walls can be done while traversing..

            if (openWalls < numWallsToOpen)
            {
                if (isolatedPathsAllowed)
                {
                    await OpenRemainingWalls();
                }
                else
                {
                    await OpenRemainingWalls2();
                }

                Debug.Assert(openWalls == numWallsToOpen);
            }
            else if(tries >= maxTries)
            {
                Debug.Log("switching to direct traversing mode");
                DirectTraverseMethod();
            }

        }
    }

    // traverse the grid at each step choosing a random cell among the adjacent one but for the previous one (no going back)
    async Task RandomTraverseFromTo(Cell sourceCell, Cell targetCell, bool markCells, bool stopOnMarked)
    {
        currentCell = sourceCell;
        previousCell = sourceCell;

        while(currentCell != targetCell && openWalls < numWallsToOpen)
        {
            var availableCells = currentCell.AdjacentCells();
            availableCells.Remove(previousCell);

            if(availableCells.Count > 0)
            {
                Cell nextCell = availableCells[Random.Range(0, availableCells.Count)];
                ChangeCell(nextCell, markCells);

                if ((stopOnMarked && currentCell.Marked) || openWalls >= numWallsToOpen)
                {
                    break;
                }

                await Task.Delay((int)(waitDuration * 1000f));
            }
            else
            {
                Debug.Log("dead end");
                break;
            }
        }

        currentCell.Marked = true;
    }

    public async Task DirectTraverseMethod()
    {
        bool ready = Prepare();

        if (ready)
        {
            await DirectTraverseFromTo(0, 0, Level.Dimension - 1, Level.Dimension - 1);
            await DirectTraverseFromTo(0, Level.Dimension - 1, Level.Dimension - 1, 0);
            await DirectTraverseFromTo(Level.Dimension - 1, 0, 0, Level.Dimension - 1);

            OpenMandatoryWalls();

            Debug.Assert(openWalls <= numWallsToOpen);

            if (openWalls < numWallsToOpen)
            {
                if (isolatedPathsAllowed)
                {
                    await OpenRemainingWalls();
                }
                else
                {
                    await OpenRemainingWalls2();
                }
            }

            Debug.Assert(openWalls == numWallsToOpen);
        }
    }

    // traverse the grid usign only two kind of moves (the two depending on the direction of the target cell with respect to the source cell).
    // also, here the adjactent cells and in between walls are referenced only using indexes in their respective grid and not the more graph-like
    // description seen for the random treverse case. still, it is only a matter of verbosity since the methods for deriving adjacencies are the
    // exact same.
    async Task DirectTraverseFromTo(int sourceRow, int sourceCol, int targetRow, int targetCol)
    {
        int verticalDirection = (int)Mathf.Sign(targetRow - sourceRow);
        int horizontalDirection = (int)Mathf.Sign(targetCol - sourceCol);

        int currentRow = sourceRow;
        int currentCol = sourceCol;

        while (!(currentRow == targetRow && currentCol == targetCol) && !Level.cells[currentRow, currentCol].Marked)
        {
            if (currentRow == targetRow)
            {
                ChangeCell(currentRow, currentCol, currentRow, currentCol + horizontalDirection, true);
                currentCol += horizontalDirection;
            }
            else if (currentCol == targetCol)
            {
                ChangeCell(currentRow, currentCol, currentRow + verticalDirection, currentCol, true);
                currentRow += verticalDirection;
            }
            else
            {
                if (Random.value < 0.5f)
                {
                    ChangeCell(currentRow, currentCol, currentRow, currentCol + horizontalDirection, true);
                    currentCol += horizontalDirection;
                }
                else
                {
                    ChangeCell(currentRow, currentCol, currentRow + verticalDirection, currentCol, true);
                    currentRow += verticalDirection;
                }
            }

            await Task.Delay((int)(waitDuration * 1000f));
        }

        Level.cells[currentRow, currentCol].Marked = true;
    }

    // one step move
    // also opens the wall/door separating the two cells
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

    // same as above but with direct cell indexing
    // since we use use function only in the case of direct traversing, we do not need to store data regarding the previous cell
    void ChangeCell(int currentRow, int currentCol, int nextRow, int nextCol, bool mark)
    {
        WallIndexes wallIndexes = WallIndexing.IndexesOfWallInBetween(currentRow, currentCol, nextRow, nextCol);
        Wall wallInBetween = Level.walls[wallIndexes.row, wallIndexes.col];

        wallInBetween.Open = true;
        openWalls++;

        if (mark)
        {
            Level.cells[currentRow, currentCol].Marked = true;
        }
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

    // leftover walls to be opened are chosen at random among remaining closed walls
    // this allows to open "isolated walls", that is unreachable doors.
    async Task OpenRemainingWalls()
    {
        List<Wall> closedWalls = new List<Wall>();
        foreach (Wall w in Level.walls)
        {
            if (w != null && !w.Open)
            {
                closedWalls.Add(w);
            }
        }

        while (openWalls < numWallsToOpen && openWalls < Level.NumWalls)
        {
            Wall randomClosedWall = closedWalls[Random.Range(0, closedWalls.Count)];
            randomClosedWall.Open = true;
            openWalls++;
            closedWalls.Remove(randomClosedWall);
            await Task.Delay((int)(waitDuration * 1000));
        }
    }

    // leftover walls to be opened are opened using random traverses starting from marked cells
    // 
    async Task OpenRemainingWalls2()
    {
        List<Cell> markedCells = new List<Cell>();
        foreach(Cell c in Level.cells)
        {
            if (c.Marked)
            {
                markedCells.Add(c);
            }
        }

        while(openWalls < numWallsToOpen)
        {
            Cell startingCell = markedCells[Random.Range(0, markedCells.Count)];
            Cell targetCell = Level.cells[Random.Range(0, Level.Dimension), Random.Range(0, Level.Dimension)];

            await RandomTraverseFromTo(startingCell, targetCell, false, false);
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

    bool Prepare()
    {
        if(Level.cells == null || Level.walls == null)
        {
            Debug.Log("no valid grid present");
            return false;
        }

        Wall.deactivateOnOpen = deactivateOpenWalls;
        DemarkAllCells();
        CloseAllWalls();

        return true;
    }

    public void Clear()
    {
        Prepare();
    }

    public async Task Traverse()
    {
        switch (traverseMode)
        {
            case TraverseMode.Direct:

                await DirectTraverseMethod();
                break;

            case TraverseMode.Random:

                await RandomTraverseMethod();
                break;
        }
    }

}

public enum TraverseMode
{
    Direct,
    Random
}