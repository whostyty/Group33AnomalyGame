using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject pausePanel;
    public GameObject settingsPanel;

    [Header("Scenes")]
    public string mainMenuScene = "MainMenu";
    public string tutorialScene = "Tutorial Scene";

    [Header("Pause Input")]
    public bool allowEscape = true;
    public bool allowGamepadStart = true;

    private bool isPaused;
    private FPController playerController;
    public bool IsPaused => isPaused;

    private void Start()
    {
        playerController = FindFirstObjectByType<FPController>();
        Time.timeScale = 1f;
        isPaused = false;
        if (pausePanel != null) pausePanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }

    private void Update()
    {
        bool escape = allowEscape && Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame;
        bool gamepad = allowGamepadStart && Gamepad.current != null && Gamepad.current.startButton.wasPressedThisFrame;
        if (!escape && !gamepad) return;

        if (!isPaused) PauseGame();
        else if (settingsPanel != null && settingsPanel.activeSelf) BackToPause();
        else ResumeGame();
    }

    public void PauseGame()
    {
        if (isPaused) return;
        isPaused = true;
        Time.timeScale = 0f;
        if (pausePanel != null) pausePanel.SetActive(true);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (playerController != null) playerController.enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        if (pausePanel != null) pausePanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (playerController != null) playerController.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OpenSettings()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(true);
    }

    public void BackToPause()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(true);
    }

    public void OpenTutorial()
    {
        SaveSystem.SetTutorialReturnScene(SceneManager.GetActiveScene().name);
        PrepareForSceneChange();
        SceneManager.LoadScene(tutorialScene);
    }

    public void QuitToMainMenu()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        SaveSystem.ClearTutorialReturnScene();
        if (sceneName != mainMenuScene && sceneName != tutorialScene)
            SaveSystem.SaveLevel(sceneName);
        PrepareForSceneChange();
        SceneManager.LoadScene(mainMenuScene);
    }

    private void PrepareForSceneChange()
    {
        Time.timeScale = 1f;
        isPaused = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
