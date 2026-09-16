
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RoadGenerator))]
public class RoadGeneratorEditor : Editor
{
    private RoadGenerator road;

    private void OnEnable()
    {
        road = (RoadGenerator)target;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(
            serializedObject.FindProperty("terrain")
        );

        EditorGUILayout.Space();

        EditorGUILayout.LabelField(
            "Road",
            EditorStyles.boldLabel
        );

        EditorGUILayout.PropertyField(
            serializedObject.FindProperty("roadWidth")
        );

        EditorGUILayout.PropertyField(
            serializedObject.FindProperty("roadHeightOffset")
        );

        EditorGUILayout.PropertyField(
            serializedObject.FindProperty("pointSpacing")
        );

        EditorGUILayout.PropertyField(
            serializedObject.FindProperty("curveSmoothness")
        );

        EditorGUILayout.Space();

        EditorGUILayout.LabelField(
            "Shoulders",
            EditorStyles.boldLabel
        );

        EditorGUILayout.PropertyField(
            serializedObject.FindProperty("shoulderWidth")
        );

        EditorGUILayout.PropertyField(
            serializedObject.FindProperty("shoulderHeightOffset")
        );

        EditorGUILayout.Space();

        EditorGUILayout.LabelField(
            "Center Line",
            EditorStyles.boldLabel
        );

        EditorGUILayout.PropertyField(
            serializedObject.FindProperty("generateCenterLine")
        );

        EditorGUILayout.PropertyField(
            serializedObject.FindProperty("centerLineWidth")
        );

        EditorGUILayout.PropertyField(
            serializedObject.FindProperty("centerLineHeight")
        );

        EditorGUILayout.Space();

        EditorGUILayout.LabelField(
            "Materials",
            EditorStyles.boldLabel
        );

        EditorGUILayout.PropertyField(
            serializedObject.FindProperty("roadMaterial")
        );

        EditorGUILayout.PropertyField(
            serializedObject.FindProperty("shoulderMaterial")
        );

        EditorGUILayout.PropertyField(
            serializedObject.FindProperty("centerLineMaterial")
        );

        EditorGUILayout.Space();

        EditorGUILayout.LabelField(
            "Collision",
            EditorStyles.boldLabel
        );

        EditorGUILayout.PropertyField(
            serializedObject.FindProperty("generateCollider")
        );

        EditorGUILayout.Space();

        EditorGUILayout.LabelField(
            "Terrain Vegetation",
            EditorStyles.boldLabel
        );

        EditorGUILayout.PropertyField(
            serializedObject.FindProperty("clearVegetationUnderRoad")
        );

        EditorGUILayout.PropertyField(
            serializedObject.FindProperty("vegetationClearance")
        );

        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.Space();

        EditorGUILayout.HelpBox(
            "SHIFT + ЛКМ по Terrain — додати точку дороги.",
            MessageType.Info
        );

        EditorGUILayout.LabelField(
            "Points: " + road.points.Count
        );

        EditorGUILayout.Space();

        if (GUILayout.Button("Generate Road"))
        {
            Undo.RecordObject(road, "Generate Road");

            road.GenerateRoad();

            EditorUtility.SetDirty(road);
        }

        if (GUILayout.Button("Clear Road"))
        {
            Undo.RecordObject(road, "Clear Road");

            road.ClearMesh();

            EditorUtility.SetDirty(road);
        }

        if (GUILayout.Button("Clear Points And Road"))
        {
            Undo.RecordObject(road, "Clear Points And Road");

            road.ClearPoints();

            EditorUtility.SetDirty(road);
        }
    }

    private void OnSceneGUI()
    {
        Event e = Event.current;

        if (road.terrain == null)
        {
            return;
        }

        DrawPoints();
        DrawPath();

        if (e.shift &&
            e.type == EventType.MouseDown &&
            e.button == 0)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(
                e.mousePosition
            );

            if (Physics.Raycast(
                ray,
                out RaycastHit hit,
                10000f
            ))
            {
                TerrainCollider terrainCollider =
                    hit.collider.GetComponent<TerrainCollider>();

                if (terrainCollider != null)
                {
                    Undo.RecordObject(
                        road,
                        "Add Road Point"
                    );

                    Vector3 point = hit.point;

                    if (road.points.Count == 0 ||
                        Vector3.Distance(
                            road.points[road.points.Count - 1],
                            point
                        ) > road.pointSpacing)
                    {
                        road.points.Add(point);

                        road.GenerateRoad();

                        EditorUtility.SetDirty(road);
                    }

                    e.Use();
                }
            }
        }

        if (e.shift &&
            e.type == EventType.MouseMove)
        {
            SceneView.RepaintAll();
        }
    }

    private void DrawPoints()
    {
        Handles.color = Color.yellow;

        for (int i = 0; i < road.points.Count; i++)
        {
            Handles.SphereHandleCap(
                0,
                road.points[i],
                Quaternion.identity,
                0.2f,
                EventType.Repaint
            );

            Handles.Label(
                road.points[i] + Vector3.up * 0.3f,
                i.ToString()
            );
        }
    }

    private void DrawPath()
    {
        if (road.points.Count < 2)
        {
            return;
        }

        Handles.color = Color.cyan;

        for (int i = 0; i < road.points.Count - 1; i++)
        {
            Handles.DrawLine(
                road.points[i],
                road.points[i + 1],
                3f
            );
        }
    }
}
