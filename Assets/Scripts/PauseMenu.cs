
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject settingsPanel;

    [Header("Scene Names")]
    [SerializeField] private string mainMenuScene = "Main Menu";
    [SerializeField] private string tutorialScene = "Tutorial Scene";

    private bool isPaused = false;

    private FPController playerController;

    private void Start()
    {
        playerController = FindFirstObjectByType<FPController>();

        // Make sure gameplay starts unpaused
        Time.timeScale = 1f;
        isPaused = false;

        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (!isPaused)
            {
                PauseGame();
            }
            else if (settingsPanel != null &&
                     settingsPanel.activeSelf)
            {
                BackToPause();
            }
            else
            {
                ResumeGame();
            }
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;

        if (pausePanel != null)
            pausePanel.SetActive(true);

        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        if (playerController != null)
            playerController.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;

        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        if (playerController != null)
            playerController.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OpenSettings()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (settingsPanel != null)
            settingsPanel.SetActive(true);
    }

    public void BackToPause()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        if (pausePanel != null)
            pausePanel.SetActive(true);
    }


    public void OpenTutorial()
    {
        // Remember which scene the player came from
        string currentScene = SceneManager.GetActiveScene().name;

        SaveSystem.SetTutorialReturnScene(currentScene);

        // Restore normal time before changing scenes
        PrepareForSceneChange();

        // Open the tutorial
        SceneManager.LoadScene(tutorialScene);
    }


    public void QuitToMainMenu()
    {
        // Save the current gameplay scene
        SaveSystem.ClearTutorialReturnScene();
        string currentScene =
            SceneManager.GetActiveScene().name;

        // Do not overwrite progress with the tutorial
        if (currentScene != tutorialScene &&
            currentScene != mainMenuScene)
        {
            SaveSystem.SaveLevel(currentScene);
        }

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
