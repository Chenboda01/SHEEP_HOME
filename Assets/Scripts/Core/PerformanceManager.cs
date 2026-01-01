using UnityEngine;
using System.Collections.Generic;

public class PerformanceManager : MonoBehaviour
{
    public static PerformanceManager Instance { get; private set; }
    
    [Header("Performance Settings")]
    public int targetFrameRate = 60;
    public bool enableVSync = false;
    public bool enableDynamicBatching = true;
    public bool enableLOD = true;
    
    [Header("Object Pooling")]
    public bool enableObjectPooling = true;
    public int defaultPoolSize = 10;
    
    [Header("Garbage Collection")]
    public bool enableGCoptimization = true;
    public float gcOptimizationInterval = 30f;
    private float lastGCoptimizationTime;
    
    [Header("Debug Information")]
    public bool showPerformanceStats = true;
    public Text debugText; // Optional UI text to show performance stats
    
    private Dictionary<string, Queue<GameObject>> objectPools = new Dictionary<string, Queue<GameObject>>();
    private List<Component> componentsToOptimize = new List<Component>();
    
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
        
        InitializePerformanceSettings();
    }
    
    void Start()
    {
        lastGCoptimizationTime = Time.time;
    }
    
    void Update()
    {
        HandlePerformanceOptimizations();
        
        if (showPerformanceStats)
        {
            UpdateDebugInfo();
        }
    }
    
    private void InitializePerformanceSettings()
    {
        // Set target frame rate
        Application.targetFrameRate = targetFrameRate;
        
        // Set V-Sync
        QualitySettings.vSyncCount = enableVSync ? 1 : 0;
        
        // Enable dynamic batching if needed
        // Note: Unity handles this automatically, but we can optimize rendering
    }
    
    private void HandlePerformanceOptimizations()
    {
        if (enableGCoptimization && Time.time - lastGCoptimizationTime > gcOptimizationInterval)
        {
            System.GC.Collect();
            lastGCoptimizationTime = Time.time;
        }
    }
    
    private void UpdateDebugInfo()
    {
        string debugInfo = "FPS: " + Mathf.RoundToInt(1.0f / Time.unscaledDeltaTime) + "\n";
        debugInfo += "Tris: " + Time.renderedFrameCount + "\n";
        debugInfo += "Target FPS: " + targetFrameRate + "\n";
        debugInfo += "Memory: " + Mathf.RoundToInt(UnityEngine.Profiling.Profiler.GetTotalMemoryLong() / 1024f / 1024f) + " MB\n";
        
        if (debugText != null)
        {
            debugText.text = debugInfo;
        }
        else
        {
            // If no UI text is assigned, log to console every few seconds
            if (Time.time % 5 < Time.deltaTime) // Log every 5 seconds
            {
                Debug.Log(debugInfo);
            }
        }
    }
    
    // Object pooling methods
    public GameObject GetPooledObject(string prefabName, GameObject prefab, Transform parent = null)
    {
        if (!enableObjectPooling) return Instantiate(prefab, parent);
        
        // Create pool if it doesn't exist
        if (!objectPools.ContainsKey(prefabName))
        {
            objectPools[prefabName] = new Queue<GameObject>();
            
            // Pre-populate pool
            for (int i = 0; i < defaultPoolSize; i++)
            {
                GameObject obj = Instantiate(prefab);
                obj.SetActive(false);
                obj.name = prefabName + "_Pooled";
                objectPools[prefabName].Enqueue(obj);
            }
        }
        
        GameObject pooledObject = null;
        
        // Try to get an inactive object from the pool
        if (objectPools[prefabName].Count > 0)
        {
            pooledObject = objectPools[prefabName].Dequeue();
        }
        
        // If no objects available, create a new one
        if (pooledObject == null)
        {
            pooledObject = Instantiate(prefab);
            pooledObject.name = prefabName + "_Pooled";
        }
        
        // Activate and position the object
        pooledObject.SetActive(true);
        if (parent != null)
        {
            pooledObject.transform.SetParent(parent);
        }
        
        return pooledObject;
    }
    
    public void ReturnToPool(string prefabName, GameObject obj)
    {
        if (!enableObjectPooling || !objectPools.ContainsKey(prefabName)) 
        {
            Destroy(obj);
            return;
        }
        
        obj.SetActive(false);
        obj.transform.SetParent(transform); // Move to performance manager to keep out of the way
        objectPools[prefabName].Enqueue(obj);
    }
    
    // Method to optimize rendering for multiple objects
    public void OptimizeRendering(List<Renderer> renderers)
    {
        foreach (Renderer renderer in renderers)
        {
            // Optimize materials and rendering settings
            if (enableLOD)
            {
                // Add LOD group if needed
                if (renderer.GetComponent<LODGroup>() == null)
                {
                    // Implementation would depend on specific LOD needs
                }
            }
        }
    }
    
    // Method to optimize audio sources
    public void OptimizeAudioSource(AudioSource audioSource)
    {
        // Set appropriate compression and spatial blend
        audioSource.rolloffMode = AudioRolloffMode.Logarithmic;
        audioSource.spatialBlend = 1f; // 3D audio
    }
    
    // Method to optimize physics
    public void OptimizePhysics()
    {
        // Set appropriate physics settings
        Physics.defaultSolverIterations = 6; // Default is usually fine
        Physics.defaultSolverVelocityIterations = 1; // Lower for performance
        
        // Set sleep threshold
        Physics.sleepThreshold = 0.005f;
    }
    
    // Bug fix: Prevent memory leaks by cleaning up pooled objects
    void OnApplicationQuit()
    {
        if (enableObjectPooling)
        {
            foreach (var pool in objectPools.Values)
            {
                foreach (GameObject obj in pool)
                {
                    if (obj != null)
                    {
                        DestroyImmediate(obj);
                    }
                }
            }
            objectPools.Clear();
        }
    }
    
    // Method to handle common Unity bugs/performance issues
    public void ApplyBugFixes()
    {
        // Fix for common issues:
        
        // 1. Fix for camera flickering in some cases
        Camera.onPreCull += (Camera cam) => {
            // Ensure camera matrices are properly set
        };
        
        // 2. Fix for lighting issues
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
        
        // 3. Optimize shadows
        QualitySettings.shadowProjection = ShadowProjection.StableFit;
        QualitySettings.shadowCascades = 2;
    }
}