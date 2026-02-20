using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class PartialHollowCylinderMesh : MonoBehaviour
{
    [Header("Basic Shape Settings")]
    [Range(3, 100)] public int segments = 20;
    [Range(0f, 360f)] public float OpeningAngle = 90f;
    public float innerRadius = 0.5f;
    public float outerRadius = 1f;
    public float height = 1f;

    [Header("Separation Settings")]
    public bool isSeparate = false;
    [Range(2, 4)] public int numberOfCuts = 2;
    [Range(1f, 30f)] public float cutAngleSize = 5f;

    void OnValidate() => GenerateMesh();

    public void GenerateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.name = "PartialHollowCylinder";

        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<int> triangles = new List<int>();

        float totalAngle = OpeningAngle;

        // Slicing logic
        List<(float start, float end)> arcSegments = new List<(float, float)>();
        if (isSeparate)
        {
            float totalCutAngle = numberOfCuts * cutAngleSize;
            float drawableAngle = Mathf.Max(0, totalAngle - totalCutAngle);
            float segmentAngle = drawableAngle / numberOfCuts;

            float angle = 0f;
            for (int i = 0; i < numberOfCuts; i++)
            {
                float start = angle;
                float end = angle + segmentAngle;
                arcSegments.Add((start, end));
                angle += segmentAngle + cutAngleSize;
            }
        }
        else
        {
            arcSegments.Add((0, totalAngle));
        }

        int vertOffset = 0;
        foreach (var (startDeg, endDeg) in arcSegments)
        {
            float arcAngle = endDeg - startDeg;
            int arcSegmentsCount = Mathf.CeilToInt(segments * (arcAngle / totalAngle));
            arcSegmentsCount = Mathf.Max(1, arcSegmentsCount); // prevent zero

            for (int i = 0; i <= arcSegmentsCount; i++)
            {
                float t = (float)i / arcSegmentsCount;
                float theta = Mathf.Deg2Rad * Mathf.Lerp(startDeg, endDeg, t);
                float cos = Mathf.Cos(theta);
                float sin = Mathf.Sin(theta);

                Vector3 innerBottom = new Vector3(cos * innerRadius, 0, sin * innerRadius);
                Vector3 innerTop = new Vector3(cos * innerRadius, height, sin * innerRadius);
                Vector3 outerBottom = new Vector3(cos * outerRadius, 0, sin * outerRadius);
                Vector3 outerTop = new Vector3(cos * outerRadius, height, sin * outerRadius);

                vertices.Add(innerBottom);
                vertices.Add(innerTop);
                vertices.Add(outerBottom);
                vertices.Add(outerTop);

                normals.Add(Vector3.down);
                normals.Add(Vector3.up);
                normals.Add(Vector3.down);
                normals.Add(Vector3.up);
            }

            int arcVertStart = vertOffset / 4;
            for (int i = 0; i < arcSegmentsCount; i++)
            {
                int i0 = arcVertStart + i;
                // Indices
                int iBottomIn(int idx) => (idx) * 4;
                int iTopIn(int idx) => (idx) * 4 + 1;
                int iBottomOut(int idx) => (idx) * 4 + 2;
                int iTopOut(int idx) => (idx) * 4 + 3;

                // Outer wall
                triangles.Add(vertOffset + iBottomOut(i));
                triangles.Add(vertOffset + iTopOut(i));
                triangles.Add(vertOffset + iBottomOut(i + 1));
                triangles.Add(vertOffset + iBottomOut(i + 1));
                triangles.Add(vertOffset + iTopOut(i));
                triangles.Add(vertOffset + iTopOut(i + 1));

                // Inner wall
                triangles.Add(vertOffset + iBottomIn(i + 1));
                triangles.Add(vertOffset + iTopIn(i));
                triangles.Add(vertOffset + iBottomIn(i));
                triangles.Add(vertOffset + iTopIn(i + 1));
                triangles.Add(vertOffset + iTopIn(i));
                triangles.Add(vertOffset + iBottomIn(i + 1));

                // Top face
                triangles.Add(vertOffset + iTopIn(i));
                triangles.Add(vertOffset + iTopIn(i + 1));
                triangles.Add(vertOffset + iTopOut(i));
                triangles.Add(vertOffset + iTopOut(i));
                triangles.Add(vertOffset + iTopIn(i + 1));
                triangles.Add(vertOffset + iTopOut(i + 1));

                // Bottom face
                triangles.Add(vertOffset + iBottomOut(i));
                triangles.Add(vertOffset + iBottomIn(i + 1));
                triangles.Add(vertOffset + iBottomIn(i));
                triangles.Add(vertOffset + iBottomOut(i + 1));
                triangles.Add(vertOffset + iBottomIn(i + 1));
                triangles.Add(vertOffset + iBottomOut(i));
            }

            // Add caps to each arc (optional but clean)
            triangles.Add(vertOffset + 0); // bottom in
            triangles.Add(vertOffset + 1); // top in
            triangles.Add(vertOffset + 2); // bottom out
            triangles.Add(vertOffset + 2); // bottom out
            triangles.Add(vertOffset + 1); // top in
            triangles.Add(vertOffset + 3); // top out

            int last = arcSegmentsCount;
            triangles.Add(vertOffset + last * 4 + 2); // bottom out
            triangles.Add(vertOffset + last * 4 + 1); // top in
            triangles.Add(vertOffset + last * 4 + 0); // bottom in
            triangles.Add(vertOffset + last * 4 + 3); // top out
            triangles.Add(vertOffset + last * 4 + 1); // top in
            triangles.Add(vertOffset + last * 4 + 2); // bottom out

            vertOffset += (arcSegmentsCount + 1) * 4;
        }

        mesh.SetVertices(vertices);
        mesh.SetNormals(normals);
        mesh.SetTriangles(triangles, 0);
        mesh.RecalculateNormals();

        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshCollider>().sharedMesh = mesh;
    }
}
