using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

public class Level : MonoBehaviour
{
    public bool square;
    public int dimension;

    public static int Dimension;
    public static int NumWalls;

    public float cellDistance;

    public static Cell[,] cells;
    public static Wall[,] walls;

    GameObject cellPrefab;
    GameObject wallPrefab;

    Vector2 firstCellPosition;
    GameObject cellHolderObj;
    GameObject interiorWallHolderObj;
    GameObject exteriorWallHolderObj;

#if UNITY_EDITOR

    #region CellBuilding

    public void CreateSquareGrid()
    {
        if(cells != null)
        {
            Debug.Log("grid alreay created.");
            return;
        }

        if(cellHolderObj != null)
        {
            Debug.Log("grid obj already present");
            return;
        }

        Dimension = dimension;
        NumWalls = 2 * dimension * (dimension - 1);

        cellPrefab = Resources.Load<GameObject>("Prefabs/Cell");

        cellHolderObj = new GameObject("Cells");

        cellHolderObj.transform.SetParent(transform);
        cells = new Cell[Dimension, Dimension];

        float offSetFromCenter = (Dimension - 0.9999f) / 2 * cellDistance;
        firstCellPosition = new Vector2(-offSetFromCenter, -offSetFromCenter);

        for(int j = 0; j < Dimension; j++)
        {
            for(int i = 0; i < Dimension; i++)
            {
                cells[j, i] = CreateCell(j, i, cellDistance);
            }
        }

        //Debug.Log("square grid created.");
    }

    public Cell CreateCell(int row, int column, float distance)
    {
        GameObject newCellObj = Instantiate(cellPrefab);

        newCellObj.transform.position = firstCellPosition + new Vector2(column * distance, row * distance);
        newCellObj.name = string.Format("Cell {0}, {1}", row, column);
        newCellObj.transform.SetParent(cellHolderObj.transform);

        Cell newCell = newCellObj.GetComponent<Cell>();

        newCell.Row = row;
        newCell.Col = column;

        return newCell;
    }

    #endregion

    #region WallBuilding

    public void BuildInteriorWalls()
    {
        if(cellHolderObj == null || cells == null)
        {
            Debug.Log("no valid grid present");
            return;
        }

        if(interiorWallHolderObj != null)
        {
            DestroyImmediate(interiorWallHolderObj);
        }

        walls = new Wall[Dimension, 2 * Dimension - 1];
        WallIndexing.squareDimension = Dimension;

        wallPrefab = Resources.Load<GameObject>("Prefabs/Wall");

        interiorWallHolderObj = new GameObject("InteriorWalls");
        interiorWallHolderObj.transform.SetParent(transform);

        for(int i = 0; i < Dimension; i++)
        {
            BuildWallColumn(i, false, true);
        }

        for(int j = 0; j < Dimension; j++)
        {
            BuildWallRow(j, false, true);
        }
    }

    public void BuildExteriorWalls()
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

        wallPrefab = Resources.Load<GameObject>("Prefabs/Wall");

        exteriorWallHolderObj = new GameObject("ExteriorWalls");
        exteriorWallHolderObj.transform.SetParent(transform);

        for (int i = 0; i < Dimension; i++)
        {
            BuildWallColumn(i, true, false);
        }

        for (int j = 0; j < Dimension; j++)
        {
            BuildWallRow(j, true, false);
        }
    }

    // row of vertical walls
    void BuildWallRow(int j, bool exterior, bool interior)
    {
        Vector3 offset = new Vector3(cellDistance / 2, 0, 0);

        if (interior)
        {
            for (int i = 0; i < Dimension - 1; i++)
            {
                WallIndexes wallIndexes = WallIndexing.IndexesOfWallInBetween(j, i, j, i + 1);
                walls[wallIndexes.row, wallIndexes.col] = BuildWall(cells[j, i].transform.position + offset, true, interiorWallHolderObj);
            }
        }

        if (exterior)
        {
            BuildWall(cells[j, 0].transform.position - offset, true, exteriorWallHolderObj);
            BuildWall(cells[j, Dimension - 1].transform.position + offset, true, exteriorWallHolderObj);
        }
    }

    // column of horizontal walls
    void BuildWallColumn(int i, bool exterior, bool interior)
    {
        Vector3 offset = new Vector3(0, cellDistance / 2, 0);

        if (interior)
        {
            for (int j = 0; j < Dimension - 1; j++)
            {
                WallIndexes wallIndexes = WallIndexing.IndexesOfWallInBetween(j, i, j + 1, i);
                walls[wallIndexes.row, wallIndexes.col] = BuildWall(cells[j, i].transform.position + offset, false, interiorWallHolderObj);
            }
        }

        if (exterior)
        {
            BuildWall(cells[0, i].transform.position - offset, false, exteriorWallHolderObj);
            BuildWall(cells[Dimension - 1, i].transform.position + offset, false, exteriorWallHolderObj);
        }
    }

    Wall BuildWall(Vector3 position, bool vertical, GameObject parent)
    {
        
        GameObject newWallObj = Instantiate(wallPrefab);
        newWallObj.transform.position = position;
        if (!vertical)
        {
            newWallObj.transform.Rotate(Vector3.forward, 90.0f);
        }
        newWallObj.transform.SetParent(parent.transform);

        Wall newWall = newWallObj.GetComponent<Wall>();
        newWall.Close();

        PrefabUtility.RecordPrefabInstancePropertyModifications(newWall);

        return newWall;
    }

    #endregion

#endif

    public void AssignCellTypes()
    {
        if (cells == null)
        {
            Debug.Log("no valid grid present");
            return;
        }

        for (int j = 0; j < Dimension; j++)
        {
            for (int i = 0; i < Dimension; i++)
            {
                cells[j, i].Type = DeriveCellType(j, i);
            }
        }
    }

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

        else if (i + j == Dimension - 1 && i * j == 0)
        {
            type = CellType.Loot;
        }

        else if (i + j == (Dimension - 1) * 2 - 1)
        {
            type = CellType.PreBoss;
        }

        else if (i + j == (Dimension - 1) * 2)
        {
            type = CellType.Boss;
        }

        else if (i + j == Dimension - 1)
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

        else if (i + j < Dimension - 1)
        {
            type = CellType.Easy;
        }

        else
        {
            type = CellType.Hard;
        }

        return type;
    }

#if UNITY_EDITOR

    public void DestroyAllCells()
    {
        foreach(Cell c in FindObjectsOfType<Cell>())
        {
            DestroyImmediate(c.gameObject);
        }

        cells = null;
    }
    
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

#endif

    // Start is called before the first frame update
    void Awake()
    {
        CreateSquareGrid();
        BuildExteriorWalls();
        BuildInteriorWalls();
        AssignCellTypes();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}