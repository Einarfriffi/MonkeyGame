using System.Collections;
using UnityEngine;
using TMPro;

public class LevelIntro : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI countdownText;

    [Header("Settings")]
    public float countdownTime = 3f;

    [Header("Gameplay Elements")]
    public GameObject[] objectsToEnableAfterCountdown;

    void Start()
    {
        if (countdownText == null)
        {
            countdownText = GetComponentInChildren<TextMeshProUGUI>();
        }

        if (GameManager.Instance != null && GameManager.Instance.playerInput != null)
        {
            GameManager.Instance.playerInput.DeactivateInput();
        }

        var playerMovement = Object.FindFirstObjectByType<PlayerMovement>(FindObjectsInactive.Include);
        if (playerMovement != null)
        {
            playerMovement.canAim = false;
        }

        StartCoroutine(CountdownAndStart());
    }

    IEnumerator CountdownAndStart()
    {
        float currentTime = countdownTime;

        while (currentTime > 0)
        {
            countdownText.text = Mathf.Ceil(currentTime).ToString();
            yield return new WaitForSecondsRealtime(1f);
            currentTime--;
        }

        countdownText.text = "GO!";
        yield return new WaitForSecondsRealtime(0.5f);

        countdownText.gameObject.SetActive(false);

        foreach (GameObject obj in objectsToEnableAfterCountdown)
        {
            obj.SetActive(true);
        }

        GameManager.Instance.StartGame();

        if (GameManager.Instance != null && GameManager.Instance.playerInput != null)
        {
            GameManager.Instance.playerInput.ActivateInput();
        }

        var playerMovement = Object.FindFirstObjectByType<PlayerMovement>(FindObjectsInactive.Include);
        if (playerMovement != null)
        {
            playerMovement.canAim = true;
        }

        var pauseMenu = Object.FindFirstObjectByType<PauseMenu>(FindObjectsInactive.Include);
        if (pauseMenu != null)
        {
            pauseMenu.EnablePausing();
        }

        Object.FindFirstObjectByType<levelHUD>()?.StartTimer();
    }
}
