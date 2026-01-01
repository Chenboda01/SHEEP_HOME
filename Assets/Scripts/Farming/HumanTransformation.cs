using UnityEngine;

public class HumanTransformation : MonoBehaviour
{
    [Header("Transformation Settings")]
    public bool canTransform = false;
    public int requiredLevel = 10;
    public float transformationDuration = 2.0f;
    
    [Header("Player References")]
    public GameObject animalForm;
    public GameObject humanForm;
    public Camera animalCamera;
    public Camera humanCamera;
    
    [Header("Visual Effects")]
    public GameObject transformationEffect;
    public AnimationCurve transformationCurve;
    
    private bool isTransforming = false;
    private bool isHuman = false;
    private float transformationTimer = 0f;
    
    void Start()
    {
        InitializeTransformation();
    }
    
    void Update()
    {
        CheckForTransformation();
        UpdateTransformation();
    }
    
    private void InitializeTransformation()
    {
        // Initially, player is in animal form
        SwitchToAnimalForm();
        
        // Check if player has reached required level
        if (TaskManager.Instance != null)
        {
            canTransform = TaskManager.Instance.playerLevel >= requiredLevel;
        }
    }
    
    private void CheckForTransformation()
    {
        // Check if player can transform and presses the transform key
        if (canTransform && !isTransforming && Input.GetKeyDown(KeyCode.H))
        {
            if (TaskManager.Instance != null && TaskManager.Instance.playerLevel >= requiredLevel)
            {
                ToggleTransformation();
            }
        }
    }
    
    private void ToggleTransformation()
    {
        if (isTransforming) return;
        
        isTransforming = true;
        transformationTimer = 0f;
        
        if (transformationEffect != null)
        {
            Instantiate(transformationEffect, transform.position, Quaternion.identity);
        }
        
        if (GameManager.Instance != null && GameManager.Instance.audioManager != null)
        {
            GameManager.Instance.audioManager.PlaySound("Transformation");
        }
    }
    
    private void UpdateTransformation()
    {
        if (isTransforming)
        {
            transformationTimer += Time.deltaTime;
            
            if (transformationTimer >= transformationDuration)
            {
                CompleteTransformation();
            }
        }
    }
    
    private void CompleteTransformation()
    {
        isTransforming = false;
        
        if (isHuman)
        {
            SwitchToAnimalForm();
            isHuman = false;
            Debug.Log("Transformed back to animal form");
        }
        else
        {
            SwitchToHumanForm();
            isHuman = true;
            Debug.Log("Transformed to human form! Movement and interaction improved.");
        }
    }
    
    private void SwitchToAnimalForm()
    {
        if (animalForm != null) animalForm.SetActive(true);
        if (humanForm != null) humanForm.SetActive(false);
        
        if (animalCamera != null) animalCamera.enabled = true;
        if (humanCamera != null) humanCamera.enabled = false;
        
        // Adjust player controller settings for animal form
        PlayerController playerController = GetComponent<PlayerController>();
        if (playerController != null)
        {
            // Animal form might have different movement characteristics
            playerController.walkSpeed = 3.0f;  // Slower in animal form
            playerController.runSpeed = 5.0f;   // Still slower than human
        }
    }
    
    private void SwitchToHumanForm()
    {
        if (animalForm != null) animalForm.SetActive(false);
        if (humanForm != null) humanForm.SetActive(true);
        
        if (animalCamera != null) animalCamera.enabled = false;
        if (humanCamera != null) humanCamera.enabled = true;
        
        // Adjust player controller settings for human form
        PlayerController playerController = GetComponent<PlayerController>();
        if (playerController != null)
        {
            // Human form has improved movement
            playerController.walkSpeed = 5.0f;  // Standard human speed
            playerController.runSpeed = 8.0f;   // Faster running
        }
    }
    
    public void CheckLevelRequirement()
    {
        if (TaskManager.Instance != null)
        {
            canTransform = TaskManager.Instance.playerLevel >= requiredLevel;
            
            if (TaskManager.Instance.playerLevel >= requiredLevel && !canTransform)
            {
                Debug.Log("You've reached level " + requiredLevel + "! Press H to transform to human form.");
            }
        }
    }
    
    public bool IsHuman()
    {
        return isHuman;
    }
    
    public bool IsTransforming()
    {
        return isTransforming;
    }
    
    void OnGUI()
    {
        // Show transformation instructions if player has reached level 10
        if (TaskManager.Instance != null && 
            TaskManager.Instance.playerLevel >= requiredLevel && 
            !isHuman)
        {
            GUI.Box(new Rect(Screen.width / 2 - 100, 30, 200, 30), "Press H to transform to human");
        }
    }
}