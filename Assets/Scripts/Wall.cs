using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Threading.Tasks;

public class Wall : MonoBehaviour
{
    Cell cellA;
    Cell cellB;

    bool open;

    Animation openAnimation;

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

    public async Task OpenWithAnimation()
    {
        if (!open)
        {
            if(openAnimation == null)
            {
                openAnimation = GetComponent<Animation>();
            }

            openAnimation.Play();

            while (openAnimation.isPlaying)
            {
                await Task.Yield();
            }
        }
    }
}
