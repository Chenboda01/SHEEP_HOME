using UnityEngine;

public abstract class InteractiveObject : MonoBehaviour
{
    [Header("Interaction Settings")]
    public string objectName = "Interactive Object";
    public string interactionText = "Press E to interact";
    public bool canInteract = true;
    
    [Header("Visual Feedback")]
    public GameObject highlightEffect;
    public Color highlightColor = Color.yellow;
    
    protected bool isHighlighted = false;
    protected Renderer[] renderers;
    
    // Store original material colors for restoration
    protected Color[] originalColors;
    protected Material[] originalMaterials;
    
    void Start()
    {
        InitializeObject();
    }
    
    void Update()
    {
        CheckForInteraction();
    }
    
    protected virtual void InitializeObject()
    {
        // Get all renderers on this object and children
        renderers = GetComponentsInChildren<Renderer>();
        
        // Store original material properties
        StoreOriginalMaterials();
    }
    
    protected void StoreOriginalMaterials()
    {
        if (renderers.Length > 0)
        {
            originalMaterials = new Material[renderers.Length];
            originalColors = new Color[renderers.Length];
            
            for (int i = 0; i < renderers.Length; i++)
            {
                originalMaterials[i] = renderers[i].material;
                originalColors[i] = renderers[i].material.color;
            }
        }
    }
    
    public void Highlight(bool state)
    {
        if (!canInteract) return;
        
        isHighlighted = state;
        
        if (renderers.Length > 0)
        {
            foreach (Renderer renderer in renderers)
            {
                if (state)
                {
                    // Create a new material to avoid changing the original
                    Material newMaterial = new Material(renderer.material);
                    newMaterial.color = highlightColor;
                    renderer.material = newMaterial;
                }
                else
                {
                    // Restore original material
                    int index = -1;
                    for (int i = 0; i < renderers.Length; i++)
                    {
                        if (renderers[i] == renderer)
                        {
                            index = i;
                            break;
                        }
                    }
                    
                    if (index >= 0 && index < originalMaterials.Length)
                    {
                        renderer.material = originalMaterials[index];
                    }
                }
            }
        }
        
        if (highlightEffect != null)
        {
            highlightEffect.SetActive(state);
        }
    }
    
    private void CheckForInteraction()
    {
        if (canInteract && IsPlayerNearby() && Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }
    
    private bool IsPlayerNearby()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            return distance < 3.0f; // Interaction distance
        }
        return false;
    }
    
    // This method should be implemented by derived classes
    public abstract void Interact();
    
    void OnMouseEnter()
    {
        if (canInteract)
        {
            Highlight(true);
        }
    }
    
    void OnMouseExit()
    {
        Highlight(false);
    }
    
    void OnDestroy()
    {
        // Clean up dynamically created materials
        if (renderers != null && renderers.Length > 0)
        {
            foreach (Renderer renderer in renderers)
            {
                if (renderer != null && renderer.material != null && 
                    renderer.material != originalMaterials[0]) // Only destroy if it's not original
                {
                    Destroy(renderer.material);
                }
            }
        }
    }
}