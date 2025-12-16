using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class NameInputDialog : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField nameInput;
    public Button submitButton;
    
    [Header("Settings")]
    public int maxNameLength = 20;
    
    private Action<string> onNameSubmitted;
    
    void Start()
    {
        if (submitButton != null)
        {
            submitButton.onClick.AddListener(OnSubmitClicked);
        }
        
        if (nameInput != null)
        {
            nameInput.characterLimit = maxNameLength;
            nameInput.onSubmit.AddListener((value) => OnSubmitClicked());
        }
    }
    
    public void Show(Action<string> callback)
    {
        onNameSubmitted = callback;
        
        if (nameInput != null)
        {
            string savedName = PlayerPrefs.GetString("PlayerName", "");
            nameInput.text = savedName;
            nameInput.Select();
            nameInput.ActivateInputField();
            
            if (EventSystem.current != null)
            {
                EventSystem.current.SetSelectedGameObject(nameInput.gameObject);
            }
        }
    }
    
    private void OnSubmitClicked()
    {
        string playerName = nameInput.text.Trim();
        
        if (string.IsNullOrEmpty(playerName))
        {
            playerName = "Player";
        }
        
        playerName = SanitizeName(playerName);
        
        if (LootLockerManager.Instance != null)
        {
            LootLockerManager.Instance.SetPlayerName(playerName, (success) =>
            {
                if (success)
                {
                    onNameSubmitted?.Invoke(playerName);
                }
            });
        }
        else
        {
            onNameSubmitted?.Invoke(playerName);
        }
    }
    
    private string SanitizeName(string input)
    {
        input = input.Trim();
        if (input.Length > maxNameLength)
        {
            input = input.Substring(0, maxNameLength);
        }
        return input;
    }
}
