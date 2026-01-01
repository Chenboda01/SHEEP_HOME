using UnityEngine;

public class Crop : InteractiveObject
{
    [Header("Crop Settings")]
    public enum CropState { Planted, Growing, ReadyToHarvest, Withered }
    public CropState currentState = CropState.Planted;
    
    [Header("Growth Settings")]
    public float growthTime = 10f; // Time in seconds for full growth
    public float witherTime = 30f;  // Time in seconds before crop withers
    private float growthTimer = 0f;
    private float witherTimer = 0f;
    
    [Header("Visual Elements")]
    public GameObject[] growthStages; // Different models for each growth stage
    public GameObject harvestedEffect;
    
    [Header("Crop Properties")]
    public int harvestValue = 10; // Value when harvested
    public string cropType = "Generic Crop";
    
    void Update()
    {
        CheckForInteraction();
        UpdateGrowth();
    }
    
    protected override void InitializeObject()
    {
        base.InitializeObject();
        objectName = cropType + " Crop";
        interactionText = "Press E to " + GetActionText();
        
        // Hide all growth stages initially
        foreach (GameObject stage in growthStages)
        {
            if (stage != null) stage.SetActive(false);
        }
        
        // Show the first growth stage
        if (growthStages.Length > 0 && growthStages[0] != null)
        {
            growthStages[0].SetActive(true);
        }
    }
    
    private void UpdateGrowth()
    {
        if (currentState == CropState.Growing)
        {
            growthTimer += Time.deltaTime;
            
            if (growthTimer >= growthTime)
            {
                currentState = CropState.ReadyToHarvest;
                ShowGrowthStage(2); // Show fully grown stage
                interactionText = "Press E to Harvest";
            }
            else
            {
                // Update to appropriate growth stage based on progress
                int stageIndex = Mathf.Min(Mathf.FloorToInt((growthTimer / growthTime) * (growthStages.Length - 1)), growthStages.Length - 1);
                ShowGrowthStage(stageIndex);
            }
        }
        else if (currentState == CropState.Withered)
        {
            witherTimer += Time.deltaTime;
            
            if (witherTimer >= witherTime)
            {
                // Remove the crop after withering
                Destroy(gameObject);
            }
        }
    }
    
    private void ShowGrowthStage(int stageIndex)
    {
        // Hide all stages
        foreach (GameObject stage in growthStages)
        {
            if (stage != null) stage.SetActive(false);
        }
        
        // Show the specified stage
        if (stageIndex < growthStages.Length && growthStages[stageIndex] != null)
        {
            growthStages[stageIndex].SetActive(true);
        }
    }
    
    private string GetActionText()
    {
        switch (currentState)
        {
            case CropState.Planted:
                return "Water";
            case CropState.Growing:
                return "Wait";
            case CropState.ReadyToHarvest:
                return "Harvest";
            case CropState.Withered:
                return "Remove";
            default:
                return "Interact";
        }
    }
    
    public override void Interact()
    {
        switch (currentState)
        {
            case CropState.Planted:
                WaterCrop();
                break;
            case CropState.Growing:
                // Nothing to do while growing
                break;
            case CropState.ReadyToHarvest:
                HarvestCrop();
                break;
            case CropState.Withered:
                RemoveCrop();
                break;
        }
        
        interactionText = "Press E to " + GetActionText();
    }
    
    private void WaterCrop()
    {
        if (currentState == CropState.Planted)
        {
            currentState = CropState.Growing;
            growthTimer = 0f;
            Debug.Log("Watered the " + cropType + " crop");
            
            // Play watering sound
            if (GameManager.Instance != null && GameManager.Instance.audioManager != null)
            {
                GameManager.Instance.audioManager.PlaySound("Watering");
            }
        }
    }
    
    private void HarvestCrop()
    {
        if (currentState == CropState.ReadyToHarvest)
        {
            Debug.Log("Harvested " + cropType + " crop for " + harvestValue + " points");
            
            // Add to player's score/resources
            if (GameManager.Instance != null && GameManager.Instance.taskManager != null)
            {
                GameManager.Instance.taskManager.CompleteTask("HarvestCrop", harvestValue);
            }
            
            // Show harvest effect
            if (harvestedEffect != null)
            {
                Instantiate(harvestedEffect, transform.position, Quaternion.identity);
            }
            
            // Play harvest sound
            if (GameManager.Instance != null && GameManager.Instance.audioManager != null)
            {
                GameManager.Instance.audioManager.PlaySound("Harvest");
            }
            
            // Remove the crop
            Destroy(gameObject);
        }
    }
    
    private void RemoveCrop()
    {
        Debug.Log("Removed withered " + cropType + " crop");
        Destroy(gameObject);
    }
    
    public void Wither()
    {
        if (currentState != CropState.Withered)
        {
            currentState = CropState.Withered;
            witherTimer = 0f;
            
            // Visual change to indicate withering
            foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
            {
                Material newMaterial = new Material(renderer.material);
                newMaterial.color = Color.gray;
                renderer.material = newMaterial;
            }
        }
    }
}