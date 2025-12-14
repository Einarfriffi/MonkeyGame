using UnityEngine;
using UnityEngine.EventSystems;

public class DisableGameplayOnHover : MonoBehaviour
{
    public void OnPointerEnter(PointerEventData _) => GameManager.Instance?.playerInput?.DeactivateInput();
    public void OnPointerExit(PointerEventData _) => GameManager.Instance?.playerInput?.ActivateInput();
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
