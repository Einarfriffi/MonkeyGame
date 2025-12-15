using UnityEngine;
using UnityEngine.UI;

public class DeathPanel : MonoBehaviour
{
    public Button restartButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        restartButton.onClick.AddListener(() => GameManager.Instance.RestartLevel());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
