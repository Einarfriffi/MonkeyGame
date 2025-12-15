using UnityEngine;

public class SettingsMenuManager : MonoBehaviour
{
    public void ChangeGraphicsQuality(int index)
    {
        QualitySettings.SetQualityLevel(index, true);
        Debug.Log("Switched to quality: " + QualitySettings.names[index]);
    }
}

