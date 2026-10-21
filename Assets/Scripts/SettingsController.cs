using UnityEngine;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    [Header("Menu Panels")]
    public GameObject previousPanel;
    public GameObject settingsPanel;

    [Header("Audio Settings")]
    public Slider masterVolumeSlider;
    public Slider soundEffectsSlider;
    public Slider musicSlider;

    [Header("Display Settings")]
    public Slider brightnessSlider;

    private void Start()
    {

        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }

    public void OpenSettings()
    {

        if (previousPanel != null)
        {
            previousPanel.SetActive(false);
        }


        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
        }
    }

    public void CloseSettings()
    {

        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }


        if (previousPanel != null)
        {
            previousPanel.SetActive(true);
        }
    }
}
