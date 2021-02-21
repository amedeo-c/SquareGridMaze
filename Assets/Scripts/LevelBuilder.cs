using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Threading.Tasks;

public class LevelBuilder : MonoBehaviour
{
    public int dimension; // defines a dimension x dimension grid

    public float cellDistance; // distance between cell centers
    public float cellScale;

    // time interval between generation steps
    public float waitDuration;

    Vector2 firstCellPosition;

    // used as parents in the hierarchy (for cleaning purposes)
    GameObject cellHolderObj;
    GameObject interiorWallHolderObj;
    GameObject exteriorWallHolderObj;

    PrefabLoader prefabLoader;

    Cell[,] cells;
    Wall[,] interiorWalls;

    #region CellBuilding

    // given a dimension, builds a square grid of cells (centered in the middle of the screen)
    // the type/variant of each cell is chosen and set in the BuildCell() function
    public async Task BuildSquareGrid()
    {
        if (cells != null || cellHolderObj != null)
        {
            DestroyImmediate(cellHolderObj);
        }

        prefabLoader = GetComponent<PrefabLoader>();

        cellHolderObj = new GameObject("Cells");
        cellHolderObj.transform.SetParent(transform);

        cells = new Cell[dimension, dimension];

        float offSetFromCenter = (dimension - 0.9999f) / 2 * cellDistance;
        firstCellPosition = new Vector2(-offSetFromCenter, -offSetFromCenter);

        for (int j = 0; j < dimension; j++)
        {
            for (int i = 0; i < dimension; i++)
            {
                cells[j, i] = BuildCell(j, i, cellDistance);
                await Task.Delay((int)(waitDuration * 1000f));
            }
        }

        Level.cells = cells;
        Level.Dimension = dimension;
    }

    // a random prefab of the necessary type (based on the row and columnn indexes) is instantiated.
    // then, its Cell component is set up.
    public Cell BuildCell(int row, int column, float distance)
    {
        CellType type = DeriveCellType(row, column);
        GameObject cellPrefab = prefabLoader.GetRandomCellPrefab(type);

        GameObject newCellObj = Instantiate(cellPrefab);

        newCellObj.transform.position = firstCellPosition + new Vector2(column * distance, row * distance);
        newCellObj.name = string.Format("Cell {0}, {1}", row, column);
        newCellObj.transform.SetParent(cellHolderObj.transform);
        newCellObj.transform.localScale = new Vector3(cellScale, cellScale, cellScale);

        Cell newCell = newCellObj.GetComponent<Cell>();

        newCell.Row = row;
        newCell.Col = column;
        newCell.Type = type;

        return newCell;
    }

    #endregion

    #region WallBuilding
    // for indexing easiness, creation of vertical walls and horizontal walls is separated.
    // moreover, in practice we also separate the creation of interior and exterior walls, but this is not a system requirement.

    public async Task BuildInteriorWalls()
    {
        if (cellHolderObj == null || cells == null)
        {
            Debug.Log("no valid grid present");
            return;
        }

        if (interiorWallHolderObj != null)
        {
            DestroyImmediate(interiorWallHolderObj);
        }

        interiorWalls = new Wall[dimension, 2 * dimension - 1]; // see WallIndexing class
        WallIndexing.squareDimension = dimension;

        interiorWallHolderObj = new GameObject("InteriorWalls");
        interiorWallHolderObj.transform.SetParent(transform);

        for (int i = 0; i < dimension; i++)
        {
            await BuildWallColumn(i, false, true);
        }

        for (int j = 0; j < dimension; j++)
        {
            await BuildWallRow(j, false, true);
        }

        Level.walls = interiorWalls;
        Level.NumWalls = 2 * dimension * (dimension - 1);
    }

    public async Task BuildExteriorWalls()
    {
        if (cellHolderObj == null || cells == null)
        {
            Debug.Log("no valid grid present");
            return;
        }

        if (exteriorWallHolderObj != null)
        {
            DestroyImmediate(exteriorWallHolderObj);
        }

        exteriorWallHolderObj = new GameObject("ExteriorWalls");
        exteriorWallHolderObj.transform.SetParent(transform);

        for (int i = 0; i < dimension; i++)
        {
            await BuildWallColumn(i, true, false);
        }

        for (int j = 0; j < dimension; j++)
        {
            await BuildWallRow(j, true, false);
        }
    }

