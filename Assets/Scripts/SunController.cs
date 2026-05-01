using UnityEngine;

public class SunController : MonoBehaviour
{    
    private Light sunLight;
    private TimeManager timeManager;

    private void Awake()
    {
        sunLight = GetComponent<Light>();
    }

    void Start()
    {
        timeManager = TimeManager.Instance;
    }

    private void Update()
    {
        if (timeManager == null) return;

        UpdateSunRotation();
        UpdateSunIntensity();
        DynamicGI.UpdateEnvironment();
    }

    private void UpdateSunRotation()
    {
        // 0-24 saatlik zamanı 0-360 derecelik rotasyona çeviriyoruz.
        // Formül: (Zaman / 24) * 360 - 90 (90 derece ofset öğle saatini tepeye alır)
        float rotationAngle = (timeManager.CurrentTimeOfDay / 24f) * 360f - 90f;
        
        transform.rotation = Quaternion.Euler(rotationAngle, -30f, 0f);
    }

    private void UpdateSunIntensity()
    {
        if (sunLight == null) return;

        sunLight.intensity = timeManager.SunIntensity;
        
        if (timeManager.CurrentTimeOfDay > 18.5f || timeManager.CurrentTimeOfDay < 5.5f)
        {
            sunLight.color = new Color(0.1f, 0.15f, 0.35f); // Gece mavisi
        }
        else
        {
            sunLight.color = Color.white; // Gündüz beyaz ışık
        }
    }
}
