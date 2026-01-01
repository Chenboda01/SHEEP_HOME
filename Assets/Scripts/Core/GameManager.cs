using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [Header("Game Settings")]
    public bool isPaused = false;
    
    [Header("Player References")]
    public GameObject player;
    
    [Header("Systems")]
    public AudioManager audioManager;
    public TaskManager taskManager;
    
    void Awake()
    {
        // Singleton pattern to ensure only one GameManager exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        InitializeGame();
    }
    
    void Update()
    {
        HandlePause();
    }
    
    private void InitializeGame()
    {
        Debug.Log("Game initialized");
        
        // Initialize player if not set
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        
        // Initialize other systems
        InitializeAudio();
        InitializeTaskSystem();
    }
    
    private void InitializeAudio()
    {
        if (audioManager == null)
        {
            audioManager = FindObjectOfType<AudioManager>();
        }
    }
    
    private void InitializeTaskSystem()
    {
        if (taskManager == null)
        {
            taskManager = FindObjectOfType<TaskManager>();
        }
    }
    
    private void HandlePause()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }
    
    public void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0 : 1;
        Cursor.visible = isPaused;
        Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
    }
    
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}