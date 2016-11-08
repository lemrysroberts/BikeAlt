using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RiderInfo : MonoBehaviour
{
    public Image RiderPortraitElement;
    public Image HealthImage;

    private Rider m_rider = null;

    public void SetOwnerRider(Rider rider)
    {
        m_rider = rider;
    }

    public void SetPortraitSprite(Sprite sprite)
    {
        RiderPortraitElement.sprite = sprite;
    }

    public void Update()
    {
        HealthImage.fillAmount = m_rider.Health;

        HealthImage.color = HSBColor.Lerp(new HSBColor(Color.red), new HSBColor(Color.green), m_rider.Health).ToColor();
    }	
}
