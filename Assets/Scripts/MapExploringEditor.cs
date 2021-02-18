using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

[CustomEditor(typeof(MapExploring))]
public class MapExploringEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if(GUILayout.Button("traverse method"))
        {
            MapExploring m = target as MapExploring;
            m.TraverseMethod();
        }
    }
}

