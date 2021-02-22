using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

#if UNITY_EDITOR

[CustomEditor(typeof(LevelExplorer))]
public class LevelExplorerEditor : Editor
{
    SerializedProperty maxTries;

    private void OnEnable()
    {
        maxTries = serializedObject.FindProperty("maxTries");
    }

    public override void OnInspectorGUI()
    {
        LevelExplorer m = target as LevelExplorer;

        base.OnInspectorGUI();
        serializedObject.UpdateIfRequiredOrScript();

        if(m.traverseMode == TraverseMode.Random)
        {
            EditorGUILayout.PropertyField(maxTries, new GUIContent("maxTries"));
        }

        serializedObject.ApplyModifiedProperties();

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

#endif

