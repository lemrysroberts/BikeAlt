using UnityEngine;
using System.Collections;

public class Rider : MonoBehaviour
{
    public float Progress { get { return m_progress; } }

    [Header("Bike Attributes")]
    public float MinMoveSpeed = 0.01f;
    public float MaxMoveSpeed = 0.02f;

    [Tooltip("Determines how fast the rider can move laterally")]
    public float TurnRate = 1.0f;

    [Tooltip("Determines how fast the rider corrects to their latest target")]
    public float CompensationSpeed = 1.0f;

    [Header("Avoidance")]
    public float TestAheadDistance = 0.1f;
    
    private BikeGangs m_game = null;

    private float m_progress = 0.0f;
    private bool m_started = false;

    private float m_targetOffset = 0.0f;
    private float m_offset = 0.0f;
    private float m_moveSpeed = 0.0f;
    private Vector2 m_velocity = Vector2.up;

    private Heatmap m_heatmap = null;
    private Steering m_steering = null;
    private float m_accumulator = 0.0f;
    private float m_accumulatorMultiplier = 1.0f;

    public void RiderStart()
    {
        m_heatmap = GetComponent<Heatmap>();
        m_steering = GetComponent<Steering>();

        m_game = FindObjectOfType<BikeGangs>();
        m_started = true;
        m_moveSpeed = Random.Range(MinMoveSpeed, MaxMoveSpeed);
        m_offset =  Random.Range(-m_game.RoadGenerator.RoadWidth, m_game.RoadGenerator.RoadWidth);
        m_targetOffset = m_game.RoadGenerator.RoadWidth / 12.0f * 3.0f;
        m_heatmap.Begin();
        m_accumulatorMultiplier = Random.Range(0.0f, 10.0f);
        m_accumulatorMultiplier += Random.Range(0.0f, 0.5f);
    }
	
	void Update ()
    {
        float offsetX = (m_targetOffset - m_offset) / (m_game.RoadGenerator.RoadWidth * 2.0f);
        float offsetY = Mathf.Clamp01(1.0f - offsetX);
        float targetAngle = Mathf.Atan2(offsetX, offsetY) * Mathf.Rad2Deg;
        
        Debug.DrawLine(transform.position, transform.position + new Vector3(offsetX, 0.0f, 0.0f), Color.magenta);


        //m_heatmap.UpdateHeatmap();
        float steeringDirection = m_steering.UpdateSteering(targetAngle);
       // Debug.Log("Steering direction: " + steeringDirection);

        m_progress += m_moveSpeed * GameTime.deltaTime;

        Vector3 direction;

        Vector3 lastPosition = transform.position;
        transform.position = m_game.RoadGenerator.GetOffsetAtPoint(m_progress, m_offset, out direction) + new Vector3(0.0f, 0.02f, 0.0f);

        Quaternion rotation = Quaternion.Euler(new Vector3(0.0f, Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg, 0.0f));

        Debug.DrawLine(transform.position, transform.position + new Vector3(m_targetOffset, 0.0f, 0.0f), Color.cyan);

        //if(Mathf.Abs(m_targetOffset - m_offset) < 0.002f)
        {
            // Pick a new target
           m_targetOffset = Mathf.Lerp(-m_game.RoadGenerator.RoadWidth * 0.7f, m_game.RoadGenerator.RoadWidth * 0.7f, (Mathf.Sin(m_accumulator) + 1.0f) / 2.0f);
            Debug.Log("NEW TARGET");
        }

        // Steer towards the target
        Vector2 targetVec = new Vector2(steeringDirection, 0.0f);
        m_velocity = Vector2.Lerp(m_velocity, targetVec, Mathf.Clamp01(GameTime.deltaTime * CompensationSpeed));

        m_offset += m_velocity.x * Mathf.Clamp01(GameTime.deltaTime * TurnRate);
        m_accumulator += GameTime.deltaTime * m_accumulatorMultiplier * 0.02f;                
    }
}
