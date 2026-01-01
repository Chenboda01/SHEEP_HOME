using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class FarmingTask
{
    public string taskName;
    public string taskDescription;
    public int targetCount;
    public int currentCount;
    public int reward;
    public bool isCompleted;
    public bool isVisible;
    
    public FarmingTask(string name, string description, int target, int rewardValue)
    {
        taskName = name;
        taskDescription = description;
        targetCount = target;
        currentCount = 0;
        reward = rewardValue;
        isCompleted = false;
        isVisible = true;
    }
    
    public void IncrementProgress(int amount = 1)
    {
        if (!isCompleted)
        {
            currentCount = Mathf.Min(currentCount + amount, targetCount);
            if (currentCount >= targetCount)
            {
                isCompleted = true;
            }
        }
    }
    
    public float GetProgress()
    {
        return targetCount > 0 ? (float)currentCount / targetCount : 0f;
    }
}

public class TaskManager : MonoBehaviour
{
    public static TaskManager Instance { get; private set; }
    
    [Header("Available Tasks")]
    public List<FarmingTask> availableTasks = new List<FarmingTask>();
    
    [Header("Active Tasks")]
    public List<FarmingTask> activeTasks = new List<FarmingTask>();
    
    [Header("Player Progress")]
    public int playerScore = 0;
    public int playerLevel = 1;
    public int experience = 0;
    public int experienceToNextLevel = 100;
    
    [Header("Task Events")]
    public bool showTaskNotifications = true;
    
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
        
        InitializeTasks();
    }
    
    void InitializeTasks()
    {
        // Define initial tasks for the game
        availableTasks.Add(new FarmingTask("Plant 5 Crops", "Plant 5 crops in the farm", 5, 50));
        availableTasks.Add(new FarmingTask("Harvest 3 Crops", "Harvest 3 fully grown crops", 3, 75));
        availableTasks.Add(new FarmingTask("Water 10 Plants", "Water 10 planted crops", 10, 40));
        availableTasks.Add(new FarmingTask("Collect Eggs", "Collect 5 eggs from chickens", 5, 60));
        availableTasks.Add(new FarmingTask("Milk Cows", "Milk 3 cows", 3, 80));
        
        // Start with the first task
        if (availableTasks.Count > 0)
        {
            StartNewTask(availableTasks[0]);
        }
    }
    
    public void StartNewTask(FarmingTask task)
    {
        if (task != null && !activeTasks.Contains(task))
        {
            activeTasks.Add(task);
            Debug.Log("Started new task: " + task.taskName);
            
            if (showTaskNotifications)
            {
                ShowTaskNotification(task);
            }
        }
    }
    
    public void CompleteTask(string taskName, int value = 1)
    {
        FarmingTask task = activeTasks.Find(t => t.taskName.ToLower().Contains(taskName.ToLower()) && !t.isCompleted);
        
        if (task != null)
        {
            task.IncrementProgress(value);
            
            // Add to player score
            playerScore += value * 10; // Base value
            
            if (task.isCompleted)
            {
                playerScore += task.reward; // Completion reward
                AddExperience(task.reward / 2); // Experience for completion
                
                Debug.Log("Completed task: " + task.taskName + " + " + task.reward + " points!");
                
                if (showTaskNotifications)
                {
                    ShowTaskCompletedNotification(task);
                }
                
                // Start a new task when current one is completed
                StartNextTask();
            }
            else
            {
                Debug.Log("Task progress: " + task.currentCount + "/" + task.targetCount);
            }
        }
    }
    
    public void AddExperience(int exp)
    {
        experience += exp;
        
        // Check for level up
        while (experience >= experienceToNextLevel)
        {
            LevelUp();
        }
    }
    
    private void LevelUp()
    {
        experience -= experienceToNextLevel;
        playerLevel++;
        experienceToNextLevel = Mathf.RoundToInt(experienceToNextLevel * 1.2f); // Increase XP needed for next level
        
        Debug.Log("Level Up! Reached level " + playerLevel);
        
        // Check for special level milestones
        if (playerLevel == 10)
        {
            Debug.Log("Congratulations! You've reached level 10. You can now transform into a human!");
            // Trigger human transformation unlock
        }
        
        if (showTaskNotifications)
        {
            ShowLevelUpNotification();
        }
    }
    
    private void StartNextTask()
    {
        // Find next incomplete task
        FarmingTask nextTask = availableTasks.Find(task => 
            !activeTasks.Contains(task) && 
            !task.isCompleted && 
            task.isVisible
        );
        
        if (nextTask != null)
        {
            StartNewTask(nextTask);
        }
        else
        {
            // If no more tasks, create a random one or repeat
            CreateRandomTask();
        }
    }
    
    private void CreateRandomTask()
    {
        // Create a simple random task
        string[] taskTypes = { "Plant", "Harvest", "Water" };
        string[] objects = { "Carrots", "Tomatoes", "Corn", "Potatoes", "Wheat" };
        
        string randomTaskType = taskTypes[Random.Range(0, taskTypes.Length)];
        string randomObject = objects[Random.Range(0, objects.Length)];
        int randomTarget = Random.Range(3, 8);
        int randomReward = Random.Range(30, 70);
        
        FarmingTask randomTask = new FarmingTask(
            randomTaskType + " " + randomTarget + " " + randomObject,
            "Complete the farming task: " + randomTaskType + " " + randomTarget + " " + randomObject,
            randomTarget,
            randomReward
        );
        
        availableTasks.Add(randomTask);
        StartNewTask(randomTask);
    }
    
    private void ShowTaskNotification(FarmingTask task)
    {
        // In a real implementation, this would show a UI notification
        Debug.Log("NEW TASK: " + task.taskName + " - " + task.taskDescription);
    }
    
    private void ShowTaskCompletedNotification(FarmingTask task)
    {
        // In a real implementation, this would show a UI notification
        Debug.Log("TASK COMPLETED: " + task.taskName + " - Reward: " + task.reward + " points");
    }
    
    private void ShowLevelUpNotification()
    {
        // In a real implementation, this would show a UI notification
        Debug.Log("LEVEL UP: Congratulations! You reached level " + playerLevel);
    }
    
    public void ResetTasks()
    {
        activeTasks.Clear();
        availableTasks.Clear();
        InitializeTasks();
    }
    
    public FarmingTask GetActiveTask(string taskName)
    {
        return activeTasks.Find(t => t.taskName.ToLower().Contains(taskName.ToLower()));
    }
    
    public List<FarmingTask> GetCompletedTasks()
    {
        return activeTasks.FindAll(t => t.isCompleted);
    }
}