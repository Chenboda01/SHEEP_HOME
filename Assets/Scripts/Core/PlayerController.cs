using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5.0f;
    public float runSpeed = 8.0f;
    public float jumpForce = 7.0f;
    
    [Header("Camera Settings")]
    public float mouseSensitivity = 2.0f;
    public float maxVerticalAngle = 80.0f;
    
    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    
    // Private variables
    private CharacterController controller;
    private Camera playerCamera;
    private Vector3 velocity;
    private bool isGrounded;
    private float verticalAngle;
    private float currentSpeed;
    
    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerCamera = Camera.main;
        
        if (playerCamera == null)
        {
            playerCamera = GetComponentInChildren<Camera>();
        }
        
        // Lock cursor to center of screen
        Cursor.lockState = CursorLockMode.Locked;
        currentSpeed = walkSpeed;
    }
    
    void Update()
    {
        HandleGroundCheck();
        HandleMovement();
        HandleJumping();
        HandleRunning();
        HandleLooking();
    }
    
    private void HandleGroundCheck()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Small downward force to keep player grounded
        }
    }
    
    private void HandleMovement()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        
        Vector3 move = transform.right * x + transform.forward * z;
        
        // Apply movement to the character controller
        controller.Move(move * currentSpeed * Time.deltaTime);
    }
    
    private void HandleJumping()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * Physics.gravity.y);
        }
        
        // Apply gravity
        velocity.y += Physics.gravity.y * Time.deltaTime;
        
        // Apply vertical velocity
        controller.Move(velocity * Time.deltaTime);
    }
    
    private void HandleRunning()
    {
        if (Input.GetKey(KeyCode.LeftShift) && isGrounded)
        {
            currentSpeed = runSpeed;
        }
        else
        {
            currentSpeed = walkSpeed;
        }
    }
    
    private void HandleLooking()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        
        transform.Rotate(Vector3.up * mouseX);
        
        verticalAngle -= mouseY;
        verticalAngle = Mathf.Clamp(verticalAngle, -maxVerticalAngle, maxVerticalAngle);
        
        if (playerCamera != null)
        {
            playerCamera.transform.localRotation = Quaternion.Euler(verticalAngle, 0f, 0f);
        }
    }
}