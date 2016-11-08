using UnityEngine;
using System.Collections.Generic;

public class UIController : MonoBehaviour
{
    public GameObject RiderInfoPrefab = null;
    public GameObject EnemyRiderInfoPrefab = null;

    public Sprite[] RiderImages;

    public float StartY = 50.0f;
    public float StartX = 50.0f;
    public float EnemyStartX = 50.0f;
    public float SpacingY = 80.0f;     

    private float m_height = 0.0f;
    private float m_enemyHeight = 0.0f;

    private List<RiderInfo> m_playerRiderInfo = new List<RiderInfo>();

    public void RegisterPlayerRider(Rider rider, bool enemy)
    {
        GameObject newRiderInfo = Instantiate(enemy ? EnemyRiderInfoPrefab.gameObject : RiderInfoPrefab.gameObject);

        newRiderInfo.transform.SetParent(transform);
        newRiderInfo.GetComponent<RectTransform>().anchoredPosition = new Vector3(enemy ? Screen.width - EnemyStartX : StartX, (enemy ? m_enemyHeight : m_height) + StartY, 0.0f);

        if (enemy)
        {
            m_enemyHeight -= SpacingY;
        }
        else
        {
            m_height -= SpacingY;
        }

        RiderInfo newInfo = newRiderInfo.GetComponent<RiderInfo>();
        newInfo.SetOwnerRider(rider);
        newInfo.SetPortraitSprite(RiderImages[m_playerRiderInfo.Count]);
        m_playerRiderInfo.Add(newInfo);
    }
}
