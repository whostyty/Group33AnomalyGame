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

    [Header("Camera Settings")]
    public Slider sensitivitySlider;

    private void Awake()
    {
        HallPassBrightness.EnsureExists();

        if (brightnessSlider != null)
        {
            brightnessSlider.minValue = 0f;
            brightnessSlider.maxValue = 1f;
            brightnessSlider.wholeNumbers = false;
            brightnessSlider.SetValueWithoutNotify(HallPassSettings.BrightnessValue);
            brightnessSlider.onValueChanged.AddListener(ChangeBrightness);
        }

        if (sensitivitySlider != null)
        {
            sensitivitySlider.minValue = 0f;
            sensitivitySlider.maxValue = 1f;
            sensitivitySlider.wholeNumbers = false;
            sensitivitySlider.SetValueWithoutNotify(HallPassSettings.LookValue);
            sensitivitySlider.onValueChanged.AddListener(ChangeSensitivity);
        }
    }

    private void Start()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }

    private void OnDestroy()
    {
        if (brightnessSlider != null)
            brightnessSlider.onValueChanged.RemoveListener(ChangeBrightness);

        if (sensitivitySlider != null)
            sensitivitySlider.onValueChanged.RemoveListener(ChangeSensitivity);
    }

    public void ChangeBrightness(float value)
    {
        HallPassSettings.SetBrightnessValue(value);
        HallPassBrightness.Refresh();
    }

    public void ChangeSensitivity(float value)
    {
        HallPassSettings.SetLookValue(value);
        FPController player = FindFirstObjectByType<FPController>();
        if (player != null)
            player.RefreshLookSettings();
    }

    public void OpenSettings()
    {
        if (previousPanel != null)
            previousPanel.SetActive(false);

        if (settingsPanel != null)
            settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        if (previousPanel != null)
            previousPanel.SetActive(true);
    }
}
