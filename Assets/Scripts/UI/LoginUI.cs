using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class LoginUI : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject loginPanel;
    public InputField usernameInput;
    public Button loginButton;
    public Button createButton;
    public Text statusText;
    public Text savedAccountsText;
    
    [Header("Game Manager")]
    public GameManager gameManager;
    
    private List<string> savedAccounts = new List<string>();
    
    void Start()
    {
        InitializeLoginUI();
    }
    
    private void InitializeLoginUI()
    {
        if (loginPanel != null) loginPanel.SetActive(true);
        
        if (usernameInput != null)
        {
            usernameInput.onEndEdit.AddListener(OnUsernameInputEndEdit);
        }
        
        if (loginButton != null)
        {
            loginButton.onClick.AddListener(OnLoginButtonClicked);
        }
        
        if (createButton != null)
        {
            createButton.onClick.AddListener(OnCreateButtonClicked);
        }
        
        RefreshSavedAccountsList();
    }
    
    private void OnUsernameInputEndEdit(string input)
    {
        ValidateInput();
    }
    
    private void OnLoginButtonClicked()
    {
        string username = usernameInput.text.Trim();
        
        if (string.IsNullOrEmpty(username))
        {
            UpdateStatus("Please enter a username");
            return;
        }
        
        if (!UserAccount.Instance.AccountExists(username))
        {
            UpdateStatus("Account does not exist: " + username);
            return;
        }
        
        if (UserAccount.Instance.LoadAccount(username))
        {
            UpdateStatus("Welcome back, " + username + "!");
            loginPanel.SetActive(false);
            
            // Start the game
            StartGame();
        }
        else
        {
            UpdateStatus("Failed to load account");
        }
    }
    
    private void OnCreateButtonClicked()
    {
        string username = usernameInput.text.Trim();
        
        if (string.IsNullOrEmpty(username))
        {
            UpdateStatus("Please enter a username");
            return;
        }
        
        if (username.Length < 3)
        {
            UpdateStatus("Username must be at least 3 characters");
            return;
        }
        
        if (UserAccount.Instance.AccountExists(username))
        {
            UpdateStatus("Account already exists: " + username);
            return;
        }
        
        if (UserAccount.Instance.CreateAccount(username))
        {
            UpdateStatus("Account created for: " + username);
            loginPanel.SetActive(false);
            
            // Start the game
            StartGame();
        }
        else
        {
            UpdateStatus("Failed to create account");
        }
        
        RefreshSavedAccountsList();
    }
    
    private void ValidateInput()
    {
        if (usernameInput != null)
        {
            string input = usernameInput.text;
            
            // Remove invalid characters
            string validInput = "";
            foreach (char c in input)
            {
                if (System.Char.IsLetterOrDigit(c) || c == '_' || c == ' ')
                {
                    validInput += c;
                }
            }
            
            if (input != validInput)
            {
                usernameInput.text = validInput;
                usernameInput.caretPosition = validInput.Length;
            }
        }
    }
    
    private void UpdateStatus(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
        }
        
        Debug.Log(message);
    }
    
    private void StartGame()
    {
        // Hide the login UI and start the game
        if (loginPanel != null)
        {
            loginPanel.SetActive(false);
        }
        
        // Enable player controls
        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            playerController.enabled = true;
        }
        
        // Start game systems
        if (TaskManager.Instance != null)
        {
            // Task manager is already initialized
        }
        
        if (FarmEnvironment.Instance != null)
        {
            // Farm environment is already initialized
        }
    }
    
    public void RefreshSavedAccountsList()
    {
        if (savedAccountsText != null && UserAccount.Instance != null)
        {
            string[] accounts = UserAccount.Instance.GetSavedAccounts();
            
            string accountsList = "Saved Accounts:\n";
            if (accounts.Length > 0)
            {
                foreach (string account in accounts)
                {
                    accountsList += "- " + account + "\n";
                }
            }
            else
            {
                accountsList += "No saved accounts";
            }
            
            savedAccountsText.text = accountsList;
        }
    }
    
    void Update()
    {
        // Check if we need to show login panel again (e.g., if player logs out)
        if (UserAccount.Instance != null && !UserAccount.Instance.isAccountLoaded && loginPanel != null)
        {
            loginPanel.SetActive(true);
        }
    }
}