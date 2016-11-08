using UnityEngine;
using System.Collections.Generic;

public class TrafficController : MonoBehaviour
{
    public Car TrafficPrefab = null;
    public int MaxCars = 50;
    public float SpawnRange = 2.0f;

    private List<GameObject> m_focalObjects = new List<GameObject>();

    private List<GameObject> m_freeCars = new List<GameObject>();
    private List<GameObject> m_usedCars = new List<GameObject>();
    private List<GameObject> m_carsToFree = new List<GameObject>();

    private BikeGangs m_game = null;

    public void Begin()
    {
        m_game = FindObjectOfType<BikeGangs>();

        for(int i = 0; i < MaxCars; ++i)
        {
            GameObject newCar = Instantiate(TrafficPrefab.gameObject);
            newCar.GetComponent<Car>().Begin();
            m_freeCars.Add(newCar);
            newCar.name = "car " + i.ToString();
        }
    }

    public void RegisterFocalObject(GameObject focalObject)
    {
        if(!m_focalObjects.Contains(focalObject)) m_focalObjects.Add(focalObject);
    }

    public void UnregisterFocalObject(GameObject focalObject)
    {
        m_focalObjects.Remove(focalObject);
    }

    void Update()
    {
        m_carsToFree.Clear();

        // Despawn dead vehicles
        foreach (GameObject car in m_usedCars)
        {
            bool inRange = false;
            foreach(GameObject focalObject in m_focalObjects)
            {
                if(focalObject.transform.position.z - car.transform.position.z < SpawnRange)
                {
                    inRange = true;
                    break;
                }
            }

            if(!inRange)
            {
                m_carsToFree.Add(car);
            }
        }

        foreach (var carToFree in m_carsToFree)
        {
            FreeCar(carToFree);
        }

        // Spawn new vehicles
        foreach (var focalObject in m_focalObjects)
        {
            float yPos = focalObject.transform.position.z;
            //Debug.DrawLine(Vector3.zero, new Vector3(focalObject.transform.position.x, 0.01f, focalObject.transform.position.z));
            //Debug.DrawLine(Vector3.zero, new Vector3(focalObject.transform.position.x, 0.01f, focalObject.transform.position.z + SpawnRange), Color.red);

            bool overlappingRanges = false;
            foreach (var otherObject in m_focalObjects)
            {
                if (focalObject != otherObject)
                {
                    float otherYPos = otherObject.transform.position.z;

                    if (yPos + SpawnRange > otherYPos - SpawnRange && yPos + SpawnRange < otherYPos + SpawnRange)
                    {
                        overlappingRanges = true;
                        break;
                    }
                }
            }

            if(!overlappingRanges)
            {
                int laneIndex = Random.Range(0, m_game.RoadGenerator.NumLanes);

                float passValue = m_game.RoadGenerator.IsLaneOncoming(laneIndex) ? 0.8f : 0.9f;
                if (Random.value > passValue && m_freeCars.Count > 0)
                {
                    GameObject newCar = m_freeCars[0];

                    float focalProgress = focalObject.GetComponent<Rider>().Progress ;
                    float progress = (focalProgress + SpawnRange);

                    Vector3 newDirection;
                    Vector3 newPosition = newCar.GetComponent<Car>().GetPositionAtProgress(progress, out newDirection);
                    Vector3 size = newCar.GetComponent<BoxCollider>().size;
                    size.Scale(newCar.transform.lossyScale);
                    size *= 2.5f;
                    Collider[] hits = Physics.OverlapBox(newPosition, size);

                    if(hits.Length == 0)
                    {
                        m_freeCars.Remove(newCar);
                        m_usedCars.Add(newCar);

                        newCar.GetComponent<Car>().SetUpdatesEnabled(true);
                        newCar.GetComponent<Car>().Lane = laneIndex;

                        newCar.GetComponent<Car>().SetProgress(progress);
                    }

                    
                }
            }
        }
    }

    void FreeCar(GameObject car)
    {
        car.GetComponent<Car>().SetUpdatesEnabled(false);
        m_usedCars.Remove(car);
        m_freeCars.Add(car);
    }
}
