using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public int Row { get; set; }
    public int Col { get; set; }

    public CellType type = CellType.Default;

    public bool marked;

    public CellType Type
    {
        set
        {
            type = value;
            GetComponent<SpriteRenderer>().color = LevelColors.GetCellColor(type);
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
