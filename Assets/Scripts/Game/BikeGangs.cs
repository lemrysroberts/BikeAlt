using UnityEngine;
using System.Collections.Generic;

public class BikeGangs : MonoBehaviour
{
    public float PlaybackSpeed = 1.0f;
    public int NumRiders = 1;
    public int NumEnemyRiders = 1;
    public Rider RiderPrefab = null;
    public Rider EnemyRiderPrefab = null;
    public Car CarPrefab = null;

    public bool DebugCollisions = false;
    public float AudioVolume = 1.0f;

    public RoadGenerator RoadGenerator { get { return m_roadGenerator; } }

    private RoadGenerator m_roadGenerator = null;
    private SplineFollowControl m_controller = null;
    private TrafficController m_trafficController = null;

    private List<Rider> m_riders = new List<Rider>();
    private List<Rider> m_enemyRiders = new List<Rider>();

    private UIController m_uiController;

    void Start ()
    {

        AudioListener.volume = AudioVolume;
        GameTime.Instance.m_timeMultiplier = PlaybackSpeed;

        m_trafficController = FindObjectOfType<TrafficController>();
        m_roadGenerator = FindObjectOfType<RoadGenerator>();
        m_controller = FindObjectOfType<SplineFollowControl>();
        m_uiController = FindObjectOfType<UIController>();

        m_roadGenerator.Regenerate();

        m_trafficController.Begin();
        m_controller.ControlBegin();

        // Spawn riders
        for(int riderIndex = 0; riderIndex < NumRiders; ++riderIndex)
        {
            Rider newRider = Instantiate(RiderPrefab.gameObject).GetComponent<Rider>();
            newRider.TeamId = 0;
            newRider.RiderStart();
            m_riders.Add(newRider);
            m_trafficController.RegisterFocalObject(newRider.gameObject);

            m_uiController.RegisterPlayerRider(newRider, false);
        }

        for (int riderIndex = 0; riderIndex < NumEnemyRiders; ++riderIndex)
        {
            Rider newRider = Instantiate(EnemyRiderPrefab.gameObject).GetComponent<Rider>();
            newRider.TeamId = 1;
            newRider.RiderStart();
            m_enemyRiders.Add(newRider);
            m_trafficController.RegisterFocalObject(newRider.gameObject);

            m_uiController.RegisterPlayerRider(newRider, true);
        }
    }
	
	void Update ()
    {
        Shader.SetGlobalFloat("_BikeProgress", m_riders[0].Progress * RoadGenerator.RoadLength);

        GameTime.Instance.Update();
    }
}
