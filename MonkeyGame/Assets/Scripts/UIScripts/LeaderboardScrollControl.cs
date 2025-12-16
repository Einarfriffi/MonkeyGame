using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class LeaderboardScrollControl : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [Header("References")]
    public ScrollRect scrollRect;
    public Button defaultReturnButton;
    
    [Header("Scroll Settings")]
    public float keyboardScrollSpeed = 0.05f;
    public float gamepadScrollSpeed = 0.05f;
    
    private bool isSelected = false;
    private GameObject lastSelectedButton;
    
    void Update()
    {
        if (!isSelected || scrollRect == null) return;
        
        float scrollDelta = 0f;
        bool navigateBack = false;
        
        if (Keyboard.current != null)
        {
            if (Keyboard.current.upArrowKey.isPressed || Keyboard.current.wKey.isPressed)
            {
                scrollDelta = keyboardScrollSpeed;
            }
            else if (Keyboard.current.downArrowKey.isPressed || Keyboard.current.sKey.isPressed)
            {
                scrollDelta = -keyboardScrollSpeed;
            }
            
            if (Keyboard.current.leftArrowKey.wasPressedThisFrame || Keyboard.current.aKey.wasPressedThisFrame)
            {
                navigateBack = true;
            }
        }
        
        if (Gamepad.current != null)
        {
            Vector2 leftStick = Gamepad.current.leftStick.ReadValue();
            Vector2 dpad = Gamepad.current.dpad.ReadValue();
            
            float vertical = Mathf.Abs(dpad.y) > 0.1f ? dpad.y : leftStick.y;
            float horizontal = Mathf.Abs(dpad.x) > 0.1f ? dpad.x : leftStick.x;
            
            if (Mathf.Abs(vertical) > 0.1f)
            {
                scrollDelta = vertical * gamepadScrollSpeed;
            }
            
            if (horizontal < -0.5f)
            {
                navigateBack = true;
            }
        }
        
        if (navigateBack)
        {
            NavigateBackToLastButton();
        }
        else if (scrollDelta != 0f)
        {
            scrollRect.verticalNormalizedPosition = Mathf.Clamp01(
                scrollRect.verticalNormalizedPosition + scrollDelta
            );
        }
    }
    
    void LateUpdate()
    {
        if (EventSystem.current != null && !isSelected)
        {
            GameObject currentSelected = EventSystem.current.currentSelectedGameObject;
            if (currentSelected != null && currentSelected != gameObject && 
                currentSelected.GetComponent<Button>() != null)
            {
                lastSelectedButton = currentSelected;
            }
        }
    }
    
    public void OnSelect(BaseEventData eventData)
    {
        isSelected = true;
    }
    
    public void OnDeselect(BaseEventData eventData)
    {
        isSelected = false;
    }
    
    private void NavigateBackToLastButton()
    {
        if (EventSystem.current == null) return;
        
        GameObject targetButton = lastSelectedButton;
        
        if (targetButton == null || !targetButton.activeInHierarchy)
        {
            targetButton = defaultReturnButton != null ? defaultReturnButton.gameObject : null;
        }
        
        if (targetButton != null)
        {
            EventSystem.current.SetSelectedGameObject(targetButton);
        }
    }
}
