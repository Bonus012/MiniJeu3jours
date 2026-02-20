using System.Collections.Generic;
using UnityEngine;

public class PartialHollowCylinderShatterer : MonoBehaviour
{
    public int segments = 4;
    public float explosionForce = 100f;
    public float explosionRadius = 5f;
    public float pieceLifetime = 3f;

    public void Shatter()
    {
        PartialHollowCylinderMesh original = GetComponent<PartialHollowCylinderMesh>();
        if (original == null) return;

        float angleStep = original.OpeningAngle / segments;
        float height = original.height;
        float innerRadius = original.innerRadius;
        float outerRadius = original.outerRadius;

        for (int i = 0; i < segments; i++)
        {
            float startAngle = i * angleStep;
            float endAngle = (i + 1) * angleStep;

            GameObject segment = CreateSegment(startAngle, endAngle, innerRadius, outerRadius, height);
            segment.transform.position = transform.position;
            segment.transform.rotation = transform.rotation;

            Rigidbody rb = segment.AddComponent<Rigidbody>();
            MeshCollider mc = segment.AddComponent<MeshCollider>();
            mc.convex = true;

            // Direction vers l’extérieur (convertie en espace monde)
            float midAngle = Mathf.Deg2Rad * ((startAngle + endAngle) / 2f);
            Vector3 localDirection = new Vector3(Mathf.Cos(midAngle), 0f, Mathf.Sin(midAngle)).normalized;
            Vector3 worldDirection = transform.TransformDirection(localDirection);

            rb.AddForce(worldDirection * explosionForce);

            Destroy(segment, pieceLifetime);
        }

        Destroy(gameObject);
    }

    GameObject CreateSegment(float startAngle, float endAngle, float innerRadius, float outerRadius, float height)
    {
        GameObject go = new GameObject("CylinderPiece");
        MeshFilter mf = go.AddComponent<MeshFilter>();
        MeshRenderer mr = go.AddComponent<MeshRenderer>();
        mr.sharedMaterial = GetComponent<MeshRenderer>().sharedMaterial;

        Mesh mesh = GenerateClosedWedgeMesh(startAngle, endAngle, innerRadius, outerRadius, height);
        mf.mesh = mesh;

        return go;
    }

    Mesh GenerateClosedWedgeMesh(float startDeg, float endDeg, float innerR, float outerR, float height)
    {
        Mesh mesh = new Mesh();
        mesh.name = "Wedge";

        int arcResolution = 6;
        float angleStep = (endDeg - startDeg) / arcResolution;

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        // Anneaux bas et haut
        for (int yLevel = 0; yLevel <= 1; yLevel++)
        {
            float y = yLevel * height;
            for (int i = 0; i <= arcResolution; i++)
            {
                float angle = Mathf.Deg2Rad * (startDeg + i * angleStep);
                float cos = Mathf.Cos(angle);
                float sin = Mathf.Sin(angle);

                vertices.Add(new Vector3(cos * innerR, y, sin * innerR));
                vertices.Add(new Vector3(cos * outerR, y, sin * outerR));
            }
        }

        int stride = (arcResolution + 1) * 2;

        // Murs intérieurs et extérieurs
        for (int i = 0; i < arcResolution; i++)
        {
            int i0 = i * 2;
            int i1 = i0 + 1;
            int i2 = i0 + 2;
            int i3 = i0 + 3;

            int i0top = i0 + stride;
            int i1top = i1 + stride;
            int i2top = i2 + stride;
            int i3top = i3 + stride;

            // Mur extérieur
            triangles.AddRange(new int[] {
                i3, i1, i1top,
                i3top, i3, i1top
            });

            // Mur intérieur
            triangles.AddRange(new int[] {
                i0, i2, i2top,
                i0, i2top, i0top
            });
        }

        // Dessous
        for (int i = 0; i < arcResolution; i++)
        {
            int baseBot = i * 2;
            triangles.AddRange(new int[] {
                baseBot, baseBot + 1, baseBot + 2,
                baseBot + 2, baseBot + 1, baseBot + 3
            });
        }

        // Dessus
        for (int i = 0; i < arcResolution; i++)
        {
            int baseTop = stride + i * 2;
            triangles.AddRange(new int[] {
                baseTop + 2, baseTop + 1, baseTop,
                baseTop + 3, baseTop + 1, baseTop + 2
            });
        }

        // Cap gauche
        int iLeftInnerBottom = 0;
        int iLeftOuterBottom = 1;
        int iLeftInnerTop = iLeftInnerBottom + stride;
        int iLeftOuterTop = iLeftOuterBottom + stride;

        triangles.AddRange(new int[] {
            iLeftInnerBottom, iLeftInnerTop, iLeftOuterBottom,
            iLeftInnerTop, iLeftOuterTop, iLeftOuterBottom
        });

        // Cap droit
        int iRightInnerBottom = arcResolution * 2;
        int iRightOuterBottom = iRightInnerBottom + 1;
        int iRightInnerTop = iRightInnerBottom + stride;
        int iRightOuterTop = iRightOuterBottom + stride;

        triangles.AddRange(new int[] {
            iRightInnerBottom, iRightOuterBottom, iRightOuterTop,
            iRightInnerBottom, iRightOuterTop, iRightInnerTop
        });

        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.RecalculateNormals();
        return mesh;
    }
}
