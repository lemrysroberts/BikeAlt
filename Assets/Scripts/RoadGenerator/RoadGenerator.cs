using UnityEngine;
using System.Collections;

public class RoadGenerator : MonoBehaviour
{
    public Vector3[] RoadPoints {  get { return m_roadPoints; } }
    public float RoadLength { get { return m_roadLength; } }

    public Material RoadMaterial = null;

    public int ControlPoints = 30;
    public int IterationsPerControlPoint = 20;
    public float RoadWidth = 2.0f;
    public float UV0Width = 2.0f;
    public float ControlPointSpacing = 2.0f;
    public int NumLanes = 4;

    [Header("Debug")]
    public bool ForceStraightRoad = false;

    private Vector3[] m_roadPoints = null;
    private float m_laneWidth = 0.0f;
    private float m_roadLength = 0.0f;

    public void Regenerate()
    {
        m_laneWidth = RoadWidth / (float)NumLanes * 2.0f;

        GameObject road = GameObjectHelper.FindChild(gameObject, "Road", true);
        if (road != null) DestroyImmediate(road);

        m_roadPoints = new Vector3[ControlPoints];
        
        float x = 0.0f;
        for (int i = 0; i < ControlPoints; ++i)
        {
            if(!ForceStraightRoad)
            {
                x += Random.Range(-1.0f, 1.0f);
            }
            
            m_roadPoints[i] = new Vector3(x, 0.0f, ControlPointSpacing * i);
        }

        m_roadLength = iTween.PathLength(m_roadPoints);

        SplineMeshSettings settings = new SplineMeshSettings();
        settings.UV0Width = UV0Width;
        settings.RoadWidth = RoadWidth;

        Mesh newMesh = SplineMeshBuilder.BuildMesh(m_roadPoints, ControlPoints * IterationsPerControlPoint, settings);

        GameObject rootObject = new GameObject("Road");
        rootObject.transform.parent = transform;
        MeshRenderer renderer = rootObject.AddComponent<MeshRenderer>();
        MeshFilter filter = rootObject.AddComponent<MeshFilter>();

        renderer.material = RoadMaterial;
        filter.mesh = newMesh;
    }

    public Vector3 GetLaneCenterAtPoint(float progress, int laneIndex, out Vector3 direction)
    {
        progress = Mathf.Clamp(progress, 0.0f, m_roadLength);

        Vector3 roadPosition = iTween.PointOnPath(m_roadPoints, progress / m_roadLength);
        Vector3 normal = Vector3.left;

        direction = Vector3.forward;

        const float comparisonOffset = 0.001f;
        if (progress > comparisonOffset)
        {
            Vector3 roadPositionPrevious = iTween.PointOnPath(m_roadPoints, (progress - comparisonOffset) / m_roadLength);
            direction = roadPosition - roadPositionPrevious;

            normal = Vector3.Cross(direction, Vector3.up);
        }

        normal.Normalize();
        Vector3 start = roadPosition - (normal * RoadWidth) + (normal * ((laneIndex * m_laneWidth) + (m_laneWidth * 0.5f)));

        return start;
    }

    public Vector3 GetOffsetAtPoint(float progress, float lateralOffset, out Vector3 direction)
    {
        progress = Mathf.Clamp(progress, 0.0f, m_roadLength);

        Vector3 roadPosition = iTween.PointOnPath(m_roadPoints, progress / m_roadLength);
        Vector3 normal = Vector3.right;

        direction = Vector3.forward;

        const float comparisonOffset = 0.001f;
        if (progress > comparisonOffset)
        {
            Vector3 roadPositionPrevious = iTween.PointOnPath(m_roadPoints, (progress - comparisonOffset) / m_roadLength);
            direction = roadPositionPrevious - roadPosition;

            normal = Vector3.Cross(direction, Vector3.up);
        }

        normal.Normalize();
        Vector3 start = roadPosition + (normal * lateralOffset);

        return start;
    }

    public bool IsLaneOncoming(int laneIndex)
    {
        return (laneIndex + 1) > (NumLanes) / 2;
    }
}
