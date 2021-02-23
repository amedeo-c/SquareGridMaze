using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public int Row { get; set; }
    public int Col { get; set; }

    [HideInInspector]
    public CellType type = CellType.Default;

    [HideInInspector]
    public bool marked; // used to distinguish already traversed cells during level exploration

    public CellType Type
    {
        set
        {
            type = value;
            //GetComponent<SpriteRenderer>().color = LevelColors.GetCellColor(type);
        }
    }

    public bool Marked
    {
        get
        {
            return marked;
        }
        set
        {
            marked = value;
        }
    }

    public Cell AdjacentCell(Direction direction)
    {
        switch (direction)
        {
            case Direction.North:

                if (Row == Level.Dimension - 1)
                {
                    return null;
                }
                else
                {
                    return Level.cells[Row + 1, Col];
                }

            case Direction.East:

                if (Col == Level.Dimension - 1)
                {
                    return null;
                }
                else
                {
                    return Level.cells[Row, Col + 1];
                }

            case Direction.South:

                if (Row == 0)
                {
                    return null;
                }
                else
                {
                    return Level.cells[Row - 1, Col]; 
                }

            default:

                if(Col == 0)
                {
                    return null;
                }
                else
                {
                    return Level.cells[Row, Col - 1];
                }
        }
    }

    public List<Cell> AdjacentCells()
    {
        List<Cell> cells = new List<Cell>();
        var directions = System.Enum.GetValues(typeof(Direction));

        foreach(int direction in directions)
        {
            Cell adjacentCell = AdjacentCell((Direction)direction);
            if(adjacentCell != null)
            {
                cells.Add(adjacentCell);
            }
        }

        return cells;

    }

    // available cells, during level exploration, are intended as adjacents cells that are still unmarked (not traversed yet)
    public List<Cell> AvailableCells()
    {
        List<Cell> cells = new List<Cell>();
        var directions = System.Enum.GetValues(typeof(Direction));

        foreach (int direction in directions)
        {
            Cell adjacentCell = AdjacentCell((Direction)direction);
            if (adjacentCell != null && !adjacentCell.Marked)
            {
                cells.Add(adjacentCell);
            }
        }

        return cells;
    }

    public IEnumerable<Cell> ReachableCells()
    {
        foreach(int direction in System.Enum.GetValues(typeof(Direction)))
        {
            Cell adjacentCell = AdjacentCell((Direction)direction);
            if(adjacentCell != null && !adjacentCell.Marked && WallIndexing.WallInBetween(this, adjacentCell).Open)
            {
                yield return adjacentCell;
            }
        }
    }

    public bool IsReachableFrom(Cell sourceCell)
    {
        if(sourceCell == this)
        {
            return true;
        }

        sourceCell.Marked = true;

        foreach(Cell c in sourceCell.ReachableCells())
        {
            if (IsReachableFrom(c))
            {
                return true;
            }
        }

        return false;
    }

    private void OnMouseUpAsButton()
    {
        Level.DemarkAllCells();

        Debug.Log("checking...");

        Debug.Log(IsReachableFrom(Level.EnterCell));
    }
}

public enum Direction
{
    North,
    East,
    South,
    West
}

public enum CellType
{
    Default,
    Enter,
    PostEnter,
    Easy,
    Loot,
    Hard,
    PreBoss,
    Boss
}
