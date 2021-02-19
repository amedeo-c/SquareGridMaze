using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

public static class WallIndexing
{
    public static int squareDimension;

    public static WallIndexes IndexesOfWallInBetween(int rowA, int colA, int rowB, int colB)
    {
        Debug.Assert(rowA == rowB ^ colA == colB);
        Debug.Assert(Mathf.Abs(rowA - rowB) + Mathf.Abs(colA - colB) == 1);
        // we assume positive values for all of the 4 indexes

        if (Mathf.Abs(rowA - rowB) + Mathf.Abs(colA - colB) != 1)
        {
            Debug.Log("invalid cell couple");
            return null;
        }

        if (rowA == rowB) // vertical wall
        {
            int wallRow = rowA;
            int wallCol = Mathf.Min(colA, colB);

            if (wallRow == squareDimension - 1 && wallCol >= squareDimension - 1)
            {
                Debug.Log("invalid cell couple");
                return null;
            }

            return new WallIndexes(wallRow, wallCol);
        }
        else // horizontal wall
        {
            int wallRow = Mathf.Min(rowA, rowB);
            int wallCol = colA + squareDimension - 1;

            if (wallRow == squareDimension - 1 && wallCol >= squareDimension - 1)
            {
                Debug.Log("invalid cell couple");
                return null;
            }

            //Debug.Log(wallRow.ToString() + ", " + wallCol.ToString());
            return new WallIndexes(wallRow, wallCol);
        }
    }

    public static Wall WallInBetween(int rowA, int colA, int rowB, int colB)
    {
        if(Level.walls == null)
        {
            Debug.Log("level walls not present");
            return null;
        }

        WallIndexes indexes = IndexesOfWallInBetween(rowA, colA, rowB, colB);
        return Level.walls[indexes.row, indexes.col];
    }

    public static Wall WallInBetween(Cell cellA, Cell cellB)
    {
        if (Level.walls == null)
        {
            Debug.Log("level walls not present");
            return null;
        }

        WallIndexes indexes = IndexesOfWallInBetween(cellA.Row, cellA.Col, cellB.Row, cellB.Col);
        return Level.walls[indexes.row, indexes.col];
    }
}

public class WallIndexes
{
    public int row;
    public int col;

    public WallIndexes(int row, int col)
    {
        this.row = row;
        this.col = col;
    }
}
