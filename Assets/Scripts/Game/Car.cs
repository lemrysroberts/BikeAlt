using UnityEngine;
using System.Collections;

public class Car : MonoBehaviour
{
    public int Lane = 0;
    public float Speed = 0.002f;

    private float progress = 0.0f;
    private BikeGangs m_game = null;
    private bool m_canUpdate = false;
    private MeshRenderer m_renderer = null;

    public void Begin()
    {
        m_game = FindObjectOfType<BikeGangs>();
        m_renderer = GetComponent<MeshRenderer>();
        Color materialColor = m_renderer.material.color;
   //     materialColor.b = materialColor.b + Random.Range(-0.15f, 0.15f);
        m_renderer.material.color = materialColor;
    }

    public void SetProgress(float progress)
    {
        this.progress = progress;
    }

    public void SetUpdatesEnabled(bool enabled)
    {
        m_canUpdate = enabled;
        m_renderer.enabled = enabled;
    }

	void Update ()
    {
        if (!m_canUpdate) return;

        Vector3 direction;

        progress += (Speed * GameTime.deltaTime) * (m_game.RoadGenerator.IsLaneOncoming(Lane) ? -1.0f : 1.0f);
        transform.position = GetPositionAtProgress(progress, out direction);

        Quaternion rotation = Quaternion.Euler(new Vector3(0.0f, Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg, 0.0f));

        transform.rotation = rotation;
	}

    public Vector3 GetPositionAtProgress(float progress, out Vector3 direction)
    {
        return m_game.RoadGenerator.GetLaneCenterAtPoint(progress, Lane, out direction) + new Vector3(0.0f, 0.001f, 0.0f);
    }
}
