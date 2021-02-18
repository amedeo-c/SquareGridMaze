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

        if(GUILayout.Button("direct traverse method"))
        {
            MapExploring m = target as MapExploring;
            m.DirectTraverseMethod();
        }

        if(GUILayout.Button("random traverse method"))
        {
            MapExploring m = target as MapExploring;
            m.RandomTraverseMethod();
        }
    }
}