    // row of vertical walls
    async Task BuildWallRow(int j, bool exterior, bool interior)
    {
        Vector3 offset = new Vector3(cellDistance / 2, 0, 0);

        if (interior)
        {
            for (int i = 0; i < dimension - 1; i++)
            {
                WallIndexes wallIndexes = WallIndexing.IndexesOfWallInBetween(j, i, j, i + 1);
                interiorWalls[wallIndexes.row, wallIndexes.col] = BuildWall(cells[j, i].transform.position + offset, true, interiorWallHolderObj);
                await Task.Delay((int)(waitDuration * 1000f));
            }
        }

        if (exterior)
        {
            BuildWall(cells[j, 0].transform.position - offset, true, exteriorWallHolderObj);
            await Task.Delay((int)(waitDuration * 1000f));
            BuildWall(cells[j, dimension - 1].transform.position + offset, true, exteriorWallHolderObj);
            await Task.Delay((int)(waitDuration * 1000f));
        }
    }

    // column of horizontal walls
    async Task BuildWallColumn(int i, bool exterior, bool interior)
    {
        Vector3 offset = new Vector3(0, cellDistance / 2, 0);

        if (interior)
        {
            for (int j = 0; j < dimension - 1; j++)
            {
                WallIndexes wallIndexes = WallIndexing.IndexesOfWallInBetween(j, i, j + 1, i);
                interiorWalls[wallIndexes.row, wallIndexes.col] = BuildWall(cells[j, i].transform.position + offset, false, interiorWallHolderObj);
                await Task.Delay((int)(waitDuration * 1000f));
            }
        }

        if (exterior)
        {
            BuildWall(cells[0, i].transform.position - offset, false, exteriorWallHolderObj);
            await Task.Delay((int)(waitDuration * 1000f));
            BuildWall(cells[dimension - 1, i].transform.position + offset, false, exteriorWallHolderObj);
            await Task.Delay((int)(waitDuration * 1000f));
        }
    }

    Wall BuildWall(Vector3 position, bool vertical, GameObject parent)
    {
        GameObject wallPrefab = prefabLoader.GetWallPrefab();

        GameObject newWallObj = Instantiate(wallPrefab);

        newWallObj.transform.position = position;
        if (!vertical)
        {
            newWallObj.transform.Rotate(Vector3.forward, 90.0f);
        }
        newWallObj.transform.SetParent(parent.transform);

        Wall newWall = newWallObj.GetComponent<Wall>();
        newWall.Open = false;

        return newWall;
    }

    #endregion

    public async Task BuildAll()
    {
        if(cells != null)
        {
            DestroyAll();
        }

        await BuildSquareGrid();
        await BuildExteriorWalls();
        await BuildInteriorWalls();
    }

    //public void AssignCellTypes()
    //{
    //    if (cells == null)
    //    {
    //        Debug.Log("no valid grid present");
    //        return;
    //    }

    //    for (int j = 0; j < dimension; j++)
    //    {
    //        for (int i = 0; i < dimension; i++)
    //        {
    //            cells[j, i].Type = DeriveCellType(j, i);
    //        }
    //    }
    //}

    // we assume a strict cell type scheme -> the type can be derived based only on the cell position.
    private CellType DeriveCellType(int j, int i)
    {
        CellType type;

        if (i + j == 0)
        {
            type = CellType.Enter;
        }

        else if (i + j == 1)
        {
            type = CellType.PostEnter;
        }

        else if (i + j == dimension - 1 && i * j == 0)
        {
            type = CellType.Loot;
        }

        else if (i + j == (dimension - 1) * 2 - 1)
        {
            type = CellType.PreBoss;
        }

        else if (i + j == (dimension - 1) * 2)
        {
            type = CellType.Boss;
        }

        else if (i + j == dimension - 1)
        {
            if (Random.value < 0.5f)
            {
                type = CellType.Easy;
            }
            else
            {
                type = CellType.Hard;
            }
        }

        else if (i + j < dimension - 1)
        {
            type = CellType.Easy;
        }

        else
        {
            type = CellType.Hard;
        }

        return type;
    }

    #region Clearing

    public void DestroyCellGrid()
    {
        DestroyImmediate(cellHolderObj);

        cellHolderObj = null;
        cells = null;
    }

    public void DestroyInteriorWalls()
    {
        DestroyImmediate(interiorWallHolderObj);

        interiorWallHolderObj = null;
    }

    public void DestroyExteriorWalls()
    {
        DestroyImmediate(exteriorWallHolderObj);

        exteriorWallHolderObj = null;
    }

    public void DestroyAll()
    {
        DestroyCellGrid();
        DestroyExteriorWalls();
        DestroyInteriorWalls();
    }

    #endregion
}

