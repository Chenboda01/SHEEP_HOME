using UnityEngine;
using System.Collections.Generic;

public class VisualEffectsManager : MonoBehaviour
{
    public static VisualEffectsManager Instance { get; private set; }
    
    [Header("Weather Effects")]
    public GameObject rainEffect;
    public GameObject snowEffect;
    public GameObject fogEffect;
    
    [Header("Environmental Effects")]
    public GameObject dayNightCycleEffect;
    public GameObject particleSystemPrefab;
    
    [Header("Interaction Effects")]
    public GameObject harvestEffect;
    public GameObject waterEffect;
    public GameObject plantingEffect;
    
    [Header("Post-Processing")]
    public bool enablePostProcessing = true;
    public float saturationLevel = 1.0f;
    public float contrastLevel = 1.0f;
    public float bloomIntensity = 0.5f;
    
    [Header("Quality Settings")]
    public int textureQuality = 2; // 0=Low, 1=Medium, 2=High, 3=Ultra
    public int shadowQuality = 2;
    public int antiAliasingLevel = 2; // 0=Off, 2=2x, 4=4x, 8=8x
    
    private Dictionary<string, GameObject> activeEffects = new Dictionary<string, GameObject>();
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        InitializeVisualEffects();
    }
    
    private void InitializeVisualEffects()
    {
        // Apply quality settings
        ApplyQualitySettings();
        
        // Initialize weather effects
        InitializeWeatherEffects();
        
        // Initialize environmental effects
        InitializeEnvironmentalEffects();
    }
    
    private void ApplyQualitySettings()
    {
        // Set texture quality
        QualitySettings.masterTextureLimit = 3 - textureQuality; // Invert so 0=lowest, 3=highest
        
        // Set shadow quality
        QualitySettings.shadows = shadowQuality > 0 ? ShadowQuality.All : ShadowQuality.Disable;
        QualitySettings.shadowResolution = (ShadowResolution)shadowQuality;
        
        // Set anti-aliasing
        switch(antiAliasingLevel)
        {
            case 0:
                QualitySettings.antiAliasing = 0;
                break;
            case 2:
                QualitySettings.antiAliasing = 2;
                break;
            case 4:
                QualitySettings.antiAliasing = 4;
                break;
            case 8:
                QualitySettings.antiAliasing = 8;
                break;
        }
        
        // Set other quality settings for better visuals
        QualitySettings.vSyncCount = 1; // Reduce tearing
        QualitySettings.maxQueuedFrames = 1; // Reduce input lag
    }
    
    private void InitializeWeatherEffects()
    {
        // Weather effects will be activated based on in-game conditions
        if (rainEffect != null) rainEffect.SetActive(false);
        if (snowEffect != null) snowEffect.SetActive(false);
        if (fogEffect != null) fogEffect.SetActive(false);
    }
    
    private void InitializeEnvironmentalEffects()
    {
        // Day/night cycle is handled by FarmEnvironment
        if (dayNightCycleEffect != null) dayNightCycleEffect.SetActive(false);
    }
    
    public void PlayHarvestEffect(Vector3 position)
    {
        if (harvestEffect != null)
        {
            GameObject effect = Instantiate(harvestEffect, position, Quaternion.identity);
            activeEffects["harvest_" + position.ToString()] = effect;
            
            // Auto destroy after duration
            StartCoroutine(AutoDestroyEffect(effect, 2f));
        }
    }
    
    public void PlayWaterEffect(Vector3 position)
    {
        if (waterEffect != null)
        {
            GameObject effect = Instantiate(waterEffect, position, Quaternion.identity);
            activeEffects["water_" + position.ToString()] = effect;
            
            // Auto destroy after duration
            StartCoroutine(AutoDestroyEffect(effect, 1f));
        }
    }
    
    public void PlayPlantingEffect(Vector3 position)
    {
        if (plantingEffect != null)
        {
            GameObject effect = Instantiate(plantingEffect, position, Quaternion.identity);
            activeEffects["planting_" + position.ToString()] = effect;
            
            // Auto destroy after duration
            StartCoroutine(AutoDestroyEffect(effect, 1.5f));
        }
    }
    
    public void ActivateRainEffect()
    {
        if (rainEffect != null)
        {
            rainEffect.SetActive(true);
            if (!activeEffects.ContainsKey("rain"))
            {
                activeEffects["rain"] = rainEffect;
            }
        }
    }
    
    public void DeactivateRainEffect()
    {
        if (rainEffect != null)
        {
            rainEffect.SetActive(false);
            if (activeEffects.ContainsKey("rain"))
            {
                activeEffects.Remove("rain");
            }
        }
    }
    
    public void ActivateSnowEffect()
    {
        if (snowEffect != null)
        {
            snowEffect.SetActive(true);
            if (!activeEffects.ContainsKey("snow"))
            {
                activeEffects["snow"] = snowEffect;
            }
        }
    }
    
    public void DeactivateSnowEffect()
    {
        if (snowEffect != null)
        {
            snowEffect.SetActive(false);
            if (activeEffects.ContainsKey("snow"))
            {
                activeEffects.Remove("snow");
            }
        }
    }
    
    public void ActivateFogEffect()
    {
        if (fogEffect != null)
        {
            fogEffect.SetActive(true);
            if (!activeEffects.ContainsKey("fog"))
            {
                activeEffects["fog"] = fogEffect;
            }
        }
    }
    
    public void DeactivateFogEffect()
    {
        if (fogEffect != null)
        {
            fogEffect.SetActive(false);
            if (activeEffects.ContainsKey("fog"))
            {
                activeEffects.Remove("fog");
            }
        }
    }
    
    public void SetSaturation(float saturation)
    {
        saturationLevel = Mathf.Clamp01(saturation);
        // In a real implementation, this would connect to post-processing
        // For now, we'll just store the value
    }
    
    public void SetContrast(float contrast)
    {
        contrastLevel = Mathf.Clamp(contrast, 0.5f, 2f);
        // In a real implementation, this would connect to post-processing
        // For now, we'll just store the value
    }
    
    public void SetBloomIntensity(float bloom)
    {
        bloomIntensity = Mathf.Clamp01(bloom);
        // In a real implementation, this would connect to post-processing
        // For now, we'll just store the value
    }
    
    public void SetTimeOfDay(float hour)
    {
        // Adjust lighting and colors based on time of day
        // This would typically be handled by the FarmEnvironment script
        if (GetComponent<FarmEnvironment>() != null)
        {
            GetComponent<FarmEnvironment>().SetTime(hour);
        }
    }
    
    private System.Collections.IEnumerator AutoDestroyEffect(GameObject effect, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (effect != null)
        {
            Destroy(effect);
        }
    }
    
    public void CleanUpAllEffects()
    {
        List<string> keys = new List<string>(activeEffects.Keys);
        foreach (string key in keys)
        {
            if (activeEffects[key] != null)
            {
                Destroy(activeEffects[key]);
            }
            activeEffects.Remove(key);
        }
    }
    
    void OnApplicationQuit()
    {
        CleanUpAllEffects();
    }
}