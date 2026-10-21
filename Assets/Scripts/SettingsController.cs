
using UnityEngine;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    [Header("Menu Panels")]
    [SerializeField] private GameObject previousPanel;
    [SerializeField] private GameObject settingsPanel;

    [Header("Audio Settings")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider soundEffectsSlider;
    [SerializeField] private Slider musicSlider;

    [Header("Display Settings")]
    [SerializeField] private Slider brightnessSlider;

    private void Start()
    {
        // Settings should be hidden when the scene starts.
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }

    public void OpenSettings()
    {
        // Hide the menu we came from.
        if (previousPanel != null)
        {
            previousPanel.SetActive(false);
        }

        // Show settings.
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
        }
    }

    public void CloseSettings()
    {
        // Hide settings.
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }

        // Return to the previous menu.
        if (previousPanel != null)
        {
            previousPanel.SetActive(true);
        }
    }
}
