
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Scene Names")]
    [SerializeField] private string tutorialSceneName = "Tutorial Scene";

    [Header("Menu Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject settingsPanel;

    private void Start()
    {
        // Ensure the game isn't paused when entering the menu
        Time.timeScale = 1f;

        // Show main menu and hide settings
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);

        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        // Make cursor visible and unlocked
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // PLAY BUTTON
    public void BeginGame()
    {
        SaveSystem.ClearTutorialReturnScene();

        string savedScene = SaveSystem.GetSavedLevel();

        if (!string.IsNullOrEmpty(savedScene) &&
            Application.CanStreamedLevelBeLoaded(savedScene))
        {
            SceneManager.LoadScene(savedScene);
        }
        else
        {
            // New players begin with the tutorial
            SceneManager.LoadScene(tutorialSceneName);
        }
    }

    // TUTORIAL BUTTON
    public void OpenTutorial()
    {
        string savedScene = SaveSystem.GetSavedLevel();

        if (!string.IsNullOrEmpty(savedScene) &&
            Application.CanStreamedLevelBeLoaded(savedScene))
        {
            // Return to saved progress after replaying tutorial
            SaveSystem.SetTutorialReturnScene(savedScene);
        }
        else
        {
            SaveSystem.ClearTutorialReturnScene();
        }

        SceneManager.LoadScene(tutorialSceneName);
    }

    // SETTINGS BUTTON
    public void OpenSettings()
    {
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);

        if (settingsPanel != null)
            settingsPanel.SetActive(true);
    }

    // BACK BUTTON IN SETTINGS
    public void CloseSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);
    }

    // OPTIONAL RESET PROGRESS BUTTON
    public void ResetSave()
    {
        SaveSystem.ResetProgress();
        SaveSystem.ClearTutorialReturnScene();

        Debug.Log("Game progress has been reset.");
    }

    // QUIT BUTTON
    public void QuitGame()
    {
        Debug.Log("Quitting Hall Pass...");
        Application.Quit();
    }
}
