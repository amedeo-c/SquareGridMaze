using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LevelColors
{
    static List<Color> cellColors;

    static Color closedWallColor;
    static Color openWallColor;

    static ColorSetup setup;

    public static Color GetCellColor(CellType type)
    {
        if(setup == null)
        {
            setup = Resources.Load<ColorSetup>("BaseColorSetup");
        }

        return setup.cellColors[(int)type];
    }

    public static Color GetWallColor(bool open)
    {
        if(setup == null)
        {
            ColorSetup setupScriptable = Resources.Load<ColorSetup>("BaseColorSetup");
            setup = ScriptableObject.Instantiate(setupScriptable);
        }

        if (open)
        {
            return setup.openWallColor;
        }
        else
        {
            return setup.closedWallColor;
        }
    }

}
