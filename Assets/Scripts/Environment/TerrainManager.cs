using UnityEngine;
using System.Collections.Generic;

public class TerrainManager : MonoBehaviour
{
    [Header("Terrain Settings")]
    public int terrainWidth = 500;
    public int terrainLength = 500;
    public float terrainHeight = 50f;
    
    [Header("Terrain Textures")]
    public Texture2D[] terrainTextures;
    public float[] textureScales;
    
    [Header("Terrain Objects")]
    public GameObject[] farmObjectsPrefabs;
    public int numberOfObjectsToSpawn = 20;
    
    [Header("Terrain Generation")]
    public bool autoGenerate = true;
    public float terrainScaleFactor = 0.02f;
    public float heightMultiplier = 20f;
    
    private Terrain terrain;
    private TerrainData terrainData;
    
    void Start()
    {
        if (autoGenerate)
        {
            GenerateTerrain();
        }
    }
    
    public void GenerateTerrain()
    {
        // Create terrain data
        terrainData = new TerrainData();
        terrainData.size = new Vector3(terrainWidth, terrainHeight, terrainLength);
        
        // Set terrain resolution
        terrainData.heightmapResolution = 513; // Must be 2^n + 1
        terrainData.alphamapResolution = 256;
        terrainData.baseMapResolution = 256;
        
        // Set terrain heightmap
        SetHeightmap();
        
        // Set terrain textures
        SetTerrainTextures();
        
        // Create terrain game object
        GameObject terrainObject = Terrain.CreateTerrainGameObject(terrainData);
        terrainObject.name = "FarmTerrain";
        terrainObject.transform.SetParent(transform);
        
        terrain = terrainObject.GetComponent<Terrain>();
        
        // Add collider for player movement
        terrain.gameObject.AddComponent<TerrainCollider>();
        
        // Spawn farm objects
        SpawnFarmObjects();
    }
    
    private void SetHeightmap()
    {
        int heightmapResolution = terrainData.heightmapResolution;
        float[,] heights = new float[heightmapResolution, heightmapResolution];
        
        for (int x = 0; x < heightmapResolution; x++)
        {
            for (int y = 0; y < heightmapResolution; y++)
            {
                // Generate height using Perlin noise
                float xCoord = (float)x / heightmapResolution * terrainScaleFactor;
                float yCoord = (float)y / heightmapResolution * terrainScaleFactor;
                
                float height = Mathf.PerlinNoise(xCoord, yCoord) * heightMultiplier;
                
                // Normalize height to 0-1 range
                heights[x, y] = height / terrainHeight;
            }
        }
        
        terrainData.SetHeights(0, 0, heights);
    }
    
    private void SetTerrainTextures()
    {
        if (terrainTextures.Length == 0) return;
        
        // Create array of SplatPrototype for terrain textures
        SplatPrototype[] splatPrototypes = new SplatPrototype[terrainTextures.Length];
        
        for (int i = 0; i < terrainTextures.Length; i++)
        {
            splatPrototypes[i] = new SplatPrototype();
            splatPrototypes[i].texture = terrainTextures[i];
            splatPrototypes[i].tileSize = new Vector3(
                textureScales.Length > i ? textureScales[i] : 10f,
                textureScales.Length > i ? textureScales[i] : 10f
            );
        }
        
        terrainData.splatPrototypes = splatPrototypes;
        
        // Create alphamap for texture blending
        int alphamapWidth = terrainData.alphamapWidth;
        int alphamapHeight = terrainData.alphamapHeight;
        int numTextures = terrainTextures.Length;
        
        float[,,] alphamaps = new float[alphamapWidth, alphamapHeight, numTextures];
        
        for (int y = 0; y < alphamapHeight; y++)
        {
            for (int x = 0; x < alphamapWidth; x++)
            {
                // Simple texture distribution based on height
                float normalizedX = (float)x / alphamapWidth;
                float normalizedY = (float)y / alphamapHeight;
                
                float height = terrainData.GetHeight(Mathf.RoundToInt(normalizedX * (terrainData.heightmapResolution - 1)),
                                                   Mathf.RoundToInt(normalizedY * (terrainData.heightmapResolution - 1))) / terrainHeight;
                
                // Distribute textures based on height and position
                float total = 0f;
                
                for (int i = 0; i < numTextures; i++)
                {
                    float textureValue = CalculateTextureValue(i, height, normalizedX, normalizedY);
                    alphamaps[y, x, i] = textureValue;
                    total += textureValue;
                }
                
                // Normalize values so they sum to 1
                if (total > 0)
                {
                    for (int i = 0; i < numTextures; i++)
                    {
                        alphamaps[y, x, i] /= total;
                    }
                }
            }
        }
        
        terrainData.SetAlphamaps(0, 0, alphamaps);
    }
    
    private float CalculateTextureValue(int textureIndex, float height, float x, float y)
    {
        // Simple distribution algorithm
        // Different textures at different heights and positions
        switch (textureIndex)
        {
            case 0: // Grass
                return Mathf.Clamp01(1f - height * 2f); // More grass at lower elevations
            case 1: // Dirt
                return Mathf.Clamp01(height * 2f - 0.5f); // More dirt at medium elevations
            case 2: // Rock
                return Mathf.Clamp01(height - 0.7f); // More rock at higher elevations
            default:
                return 1f / terrainTextures.Length; // Even distribution for additional textures
        }
    }
    
    private void SpawnFarmObjects()
    {
        if (farmObjectsPrefabs.Length == 0) return;
        
        for (int i = 0; i < numberOfObjectsToSpawn; i++)
        {
            // Random position on the terrain
            float x = Random.Range(0f, terrainWidth);
            float z = Random.Range(0f, terrainLength);
            
            // Get the correct height at this position
            float y = terrain.SampleHeight(new Vector3(x, 0, z)) + 1f; // +1 to place above ground
            
            Vector3 spawnPosition = new Vector3(x, y, z);
            
            // Select a random object prefab
            GameObject prefab = farmObjectsPrefabs[Random.Range(0, farmObjectsPrefabs.Length)];
            
            // Instantiate the object
            GameObject spawnedObject = Instantiate(prefab, spawnPosition, Quaternion.identity);
            spawnedObject.transform.SetParent(transform);
            
            // Random rotation for natural look
            spawnedObject.transform.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
        }
    }
    
    public float GetTerrainHeightAtPosition(Vector3 position)
    {
        if (terrain != null)
        {
            return terrain.SampleHeight(position);
        }
        return 0f;
    }
    
    public Vector3 GetTerrainNormalAtPosition(Vector3 position)
    {
        if (terrain != null)
        {
            return terrain.terrainData.GetInterpolatedNormal(
                (position.x - terrain.transform.position.x) / terrain.terrainData.size.x,
                (position.z - terrain.transform.position.z) / terrain.terrainData.size.z
            );
        }
        return Vector3.up;
    }
}