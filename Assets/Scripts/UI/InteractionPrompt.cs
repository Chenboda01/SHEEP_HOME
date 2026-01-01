using UnityEngine;
using UnityEngine.UI;

public class InteractionPrompt : MonoBehaviour
{
    [Header("UI Elements")]
    public Text promptText;
    public GameObject promptPanel;
    
    [Header("Prompt Settings")]
    public float promptDistance = 3f;
    public LayerMask interactiveLayerMask;
    
    private Camera playerCamera;
    private InteractiveObject currentInteractiveObject;
    
    void Start()
    {
        playerCamera = Camera.main;
        if (playerCamera == null)
        {
            playerCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        }
        
        if (promptPanel != null)
        {
            promptPanel.SetActive(false);
        }
    }
    
    void Update()
    {
        UpdateInteractionPrompt();
    }
    
    private void UpdateInteractionPrompt()
    {
        if (playerCamera == null) return;
        
        // Raycast to find interactive objects
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, promptDistance, interactiveLayerMask))
        {
            InteractiveObject interactable = hit.collider.GetComponent<InteractiveObject>();
            
            if (interactable != null && interactable.canInteract)
            {
                // Found a new interactive object
                if (currentInteractiveObject != interactable)
                {
                    // Hide previous prompt
                    if (currentInteractiveObject != null)
                    {
                        currentInteractiveObject.Highlight(false);
                    }
                    
                    // Show new prompt
                    currentInteractiveObject = interactable;
                    currentInteractiveObject.Highlight(true);
                    
                    if (promptPanel != null && promptText != null)
                    {
                        promptPanel.SetActive(true);
                        promptText.text = interactable.interactionText;
                    }
                }
            }
            else
            {
                // No valid interactive object found
                if (currentInteractiveObject != null)
                {
                    currentInteractiveObject.Highlight(false);
                    currentInteractiveObject = null;
                    
                    if (promptPanel != null)
                    {
                        promptPanel.SetActive(false);
                    }
                }
            }
        }
        else
        {
            // No raycast hit
            if (currentInteractiveObject != null)
            {
                currentInteractiveObject.Highlight(false);
                currentInteractiveObject = null;
                
                if (promptPanel != null)
                {
                    promptPanel.SetActive(false);
                }
            }
        }
    }
    
    public void ForceHidePrompt()
    {
        if (currentInteractiveObject != null)
        {
            currentInteractiveObject.Highlight(false);
            currentInteractiveObject = null;
        }
        
        if (promptPanel != null)
        {
            promptPanel.SetActive(false);
        }
    }
}