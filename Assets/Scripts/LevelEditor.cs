using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

[CustomEditor(typeof(Level))]
public class LevelEditor : Editor
{
    SerializedProperty square;
    SerializedProperty dimension;
    SerializedProperty numRows;
    SerializedProperty numColumns;
    SerializedProperty cellDistance;

    private void OnEnable()
    {
        square = serializedObject.FindProperty("square");
        dimension = serializedObject.FindProperty("dimension");
        cellDistance = serializedObject.FindProperty("cellDistance");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.UpdateIfRequiredOrScript();

        EditorGUILayout.PropertyField(square, new GUIContent("square"));

        EditorGUI.indentLevel++;
        if (square.boolValue)
        {
            EditorGUILayout.PropertyField(dimension, new GUIContent("dimension"));
        }
        else
        {
            EditorGUILayout.PropertyField(numRows, new GUIContent("num rows"));
            EditorGUILayout.PropertyField(numColumns, new GUIContent("num columns"));
        }
        EditorGUI.indentLevel--;

        EditorGUILayout.Slider(cellDistance, 1, 3);

        serializedObject.ApplyModifiedProperties();

        if (GUILayout.Button("clear"))
        {
            serializedObject.ApplyModifiedProperties();

            Level l = target as Level;
            l.DestroyCellGrid();
            l.DestroyInteriorWalls();
            l.DestroyExteriorWalls();
        }

        if (GUILayout.Button("build grid"))
        {
            serializedObject.ApplyModifiedProperties();

            if (square.boolValue)
            {
                Level l = target as Level;
                l.CreateSquareGrid();
            }
        }

        if (GUILayout.Button("build external walls"))
        {
            Level l = target as Level;
            l.BuildExteriorWalls();
        }

        if (GUILayout.Button("build internal walls"))
        {
            Level l = target as Level;
            l.BuildInteriorWalls();
        }

        if (GUILayout.Button("assign types"))
        {
            serializedObject.ApplyModifiedProperties();

            Level l = target as Level;
            l.AssignCellTypes();
        }
    }
}
