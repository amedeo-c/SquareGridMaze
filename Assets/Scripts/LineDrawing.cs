using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

#if UNITY_EDITOR
public class LineDrawing : EditorWindow
{
    [MenuItem("Tools/Line Drawing")]
    public static void OpenWindows() => GetWindow<LineDrawing>("Line Drawing");

    public GameObject linkPrefab;
    public float linkLengthPercentage;

    Transform startingPoint;
    Transform arrivalPoint;

    GameObject linkHolder;

    SerializedObject so;
    SerializedProperty linkPrefabProperty;
    SerializedProperty linkLengthPercentageProperty;

    private void OnEnable()
    {
        so = new SerializedObject(this);
        linkPrefabProperty = so.FindProperty("linkPrefab");
        linkLengthPercentageProperty = so.FindProperty("linkLengthPercentage");

        SceneView.duringSceneGui += OnSceneGui;
        //Selection.selectionChanged += OnSelectionChange;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGui;
        //Selection.selectionChanged -= OnSelectionChange;
    }

    private void OnGUI()
    {
        GUILayout.Label("Line Drawing");

        EditorGUILayout.PropertyField(linkPrefabProperty, new GUIContent("Link Prefab"));
        EditorGUILayout.PropertyField(linkLengthPercentageProperty, new GUIContent("Link Length Percentage"));

        so.ApplyModifiedProperties();

        if(GUILayout.Button("Draw selected links"))
        {
            if(Selection.gameObjects.Length < 2)
            {
                return;
            }

            Vector3 startingPoint = Selection.activeTransform.position;
            for(int i = 0; i < Selection.gameObjects.Length; i++)
            {
                DrawLink(startingPoint, Selection.gameObjects[i].transform.position);
            }
        }

        if(GUILayout.Button("clear links"))
        {
            if(linkHolder == null)
            {
                return;
            }

            DestroyImmediate(linkHolder.gameObject);
            linkHolder = null;
        }
    }

    private void OnSceneGui(SceneView sceneView)
    {
        Event e = Event.current;

        if(e.keyCode == KeyCode.Mouse0 && e.type == EventType.KeyDown)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);

            Debug.Log(ray.origin);
        }
    }

    private void DrawLink(Vector3 start, Vector3 end)
    {
        if(linkHolder == null)
        {
            linkHolder = new GameObject("Links");
        }

        Vector3 linkPos = (start + end) / 2;
        Quaternion linkRot = Quaternion.LookRotation(Vector3.forward, end - start);
        float linkLength = (end - start).magnitude * linkLengthPercentage;

        GameObject link = Instantiate(linkPrefab, linkHolder.transform);
        link.transform.position = linkPos;
        link.transform.rotation = linkRot;
        link.transform.localScale = new Vector3(0.04f, linkLength, 1);
    }
}
#endif