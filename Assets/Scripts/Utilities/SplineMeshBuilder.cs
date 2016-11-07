using UnityEngine;
using System.Collections;

public class SplineMeshSettings
{
    public float UV0Width = 1.0f;
    public float RoadWidth = 1.0f;
}

public class SplineMeshBuilder
{
	public static Mesh BuildMesh(Vector3[] points, int iterations, SplineMeshSettings settings)
    {
        int sectionIndices = 6;

        Mesh nodeMesh = new Mesh();
        Vector3[] vertices = new Vector3[(iterations * 2) * 2];
        Vector3[] normals = new Vector3[(iterations * 2) * 2]; 
        Vector2[] uvs0 = new Vector2[(iterations * 2) * 2];
        Vector2[] uvs1 = new Vector2[(iterations * 2) * 2];
        Color[] colors = new Color[(iterations * 2) * 2];
        int[] triangles = new int[(iterations - 1) * sectionIndices + sectionIndices];

        float stepSize = 1.0f / (float)(iterations - 1);

        float UV0Progress = 0.0f;
        float UV1Progress = 0.0f;

        float distanceToLastBezier = 0.0f;
        float progress = 0.0f;

        float lastUV0Distance = 0.0f;
        float lastUV1Distance = 0.0f;

        Vector3 lastPoint = Vector3.zero;

        for (int i = 0; i < iterations; i++)
        {
            Vector3 position = iTween.PointOnPath(points, stepSize * (float)i);

            Vector3 tangent;

            if (i > 0)
            {
                tangent = Vector3.Cross(position - lastPoint, Vector3.up);
            }
            else
            {
                Vector3 nextPosition = iTween.PointOnPath(points, stepSize * (float)(i + 1));
                tangent = Vector3.Cross(nextPosition - position, Vector3.up);
            }

            tangent.Normalize();
            
            Vector3 bezierPoint = position;

            vertices[i * 4] = new Vector3(bezierPoint.x, 0.0f, bezierPoint.z);
            vertices[i * 4] += new Vector3(tangent.x, 0.0f, tangent.z) * settings.RoadWidth;

            vertices[i * 4 + 1] = new Vector3(bezierPoint.x, 0.0f, bezierPoint.z);
            vertices[i * 4 + 1] += new Vector3(-tangent.x, 0.0f, -tangent.z) * settings.RoadWidth;

            vertices[i * 4 + 2] = new Vector3(bezierPoint.x, 0.0f, bezierPoint.z);
            vertices[i * 4 + 2] += new Vector3(tangent.x, 0.0f, tangent.z) * settings.RoadWidth;

            vertices[i * 4 + 3] = new Vector3(bezierPoint.x, 0.0f, bezierPoint.z);
            vertices[i * 4 + 3] += new Vector3(-tangent.x, 0.0f, -tangent.z) * settings.RoadWidth;

            if (i > 0) { distanceToLastBezier = (bezierPoint - lastPoint).magnitude; } // Only start tracking distances between beziers when there are two to compare

            lastPoint = bezierPoint;

            progress += stepSize;

            if (i > 0)
            {
                float distanceDelta = distanceToLastBezier;

                float uv0DistanceDelta = distanceDelta * 1.0f;
                float uv1DistanceDelta = distanceDelta * 1.0f;

                UV0Progress += uv0DistanceDelta;
                UV1Progress += uv1DistanceDelta;
            }

            lastUV0Distance = UV0Progress;
            lastUV1Distance = UV1Progress;

            colors[i * 4] = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            colors[i * 4 + 1] = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            colors[i * 4 + 2] = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            colors[i * 4 + 3] = new Color(1.0f, 1.0f, 1.0f, 1.0f);

            uvs0[i * 4] = new Vector2(UV0Progress, 0.0f);
            uvs0[i * 4 + 1] = new Vector2(UV0Progress, 1.0f * settings.UV0Width);
            uvs0[i * 4 + 2] = new Vector2(UV0Progress, 0.0f);
            uvs0[i * 4 + 3] = new Vector2(UV0Progress, 1.0f * settings.UV0Width) ;

            uvs1[i * 4] = new Vector2(UV1Progress, 0.0f);
            uvs1[i * 4 + 1] = new Vector2(UV1Progress, 1.0f);
            uvs1[i * 4 + 2] = new Vector2(UV1Progress, 0.0f);
            uvs1[i * 4 + 3] = new Vector2(UV1Progress, 1.0f);


            normals[i * 4] = new Vector3(0.0f, 1.0f, 0.0f);
            normals[i * 4 + 1] = new Vector3(0.0f, 1.0f, 0.0f);
            normals[i * 4 + 2] = new Vector3(0.0f, 1.0f, 0.0f);
            normals[i * 4 + 3] = new Vector3(0.0f, 1.0f, 0.0f);
        }

        for (int i = 0; i < iterations - 1; i++)
        {
            triangles[i * sectionIndices] = i * 4;
            triangles[i * sectionIndices + 1] = i * 4 + 4;
            triangles[i * sectionIndices + 2] = i * 4 + 1;

            triangles[i * sectionIndices + 3] = i * 4 + 4;
            triangles[i * sectionIndices + 4] = i * 4 + 5;
            triangles[i * sectionIndices + 5] = i * 4 + 1;
        }

        nodeMesh.vertices = vertices;
        nodeMesh.uv = uvs0;
        nodeMesh.uv2 = uvs1;
        nodeMesh.colors = colors;
        nodeMesh.triangles = triangles;
        nodeMesh.normals = normals;

        nodeMesh.RecalculateBounds();

        return nodeMesh;
    }
}
