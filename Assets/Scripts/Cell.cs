using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public int Row { get; set; }
    public int Col { get; set; }

    public CellType type = CellType.Default;

    public bool marked;

    Wall[] adjacentWalls;

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

    public void SetConnections()
    {
        var directions = System.Enum.GetValues(typeof(Direction));
        adjacentWalls = new Wall[directions.Length];

        foreach(int d in directions)
        {

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
    
    public Wall AdjacentWall(Direction direction)
    {
        Cell otherCell = AdjacentCell(direction);

        return WallIndexing.WallInBetween(Row, Col, otherCell.Row, otherCell.Col);
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
