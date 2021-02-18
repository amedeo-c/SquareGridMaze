using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    Cell cellA;
    Cell cellB;

    bool open;

    public bool Open
    {
        get
        {
            return open;
        }
        set
        {
            if(open != value)
            {
                transform.Rotate(Vector3.forward, 90.0f);
            }

            open = value;
            GetComponent<SpriteRenderer>().color = LevelColors.GetWallColor(open);
        }
    }
}
