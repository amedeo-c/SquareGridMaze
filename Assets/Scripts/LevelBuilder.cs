using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

public class LevelBuilder : EditorWindow
{
    [MenuItem("Tools/Level Builder")]
    public static void OpenWindows() => GetWindow<LevelBuilder>("Level Builder");
}
