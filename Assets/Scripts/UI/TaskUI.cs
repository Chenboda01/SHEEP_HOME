using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TaskUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Text taskTitleText;
    public Text taskDescriptionText;
    public Text taskProgressText;
    public Slider taskProgressSlider;
    public Text scoreText;
    public Text levelText;
    public Text experienceText;
    
    [Header("UI Panels")]
    public GameObject taskPanel;
    public GameObject pauseMenu;
    
    void Start()
    {
        InitializeUI();
    }
    
    void Update()
    {
        UpdateUI();
    }
    
    private void InitializeUI()
    {
        if (taskPanel != null) taskPanel.SetActive(true);
        if (pauseMenu != null) pauseMenu.SetActive(false);
        
        UpdateUI();
    }
    
    private void UpdateUI()
    {
        // Update score
        if (scoreText != null && TaskManager.Instance != null)
        {
            scoreText.text = "Score: " + TaskManager.Instance.playerScore;
        }
        
        // Update level
        if (levelText != null && TaskManager.Instance != null)
        {
            levelText.text = "Level: " + TaskManager.Instance.playerLevel;
        }
        
        // Update experience
        if (experienceText != null && TaskManager.Instance != null)
        {
            experienceText.text = TaskManager.Instance.experience + "/" + TaskManager.Instance.experienceToNextLevel + " XP";
        }
        
        // Update active task information
        UpdateTaskInfo();
    }
    
    private void UpdateTaskInfo()
    {
        if (TaskManager.Instance == null) return;
        
        List<FarmingTask> activeTasks = TaskManager.Instance.activeTasks;
        FarmingTask currentTask = null;
        
        // Find the first incomplete task
        foreach (FarmingTask task in activeTasks)
        {
            if (!task.isCompleted)
            {
                currentTask = task;
                break;
            }
        }
        
        if (currentTask != null)
        {
            if (taskTitleText != null)
                taskTitleText.text = currentTask.taskName;
            
            if (taskDescriptionText != null)
                taskDescriptionText.text = currentTask.taskDescription;
            
            if (taskProgressText != null)
                taskProgressText.text = currentTask.currentCount + "/" + currentTask.targetCount;
            
            if (taskProgressSlider != null)
                taskProgressSlider.value = currentTask.GetProgress();
        }
        else
        {
            if (taskTitleText != null)
                taskTitleText.text = "No active tasks";
            
            if (taskDescriptionText != null)
                taskDescriptionText.text = "Complete tasks to earn rewards and level up!";
            
            if (taskProgressText != null)
                taskProgressText.text = "";
            
            if (taskProgressSlider != null)
                taskProgressSlider.value = 0;
        }
    }
    
    public void TogglePauseMenu()
    {
        if (pauseMenu != null)
        {
            bool isActive = !pauseMenu.activeSelf;
            pauseMenu.SetActive(isActive);
            
            if (GameManager.Instance != null)
            {
                GameManager.Instance.isPaused = isActive;
                Time.timeScale = isActive ? 0 : 1;
                Cursor.visible = isActive;
                Cursor.lockState = isActive ? CursorLockMode.None : CursorLockMode.Locked;
            }
        }
    }
}