using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsMenuManager : MonoBehaviour
{
    public void ChangeGraphicsQuality(int index)
    {
        QualitySettings.SetQualityLevel(index);
    }
}
