using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
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

    public void Close()
    {
        if (open)
        {
            open = false;
            transform.Rotate(Vector3.forward, -90.0f);
        }

        GetComponent<SpriteRenderer>().color = LevelColors.GetWallColor(false);
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
