using UnityEngine;
using System.Collections;

public class SplineFollowControl : MonoBehaviour
{
    public float LerpSpeed = 0.1f;
    public float MoveSpeed = 0.1f;
    public float Height = 1.0f;
    public float OffsetZ = -1.0f;
    public float ZoomSpeed = 0.1f;
    public float ZoomMax = 1.5f;
    public float ZoomMin = 0.5f;

    private float m_progress = 0.0f;
    private bool m_readyForUpdate = false;
    private BikeGangs m_game = null;
    private float m_targetProgress;

    private float m_zoomTarget = 1.0f;
    private float m_zoom = 1.0f;

    public void ControlBegin()
    {
        m_game = FindObjectOfType<BikeGangs>();
        m_readyForUpdate = true;

        transform.position = iTween.PointOnPath(m_game.RoadGenerator.RoadPoints, m_progress) + new Vector3(0.0f, Height, OffsetZ);
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (!m_readyForUpdate) return;

        if (Input.GetKey(KeyCode.W)) { m_targetProgress += MoveSpeed * Time.deltaTime; }
        if (Input.GetKey(KeyCode.S)) { m_targetProgress -= MoveSpeed * Time.deltaTime; }

        if (Input.GetKeyDown(KeyCode.Equals)) { GameTime.Instance.m_timeMultiplier = Mathf.Clamp01(GameTime.Instance.m_timeMultiplier + 0.05f); Debug.Log("Time up"); }
        if (Input.GetKeyDown(KeyCode.Minus)) { GameTime.Instance.m_timeMultiplier = Mathf.Clamp01(GameTime.Instance.m_timeMultiplier - 0.05f); Debug.Log("Time down"); }

        m_zoomTarget -= (float)Input.mouseScrollDelta.y * ZoomSpeed;
        m_zoomTarget = Mathf.Clamp(m_zoomTarget, ZoomMin, ZoomMax);

        m_targetProgress = Mathf.Clamp01(m_targetProgress);

        m_progress = Mathf.Lerp(m_progress, m_targetProgress, LerpSpeed * Time.deltaTime);
        m_zoom = Mathf.Lerp(m_zoom, m_zoomTarget, ZoomSpeed * Time.deltaTime);

        transform.position = iTween.PointOnPath(m_game.RoadGenerator.RoadPoints, m_progress) + new Vector3(0.0f, Height * m_zoom, OffsetZ * m_zoom);
    }
}
