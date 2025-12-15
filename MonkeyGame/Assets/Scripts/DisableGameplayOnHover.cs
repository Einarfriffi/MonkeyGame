using UnityEngine;
using UnityEngine.EventSystems;

public class DisableGameplayOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (GameManager.Instance != null && GameManager.Instance.playerInput != null)
        {
            GameManager.Instance.playerInput.DeactivateInput();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (GameManager.Instance != null && GameManager.Instance.playerInput != null)
        {
            GameManager.Instance.playerInput.ActivateInput();
        }
    }
}
