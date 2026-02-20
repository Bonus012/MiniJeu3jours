using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class KillingZoneMesh : MonoBehaviour
{
    public float innerRadius = 0.8f;
    public float outerRadius = 1f;
    public float height = 0.05f;
    public float angle = 15f;
    public int arcResolution = 6;

    void OnValidate()
    {
        Generate();
    }

    void Generate()
    {
        MeshFilter mf = GetComponent<MeshFilter>();
        mf.sharedMesh = GenerateWedge();
    }

    public Mesh GenerateWedge()
    {
        Mesh mesh = new Mesh();
        mesh.name = "KillingZoneWedge";

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        float angleStep = angle / arcResolution;

        // Génère anneaux (bas et haut)
        for (int y = 0; y <= 1; y++)
        {
            float yPos = y * height;
            for (int i = 0; i <= arcResolution; i++)
            {
                float a = Mathf.Deg2Rad * (i * angleStep);
                vertices.Add(new Vector3(Mathf.Cos(a) * innerRadius, yPos, Mathf.Sin(a) * innerRadius));
                vertices.Add(new Vector3(Mathf.Cos(a) * outerRadius, yPos, Mathf.Sin(a) * outerRadius));
            }
        }

        int stride = (arcResolution + 1) * 2;

        // Parois intérieures et extérieures
        for (int i = 0; i < arcResolution; i++)
        {
            int i0 = i * 2;
            int i1 = i0 + 1;
            int i2 = i0 + 2;
            int i3 = i0 + 3;

            int i0Top = i0 + stride;
            int i1Top = i1 + stride;
            int i2Top = i2 + stride;
            int i3Top = i3 + stride;

            // Extérieur (inversé)
            triangles.AddRange(new int[] { i1Top, i3, i1, i1Top, i3Top, i3 });

            // Intérieur (inversé)
            triangles.AddRange(new int[] { i0Top, i0, i2, i2Top, i0Top, i2 });
        }

        // Dessous
        for (int i = 0; i < arcResolution; i++)
        {
            int i0 = i * 2;
            int i1 = i0 + 1;
            int i2 = i0 + 2;
            int i3 = i0 + 3;

            triangles.AddRange(new int[] { i0, i1, i2, i2, i1, i3 });
        }

        // Dessus
        for (int i = 0; i < arcResolution; i++)
        {
            int i0 = stride + i * 2;
            int i1 = i0 + 1;
            int i2 = i0 + 2;
            int i3 = i0 + 3;

            triangles.AddRange(new int[] { i2, i1, i0, i2, i3, i1 });
        }

        // Côté gauche (start angle)
        int li0 = 0;
        int li1 = 1;
        int li0Top = li0 + stride;
        int li1Top = li1 + stride;

        triangles.AddRange(new int[] { li0, li0Top, li1, li1, li0Top, li1Top });

        // Côté droit (end angle)
        int ri0 = arcResolution * 2;
        int ri1 = ri0 + 1;
        int ri0Top = ri0 + stride;
        int ri1Top = ri1 + stride;

        triangles.AddRange(new int[] { ri0, ri1, ri0Top, ri1, ri1Top, ri0Top });

        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.RecalculateNormals();
        return mesh;
    }
}
