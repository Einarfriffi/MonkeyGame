using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class SettingsMenuManager : MonoBehaviour
{

    public TMP_Dropdown graphicsDropdown;

    public void ChangeGraphicsQuality()
    {
        QualitySettings.SetQualityLevel(graphicsDropdown.value);
    }

}
