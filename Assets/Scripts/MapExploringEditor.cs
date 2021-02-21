using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

[CustomEditor(typeof(MapExploring))]
public class MapExploringEditor : Editor
{
    SerializedProperty maxTries;

    private void OnEnable()
    {
        maxTries = serializedObject.FindProperty("maxTries");
    }

    public override void OnInspectorGUI()
    {
        MapExploring m = target as MapExploring;

        base.OnInspectorGUI();
        serializedObject.UpdateIfRequiredOrScript();

        if(m.traverseMode == TraverseMode.Random)
        {
            EditorGUILayout.PropertyField(maxTries, new GUIContent("maxTries"));
        }

        if (GUILayout.Button("clear"))
        {
            serializedObject.ApplyModifiedProperties();

            m.Clear();
        }

        if (GUILayout.Button("traverse"))
        {
            serializedObject.ApplyModifiedProperties();

            m.Traverse();
        }

        //if (GUILayout.Button("direct traverse method"))
        //{
        //    serializedObject.ApplyModifiedProperties();

        //    MapExploring m = target as MapExploring;
        //    m.DirectTraverseMethod();
        //}

        //if(GUILayout.Button("random traverse method"))
        //{
        //    serializedObject.ApplyModifiedProperties();

        //    MapExploring m = target as MapExploring;
        //    m.RandomTraverseMethod();
        //}
    }
}

