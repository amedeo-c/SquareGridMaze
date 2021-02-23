using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Threading.Tasks;

public class Wall : MonoBehaviour
{
    bool open;

    public static bool deactivateOnOpen;

    bool highlighted;

    public bool Open
    {
        get
        {
            return open;
        }
        set
        {
            if (deactivateOnOpen)
            {
                gameObject.SetActive(!value);
            }
            else
            {
                if (open != value)
                {
                    transform.Rotate(Vector3.forward, 90.0f);
                }
                GetComponent<SpriteRenderer>().color = LevelColors.GetWallColor(value);
            }

            open = value;
        }
    }

    public bool Highlighted
    {
        set
        {
            highlighted = value;

            if (highlighted)
            {
                GetComponent<SpriteRenderer>().color = Color.blue;
            }
            else
            {
                if (open)
                {
                    GetComponent<SpriteRenderer>().color = Color.white;
                }
                else
                {
                    GetComponent<SpriteRenderer>().color = Color.black;
                }
            }
        }
    }
}
