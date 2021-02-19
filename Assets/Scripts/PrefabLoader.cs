using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;

public class PrefabLoader : MonoBehaviour
{
    public string cellsFolderPath;
    public string wallsFolderPath;

    public bool alwaysReload;

    List<GameObject[]> cellPrefabs;

    GameObject wallPrefab;

    void LoadCellPrefabs()
    {
        cellPrefabs = new List<GameObject[]>();

        string[] cellTypeNames = System.Enum.GetNames(typeof(CellType));

        foreach(string n in cellTypeNames)
        {
            //string resourcesPath = Path.Combine(cellsFolderPath, n);
            string resourcesPath = cellsFolderPath + "/" + n + "/";

            GameObject[] typePrefabs = Resources.LoadAll<GameObject>(resourcesPath);
            cellPrefabs.Add(typePrefabs);
        }
    }

    void LoadWallPrefab() // only one at the moment
    {
        string resourcePath = wallsFolderPath + "/Wall";
        wallPrefab = Resources.Load<GameObject>(resourcePath);
        Debug.Assert(wallPrefab != null);
    }

    public GameObject GetRandomPrefab(CellType type)
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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
