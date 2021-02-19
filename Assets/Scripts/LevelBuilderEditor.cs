using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

[CustomEditor(typeof(LevelBuilder))]
public class LevelBuilderEditor : Editor
{
    SerializedProperty dimension;
    SerializedProperty cellDistance;
    SerializedProperty cellScale;
    SerializedProperty waitDuration;

    private void OnEnable()
    {
        dimension = serializedObject.FindProperty("dimension");
        cellDistance = serializedObject.FindProperty("cellDistance");
        cellScale = serializedObject.FindProperty("cellScale");
        waitDuration = serializedObject.FindProperty("waitDuration");
    }

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();

        serializedObject.UpdateIfRequiredOrScript();

        EditorGUILayout.PropertyField(dimension, new GUIContent("dimension"));

        EditorGUILayout.Slider(cellDistance, 0.5f, 5);

        EditorGUILayout.Slider(cellScale, 0.1f, 3);

        EditorGUILayout.PropertyField(waitDuration, new GUIContent("wait duration"));

        serializedObject.ApplyModifiedProperties();

        if (GUILayout.Button("clear"))
        {
            serializedObject.ApplyModifiedProperties();

            LevelBuilder l = target as LevelBuilder;
            l.DestroyCellGrid();
            l.DestroyInteriorWalls();
            l.DestroyExteriorWalls();
        }

        if(GUILayout.Button("build all"))
        {
            serializedObject.ApplyModifiedProperties();

            LevelBuilder l = target as LevelBuilder;
            l.BuildAll();
        }

        if (GUILayout.Button("build grid"))
        {
            serializedObject.ApplyModifiedProperties();

            LevelBuilder l = target as LevelBuilder;
            l.BuildSquareGrid();
        }

        if (GUILayout.Button("build external walls"))
        {
            serializedObject.ApplyModifiedProperties();

            LevelBuilder l = target as LevelBuilder;
            l.BuildExteriorWalls();
        }

        if (GUILayout.Button("build internal walls"))
        {
            serializedObject.ApplyModifiedProperties();

            LevelBuilder l = target as LevelBuilder;
            l.BuildInteriorWalls();
        }
    }
}
