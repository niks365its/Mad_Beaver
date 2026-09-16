
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class RoadGenerator : MonoBehaviour
{
    [Header("Terrain")]
    public Terrain terrain;

    [Header("Road")]
    [Min(0.1f)]
    public float roadWidth = 5f;

    [Min(0.01f)]
    public float roadHeightOffset = 0.03f;

    [Min(0.1f)]
    public float pointSpacing = 1f;

    [Range(0f, 1f)]
    public float curveSmoothness = 0.5f;

    [Header("Shoulders")]
    [Min(0f)]
    public float shoulderWidth = 0.8f;

    [Min(0f)]
    public float shoulderHeightOffset = 0.01f;

    [Header("Center Line")]
    public bool generateCenterLine = true;

    [Min(0.01f)]
    public float centerLineWidth = 0.12f;

    [Min(0.001f)]
    public float centerLineHeight = 0.02f;

    [Header("Materials")]
    public Material roadMaterial;
    public Material shoulderMaterial;
    public Material centerLineMaterial;

    [Header("Collision")]
    public bool generateCollider = true;

    [Header("Remove Terrain Vegetation")]
    public bool clearVegetationUnderRoad = true;

    [Min(0f)]
    public float vegetationClearance = 0.2f;

    [Header("Path")]
    public List<Vector3> points = new List<Vector3>();

    private MeshFilter roadMeshFilter;
    private MeshRenderer roadMeshRenderer;
    private MeshCollider roadMeshCollider;

    private GameObject shoulderObject;
    private GameObject centerLineObject;

    private readonly List<Vector3> generatedPoints = new List<Vector3>();

    public void GenerateRoad()
    {
        if (terrain == null)
        {
            Debug.LogWarning("RoadGenerator: Terrain не вибраний.");
            return;
        }

        if (points == null || points.Count < 2)
        {
            ClearMesh();
            return;
        }

        EnsureComponents();

        generatedPoints.Clear();
        generatedPoints.AddRange(CreateSmoothPoints());

        if (generatedPoints.Count < 2)
        {
            ClearMesh();
            return;
        }

        GenerateRoadMesh();
        GenerateShoulders();
        GenerateCenterLine();

        if (clearVegetationUnderRoad)
        {
            ClearVegetation();
        }
    }

    private List<Vector3> CreateSmoothPoints()
    {
        List<Vector3> result = new List<Vector3>();

        if (points.Count < 2)
        {
            return result;
        }

        if (points.Count == 2)
        {
            AddLinearPoints(result, points[0], points[1]);
            return result;
        }

        for (int i = 0; i < points.Count - 1; i++)
        {
            Vector3 p0 = i == 0 ? points[i] : points[i - 1];
            Vector3 p1 = points[i];
            Vector3 p2 = points[i + 1];
            Vector3 p3 = i + 2 < points.Count ? points[i + 2] : points[i + 1];

            float distance = Vector3.Distance(p1, p2);
            int steps = Mathf.Max(1, Mathf.CeilToInt(distance / pointSpacing));

            for (int j = 0; j < steps; j++)
            {
                float t = j / (float)steps;

                Vector3 position = CatmullRom(
                    p0,
                    p1,
                    p2,
                    p3,
                    t,
                    curveSmoothness
                );

                position.y = GetTerrainHeight(position) + roadHeightOffset;

                if (result.Count == 0 ||
                    Vector3.Distance(result[result.Count - 1], position) >= pointSpacing * 0.5f)
                {
                    result.Add(position);
                }
            }
        }

        Vector3 last = points[points.Count - 1];
        last.y = GetTerrainHeight(last) + roadHeightOffset;
        result.Add(last);

        return result;
    }

    private void AddLinearPoints(List<Vector3> result, Vector3 start, Vector3 end)
    {
        float distance = Vector3.Distance(start, end);
        int steps = Mathf.Max(1, Mathf.CeilToInt(distance / pointSpacing));

        for (int i = 0; i <= steps; i++)
        {
            float t = i / (float)steps;

            Vector3 point = Vector3.Lerp(start, end, t);
            point.y = GetTerrainHeight(point) + roadHeightOffset;

            result.Add(point);
        }
    }

    private Vector3 CatmullRom(
        Vector3 p0,
        Vector3 p1,
        Vector3 p2,
        Vector3 p3,
        float t,
        float smoothness)
    {
        float tension = Mathf.Lerp(1f, 0f, smoothness);

        Vector3 m1 = (p2 - p0) * tension * 0.5f;
        Vector3 m2 = (p3 - p1) * tension * 0.5f;

        float t2 = t * t;
        float t3 = t2 * t;

        return
            (2f * t3 - 3f * t2 + 1f) * p1 +
            (t3 - 2f * t2 + t) * m1 +
            (-2f * t3 + 3f * t2) * p2 +
            (t3 - t2) * m2;
    }

    private void GenerateRoadMesh()
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        for (int i = 0; i < generatedPoints.Count; i++)
        {
            Vector3 current = generatedPoints[i];

            Vector3 forward = GetForward(i);
            Vector3 right = new Vector3(forward.z, 0f, -forward.x);

            Vector3 leftPoint = current - right * (roadWidth * 0.5f);
            Vector3 rightPoint = current + right * (roadWidth * 0.5f);

            leftPoint.y = GetTerrainHeight(leftPoint) + roadHeightOffset;
            rightPoint.y = GetTerrainHeight(rightPoint) + roadHeightOffset;

            int index = vertices.Count;

            vertices.Add(transform.InverseTransformPoint(leftPoint));
            vertices.Add(transform.InverseTransformPoint(rightPoint));

            float uvY = i * pointSpacing / roadWidth;

            uvs.Add(new Vector2(0f, uvY));
            uvs.Add(new Vector2(1f, uvY));

            if (i < generatedPoints.Count - 1)
            {
                triangles.Add(index);
                triangles.Add(index + 2);
                triangles.Add(index + 1);

                triangles.Add(index + 1);
                triangles.Add(index + 2);
                triangles.Add(index + 3);
            }
        }

        Mesh mesh = new Mesh();
        mesh.name = "Road Mesh";

        if (vertices.Count > 65535)
        {
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        }

        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.SetUVs(0, uvs);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        roadMeshFilter.sharedMesh = mesh;
        roadMeshRenderer.sharedMaterial = roadMaterial;

        if (generateCollider)
        {
            roadMeshCollider.sharedMesh = null;
            roadMeshCollider.sharedMesh = mesh;
        }
    }

    private void GenerateShoulders()
    {
        EnsureShoulderObject();

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        float halfRoad = roadWidth * 0.5f;
        Transform shoulderTransform = shoulderObject.transform;

        for (int i = 0; i < generatedPoints.Count; i++)
        {
            Vector3 current = generatedPoints[i];

            Vector3 forward = GetForward(i);
            Vector3 right = new Vector3(forward.z, 0f, -forward.x);

            Vector3 roadLeft = current - right * halfRoad;
            Vector3 roadRight = current + right * halfRoad;

            Vector3 outerLeft = current - right * (halfRoad + shoulderWidth);
            Vector3 outerRight = current + right * (halfRoad + shoulderWidth);

            roadLeft.y = GetTerrainHeight(roadLeft) + roadHeightOffset + shoulderHeightOffset;
            roadRight.y = GetTerrainHeight(roadRight) + roadHeightOffset + shoulderHeightOffset;

            outerLeft.y = GetTerrainHeight(outerLeft) + roadHeightOffset + shoulderHeightOffset;
            outerRight.y = GetTerrainHeight(outerRight) + roadHeightOffset + shoulderHeightOffset;

            int index = vertices.Count;

            // Використовуємо shoulderTransform замість transform
            vertices.Add(shoulderTransform.InverseTransformPoint(roadLeft));
            vertices.Add(shoulderTransform.InverseTransformPoint(outerLeft));
            vertices.Add(shoulderTransform.InverseTransformPoint(roadRight));
            vertices.Add(shoulderTransform.InverseTransformPoint(outerRight));

            float uvY = i * pointSpacing;

            uvs.Add(new Vector2(0f, uvY));
            uvs.Add(new Vector2(1f, uvY));
            uvs.Add(new Vector2(0f, uvY));
            uvs.Add(new Vector2(1f, uvY));

            if (i < generatedPoints.Count - 1)
            {
                // Ліве узбіччя (виправлено обхід трикутників за годинниковою стрілкою)
                triangles.Add(index);
                triangles.Add(index + 1);
                triangles.Add(index + 4);

                triangles.Add(index + 1);
                triangles.Add(index + 5);
                triangles.Add(index + 4);

                // Праве узбіччя (виправлено обхід трикутників за годинниковою стрілкою)
                triangles.Add(index + 2);
                triangles.Add(index + 6);
                triangles.Add(index + 3);

                triangles.Add(index + 3);
                triangles.Add(index + 6);
                triangles.Add(index + 7);
            }
        }

        Mesh mesh = new Mesh();
        mesh.name = "Road Shoulders";

        if (vertices.Count > 65535)
        {
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        }

        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.SetUVs(0, uvs);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        MeshFilter filter = shoulderObject.GetComponent<MeshFilter>();
        MeshRenderer renderer = shoulderObject.GetComponent<MeshRenderer>();

        filter.sharedMesh = mesh;
        renderer.sharedMaterial = shoulderMaterial;
    }

    private void GenerateCenterLine()
    {
        EnsureCenterLineObject();

        if (!generateCenterLine)
        {
            centerLineObject.SetActive(false);
            return;
        }

        centerLineObject.SetActive(true);

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        float halfLine = centerLineWidth * 0.5f;

        for (int i = 0; i < generatedPoints.Count; i++)
        {
            Vector3 current = generatedPoints[i];

            Vector3 forward = GetForward(i);
            Vector3 right = new Vector3(forward.z, 0f, -forward.x);

            Vector3 left = current - right * halfLine;
            Vector3 rightPoint = current + right * halfLine;

            left.y = GetTerrainHeight(left) + roadHeightOffset + centerLineHeight;
            rightPoint.y = GetTerrainHeight(rightPoint) + roadHeightOffset + centerLineHeight;

            int index = vertices.Count;

            vertices.Add(transform.InverseTransformPoint(left));
            vertices.Add(transform.InverseTransformPoint(rightPoint));

            float uvY = i * pointSpacing;

            uvs.Add(new Vector2(0f, uvY));
            uvs.Add(new Vector2(1f, uvY));

            if (i < generatedPoints.Count - 1)
            {
                triangles.Add(index);
                triangles.Add(index + 2);
                triangles.Add(index + 1);

                triangles.Add(index + 1);
                triangles.Add(index + 2);
                triangles.Add(index + 3);
            }
        }

        Mesh mesh = new Mesh();
        mesh.name = "White Center Line";

        if (vertices.Count > 65535)
        {
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        }

        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.SetUVs(0, uvs);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        MeshFilter filter = centerLineObject.GetComponent<MeshFilter>();
        MeshRenderer renderer = centerLineObject.GetComponent<MeshRenderer>();

        filter.sharedMesh = mesh;
        renderer.sharedMaterial = centerLineMaterial;
    }

    private Vector3 GetForward(int index)
    {
        Vector3 forward;

        if (index == 0)
        {
            forward = generatedPoints[1] - generatedPoints[0];
        }
        else if (index == generatedPoints.Count - 1)
        {
            forward = generatedPoints[index] - generatedPoints[index - 1];
        }
        else
        {
            forward = generatedPoints[index + 1] - generatedPoints[index - 1];
        }

        forward.y = 0f;

        if (forward.sqrMagnitude < 0.0001f)
        {
            forward = Vector3.forward;
        }

        return forward.normalized;
    }

    private float GetTerrainHeight(Vector3 worldPosition)
    {
        return terrain.SampleHeight(worldPosition) + terrain.transform.position.y;
    }

    private void EnsureComponents()
    {
        roadMeshFilter = GetComponent<MeshFilter>();

        if (roadMeshFilter == null)
        {
            roadMeshFilter = gameObject.AddComponent<MeshFilter>();
        }

        roadMeshRenderer = GetComponent<MeshRenderer>();

        if (roadMeshRenderer == null)
        {
            roadMeshRenderer = gameObject.AddComponent<MeshRenderer>();
        }

        roadMeshCollider = GetComponent<MeshCollider>();

        if (generateCollider && roadMeshCollider == null)
        {
            roadMeshCollider = gameObject.AddComponent<MeshCollider>();
        }
    }

    private void EnsureShoulderObject()
    {
        if (shoulderObject != null)
        {
            return;
        }

        Transform existing = transform.Find("Shoulders");

        if (existing != null)
        {
            shoulderObject = existing.gameObject;
            return;
        }

        shoulderObject = new GameObject("Shoulders");
        shoulderObject.transform.SetParent(transform);
        shoulderObject.transform.localPosition = Vector3.zero;
        shoulderObject.transform.localRotation = Quaternion.identity;
        shoulderObject.transform.localScale = Vector3.one;

        shoulderObject.AddComponent<MeshFilter>();
        shoulderObject.AddComponent<MeshRenderer>();
    }

    private void EnsureCenterLineObject()
    {
        if (centerLineObject != null)
        {
            return;
        }

        Transform existing = transform.Find("White Center Line");

        if (existing != null)
        {
            centerLineObject = existing.gameObject;
            return;
        }

        centerLineObject = new GameObject("White Center Line");
        centerLineObject.transform.SetParent(transform);
        centerLineObject.transform.localPosition = Vector3.zero;
        centerLineObject.transform.localRotation = Quaternion.identity;
        centerLineObject.transform.localScale = Vector3.one;

        centerLineObject.AddComponent<MeshFilter>();
        centerLineObject.AddComponent<MeshRenderer>();
    }

    private void ClearVegetation()
    {
        if (terrain == null || terrain.terrainData == null)
        {
            return;
        }

        RemoveTrees();
        RemoveTerrainDetails();
    }

    private void RemoveTrees()
    {
        TerrainData data = terrain.terrainData;
        TreeInstance[] trees = data.treeInstances;

        if (trees.Length == 0)
        {
            return;
        }

        float terrainWidth = data.size.x;
        float terrainLength = data.size.z;

        List<TreeInstance> remainingTrees = new List<TreeInstance>();

        float clearWidth = roadWidth * 0.5f + shoulderWidth + vegetationClearance;

        for (int i = 0; i < trees.Length; i++)
        {
            TreeInstance tree = trees[i];

            Vector3 worldPosition = new Vector3(
                terrain.transform.position.x + tree.position.x * terrainWidth,
                0f,
                terrain.transform.position.z + tree.position.z * terrainLength
            );

            if (!IsPointNearRoad(worldPosition, clearWidth))
            {
                remainingTrees.Add(tree);
            }
        }

        data.SetTreeInstances(remainingTrees.ToArray(), false);
    }

    private void RemoveTerrainDetails()
    {
        TerrainData data = terrain.terrainData;

        int detailWidth = data.detailWidth;
        int detailHeight = data.detailHeight;

        float terrainWidth = data.size.x;
        float terrainLength = data.size.z;

        float clearWidth = roadWidth * 0.5f + shoulderWidth + vegetationClearance;

        for (int layer = 0; layer < data.detailPrototypes.Length; layer++)
        {
            int[,] details = data.GetDetailLayer(
                0,
                0,
                detailWidth,
                detailHeight,
                layer
            );

            bool changed = false;

            for (int z = 0; z < detailHeight; z++)
            {
                for (int x = 0; x < detailWidth; x++)
                {
                    if (details[z, x] == 0)
                    {
                        continue;
                    }

                    float normalizedX = (x + 0.5f) / detailWidth;
                    float normalizedZ = (z + 0.5f) / detailHeight;

                    Vector3 worldPosition = new Vector3(
                        terrain.transform.position.x + normalizedX * terrainWidth,
                        0f,
                        terrain.transform.position.z + normalizedZ * terrainLength
                    );

                    if (IsPointNearRoad(worldPosition, clearWidth))
                    {
                        details[z, x] = 0;
                        changed = true;
                    }
                }
            }

            if (changed)
            {
                data.SetDetailLayer(
                    0,
                    0,
                    layer,
                    details
                );
            }
        }
    }

    private bool IsPointNearRoad(Vector3 position, float width)
    {
        if (generatedPoints.Count < 2)
        {
            return false;
        }

        float widthSqr = width * width;

        for (int i = 0; i < generatedPoints.Count - 1; i++)
        {
            Vector3 a = generatedPoints[i];
            Vector3 b = generatedPoints[i + 1];

            a.y = 0f;
            b.y = 0f;

            Vector3 p = position;
            p.y = 0f;

            Vector3 closest = ClosestPointOnSegment(p, a, b);

            if ((p - closest).sqrMagnitude <= widthSqr)
            {
                return true;
            }
        }

        return false;
    }

    private Vector3 ClosestPointOnSegment(Vector3 point, Vector3 a, Vector3 b)
    {
        Vector3 direction = b - a;

        if (direction.sqrMagnitude < 0.0001f)
        {
            return a;
        }

        float t = Vector3.Dot(point - a, direction) / direction.sqrMagnitude;
        t = Mathf.Clamp01(t);

        return a + direction * t;
    }

    public void ClearMesh()
    {
        MeshFilter filter = GetComponent<MeshFilter>();

        if (filter != null)
        {
            filter.sharedMesh = null;
        }

        MeshCollider collider = GetComponent<MeshCollider>();

        if (collider != null)
        {
            collider.sharedMesh = null;
        }

        if (shoulderObject != null)
        {
            MeshFilter shoulderFilter = shoulderObject.GetComponent<MeshFilter>();

            if (shoulderFilter != null)
            {
                shoulderFilter.sharedMesh = null;
            }
        }

        if (centerLineObject != null)
        {
            MeshFilter lineFilter = centerLineObject.GetComponent<MeshFilter>();

            if (lineFilter != null)
            {
                lineFilter.sharedMesh = null;
            }
        }
    }

    public void ClearPoints()
    {
        points.Clear();
        generatedPoints.Clear();
        ClearMesh();
    }
}

