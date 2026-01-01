using UnityEngine;
using System.Collections.Generic;

public class FarmEnvironment : MonoBehaviour
{
    [Header("Environment Settings")]
    public float dayNightCycleSpeed = 0.5f;
    public Light sunLight;
    public Gradient dayNightColors;
    
    [Header("Weather System")]
    public bool enableWeather = true;
    public float weatherChangeInterval = 300f; // 5 minutes
    private float lastWeatherChange;
    
    [Header("Environment Objects")]
    public List<GameObject> farmObjects = new List<GameObject>();
    public List<Transform> spawnPoints = new List<Transform>();
    
    [Header("Time of Day")]
    [Range(0, 24)]
    public float currentTime = 12f; // 12 = noon
    
    private float dayLengthInSeconds = 120f; // 2 minutes for a full day/night cycle
    private float timeScale = 1f;
    
    void Start()
    {
        InitializeEnvironment();
    }
    
    void Update()
    {
        UpdateDayNightCycle();
        UpdateWeather();
    }
    
    private void InitializeEnvironment()
    {
        // Find the sun light if not assigned
        if (sunLight == null)
        {
            Light[] lights = FindObjectsOfType<Light>();
            foreach (Light light in lights)
            {
                if (light.type == LightType.Directional)
                {
                    sunLight = light;
                    break;
                }
            }
        }
        
        // Register all farm objects in the scene
        RegisterFarmObjects();
        
        lastWeatherChange = Time.time;
    }
    
    private void RegisterFarmObjects()
    {
        // Find all objects that might be farm-related
        GameObject[] allObjects = GameObject.FindGameObjectsWithTag("FarmObject");
        foreach (GameObject obj in allObjects)
        {
            if (!farmObjects.Contains(obj))
            {
                farmObjects.Add(obj);
            }
        }
        
        // Also look for objects with specific components
        InteractiveObject[] interactiveObjects = FindObjectsOfType<InteractiveObject>();
        foreach (InteractiveObject obj in interactiveObjects)
        {
            if (!farmObjects.Contains(obj.gameObject))
            {
                farmObjects.Add(obj.gameObject);
            }
        }
    }
    
    private void UpdateDayNightCycle()
    {
        // Update time based on real time scaled by timeScale
        currentTime += (Time.deltaTime / dayLengthInSeconds) * 24f * timeScale;
        
        if (currentTime >= 24f)
        {
            currentTime -= 24f; // Wrap around to next day
        }
        
        // Calculate sun rotation based on time of day
        if (sunLight != null)
        {
            float sunAngle = (currentTime / 24f) * 360f - 90f; // -90 to start at sunrise
            sunLight.transform.rotation = Quaternion.Euler(sunAngle * 0.5f, sunAngle, 0);
            
            // Update light color based on time of day
            float colorTime = Mathf.Repeat(currentTime + 6f, 24f) / 24f; // Shift to make noon at 0.5
            sunLight.color = dayNightColors.Evaluate(colorTime);
            
            // Adjust intensity based on time of day
            float intensity = Mathf.Clamp01(1f - Mathf.Abs(colorTime - 0.5f) * 2f);
            sunLight.intensity = Mathf.Lerp(0.2f, 1f, intensity);
        }
    }
    
    private void UpdateWeather()
    {
        if (!enableWeather) return;
        
        if (Time.time - lastWeatherChange > weatherChangeInterval)
        {
            ChangeWeather();
            lastWeatherChange = Time.time;
        }
    }
    
    private void ChangeWeather()
    {
        // In a real implementation, this would change weather conditions
        // For now, we'll just log the change
        Debug.Log("Weather changed at time: " + currentTime.ToString("F2"));
    }
    
    public void SetTimeScale(float scale)
    {
        timeScale = scale;
    }
    
    public void SetTime(float hour)
    {
        currentTime = Mathf.Clamp(hour, 0f, 24f);
    }
    
    public string GetTimeOfDay()
    {
        if (currentTime >= 6 && currentTime < 12)
            return "Morning";
        else if (currentTime >= 12 && currentTime < 18)
            return "Afternoon";
        else if (currentTime >= 18 && currentTime < 22)
            return "Evening";
        else
            return "Night";
    }
    
    public void AddFarmObject(GameObject obj)
    {
        if (!farmObjects.Contains(obj))
        {
            farmObjects.Add(obj);
        }
    }
    
    public void RemoveFarmObject(GameObject obj)
    {
        farmObjects.Remove(obj);
    }
}