using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using System.Linq;

public class PrefabLoader : MonoBehaviour
{
    [Tooltip("relative path of the resource folder containing cell prefebs subfolders")]
    public string cellsFolderPath = "Prefabs/CellPrefabs";

    [Tooltip("relative path of the resource folder containing wall prefab")]
    public string wallsFolderPath = "Prefabs/WallPrefabs";

    [Tooltip("useful in edit mode to apply eventual changes")]
    public bool alwaysReload;

    List<GameObject[]> cellPrefabs;

    GameObject wallPrefab;

    // for both cells and walls prefabs, we load all the necessary resources once and for all and then 

    // we assume that for each cell type there exists a resources folder containing all the possible type variants
    void LoadCellPrefabs()
    {
        cellPrefabs = new List<GameObject[]>();

        string[] cellTypeNames = System.Enum.GetNames(typeof(CellType));

        foreach(string n in cellTypeNames)
        {
            string resourcesPath = cellsFolderPath + "/" + n + "/";

            GameObject[] typePrefabs = Resources.LoadAll<GameObject>(resourcesPath);

            Debug.Assert(typePrefabs.All(item => item.GetComponent<Cell>() != null));

            cellPrefabs.Add(typePrefabs);
        }
    }

    void LoadWallPrefab() // only one at the moment
    {
        string resourcePath = wallsFolderPath + "/Wall";
        wallPrefab = Resources.Load<GameObject>(resourcePath);
        Debug.Assert(wallPrefab != null);
        Debug.Assert(wallPrefab.GetComponent<Wall>() != null);
    }

    public GameObject GetRandomCellPrefab(CellType type)
    {
        if(cellPrefabs == null || alwaysReload)
        {
            LoadCellPrefabs();
        }

        GameObject[] typePrefabs = cellPrefabs[(int)type];
        return typePrefabs[Random.Range(0, typePrefabs.Length)];
    }

    public GameObject GetWallPrefab()
    {
        if(wallPrefab == null || alwaysReload)
        {
            LoadWallPrefab();
        }

        return wallPrefab;
    }
}
