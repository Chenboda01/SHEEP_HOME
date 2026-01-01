using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class PlayerData
{
    public string username;
    public int playerLevel;
    public int playerScore;
    public int experience;
    public int experienceToNextLevel;
    public List<FarmingTaskData> completedTasks;
    public List<FarmingTaskData> activeTasks;
    public bool isHumanFormUnlocked;
    public float gameTime;
    
    public PlayerData(TaskManager taskManager, string name)
    {
        username = name;
        playerLevel = taskManager.playerLevel;
        playerScore = taskManager.playerScore;
        experience = taskManager.experience;
        experienceToNextLevel = taskManager.experienceToNextLevel;
        
        // Save task data
        completedTasks = new List<FarmingTaskData>();
        activeTasks = new List<FarmingTaskData>();
        
        foreach(FarmingTask task in taskManager.GetCompletedTasks())
        {
            completedTasks.Add(new FarmingTaskData(task));
        }
        
        foreach(FarmingTask task in taskManager.activeTasks)
        {
            if(!task.isCompleted)
            {
                activeTasks.Add(new FarmingTaskData(task));
            }
        }
        
        isHumanFormUnlocked = taskManager.playerLevel >= 10;
        gameTime = Time.timeSinceLevelLoad;
    }
}

[System.Serializable]
public class FarmingTaskData
{
    public string taskName;
    public string taskDescription;
    public int targetCount;
    public int currentCount;
    public int reward;
    public bool isCompleted;
    
    public FarmingTaskData(FarmingTask task)
    {
        taskName = task.taskName;
        taskDescription = task.taskDescription;
        targetCount = task.targetCount;
        currentCount = task.currentCount;
        reward = task.reward;
        isCompleted = task.isCompleted;
    }
}

public class UserAccount : MonoBehaviour
{
    public static UserAccount Instance { get; private set; }
    
    [Header("Account Settings")]
    public string currentUsername = "";
    public bool isAccountLoaded = false;
    
    [Header("Save Settings")]
    public bool autoSave = true;
    public float autoSaveInterval = 60f; // Save every minute
    private float lastSaveTime;
    
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
    }
    
    void Start()
    {
        lastSaveTime = Time.time;
    }
    
    void Update()
    {
        if (autoSave && Time.time - lastSaveTime > autoSaveInterval)
        {
            SaveGame();
            lastSaveTime = Time.time;
        }
    }
    
    public bool CreateAccount(string username)
    {
        if (string.IsNullOrEmpty(username))
        {
            Debug.LogError("Username cannot be empty");
            return false;
        }
        
        if (AccountExists(username))
        {
            Debug.LogError("Account with username " + username + " already exists");
            return false;
        }
        
        currentUsername = username;
        isAccountLoaded = true;
        
        // Initialize new game state
        if (TaskManager.Instance != null)
        {
            TaskManager.Instance.playerScore = 0;
            TaskManager.Instance.playerLevel = 1;
            TaskManager.Instance.experience = 0;
            TaskManager.Instance.experienceToNextLevel = 100;
        }
        
        Debug.Log("Account created for: " + username);
        return true;
    }
    
    public bool LoadAccount(string username)
    {
        if (string.IsNullOrEmpty(username))
        {
            Debug.LogError("Username cannot be empty");
            return false;
        }
        
        string savePath = GetSavePath(username);
        
        if (!File.Exists(savePath))
        {
            Debug.LogError("Account with username " + username + " does not exist");
            return false;
        }
        
        try
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream fileStream = new FileStream(savePath, FileMode.Open);
            
            PlayerData playerData = (PlayerData)formatter.Deserialize(fileStream);
            fileStream.Close();
            
            // Apply loaded data to game systems
            ApplyPlayerData(playerData);
            
            currentUsername = username;
            isAccountLoaded = true;
            
            Debug.Log("Account loaded for: " + username);
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error loading account: " + e.Message);
            return false;
        }
    }
    
    public void SaveGame()
    {
        if (string.IsNullOrEmpty(currentUsername))
        {
            Debug.LogError("No account loaded to save");
            return;
        }
        
        if (TaskManager.Instance == null)
        {
            Debug.LogError("TaskManager not found to save game data");
            return;
        }
        
        try
        {
            PlayerData playerData = new PlayerData(TaskManager.Instance, currentUsername);
            
            BinaryFormatter formatter = new BinaryFormatter();
            string savePath = GetSavePath(currentUsername);
            FileStream fileStream = new FileStream(savePath, FileMode.Create);
            
            formatter.Serialize(fileStream, playerData);
            fileStream.Close();
            
            Debug.Log("Game saved for: " + currentUsername);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error saving game: " + e.Message);
        }
    }
    
    private void ApplyPlayerData(PlayerData playerData)
    {
        if (TaskManager.Instance != null)
        {
            TaskManager.Instance.playerLevel = playerData.playerLevel;
            TaskManager.Instance.playerScore = playerData.playerScore;
            TaskManager.Instance.experience = playerData.experience;
            TaskManager.Instance.experienceToNextLevel = playerData.experienceToNextLevel;
            
            // Restore tasks
            TaskManager.Instance.activeTasks.Clear();
            
            // Add back active tasks
            foreach(FarmingTaskData taskData in playerData.activeTasks)
            {
                FarmingTask task = new FarmingTask(taskData.taskName, taskData.taskDescription, 
                                                 taskData.targetCount, taskData.reward);
                task.currentCount = taskData.currentCount;
                task.isCompleted = taskData.isCompleted;
                TaskManager.Instance.activeTasks.Add(task);
            }
            
            // Add back completed tasks
            foreach(FarmingTaskData taskData in playerData.completedTasks)
            {
                FarmingTask task = new FarmingTask(taskData.taskName, taskData.taskDescription, 
                                                 taskData.targetCount, taskData.reward);
                task.currentCount = taskData.currentCount;
                task.isCompleted = taskData.isCompleted;
                TaskManager.Instance.activeTasks.Add(task);
            }
        }
        
        // Handle human form unlock
        if (playerData.isHumanFormUnlocked && HumanTransformation.Instance != null)
        {
            // Unlock human transformation
        }
    }
    
    public bool AccountExists(string username)
    {
        string savePath = GetSavePath(username);
        return File.Exists(savePath);
    }
    
    private string GetSavePath(string username)
    {
        return Path.Combine(Application.persistentDataPath, username + ".sav");
    }
    
    public void DeleteAccount(string username)
    {
        string savePath = GetSavePath(username);
        
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
            Debug.Log("Account deleted: " + username);
        }
    }
    
    public string[] GetSavedAccounts()
    {
        string[] files = Directory.GetFiles(Application.persistentDataPath, "*.sav");
        
        for (int i = 0; i < files.Length; i++)
        {
            files[i] = Path.GetFileNameWithoutExtension(files[i]);
        }
        
        return files;
    }
    
    public void Logout()
    {
        if (autoSave)
        {
            SaveGame();
        }
        
        currentUsername = "";
        isAccountLoaded = false;
        Debug.Log("User logged out");
    }
}