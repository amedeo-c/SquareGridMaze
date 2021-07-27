using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public int Row { get; set; }
    public int Col { get; set; }

    CellType type = CellType.Default;

    public CellType Type
    {
        set
        {
            type = value;
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

    // all adjacent cells, regardless of marks and walls. cells on the grid perimeter will have some null adjacent cells.
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

    private void OnMouseUpAsButton()
    {
        LevelExplorer.ResetMarked();
        Level.RemoveWallHighlights();

        Debug.Log(LevelExplorer.IsReachableFrom(Level.EnterCell, this));
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
