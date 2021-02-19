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
        serializedObject.UpdateIfRequiredOrScript();

        if(GUILayout.Button("direct traverse method"))
        {
            serializedObject.ApplyModifiedProperties();

            MapExploring m = target as MapExploring;
            m.DirectTraverseMethod();
        }

        if(GUILayout.Button("random traverse method"))
        {
            serializedObject.ApplyModifiedProperties();

            MapExploring m = target as MapExploring;
            m.RandomTraverseMethod();
        }
    }
}

