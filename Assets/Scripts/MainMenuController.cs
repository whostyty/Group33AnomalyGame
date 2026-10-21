using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Scene Names")]
    public string tutorialSceneName = "Tutorial Scene";

    [Header("Menu Panels")]
    public GameObject mainMenuPanel;
    public GameObject settingsPanel;

    private void Start()
    {

        Time.timeScale = 1f;


        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);

        if (settingsPanel != null)
            settingsPanel.SetActive(false);


        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }


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

            SceneManager.LoadScene(tutorialSceneName);
        }
    }


    public void OpenTutorial()
    {
        string savedScene = SaveSystem.GetSavedLevel();

        if (!string.IsNullOrEmpty(savedScene) &&
            Application.CanStreamedLevelBeLoaded(savedScene))
        {

            SaveSystem.SetTutorialReturnScene(savedScene);
        }
        else
        {
            SaveSystem.ClearTutorialReturnScene();
        }

        SceneManager.LoadScene(tutorialSceneName);
    }


    public void OpenSettings()
    {
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);

        if (settingsPanel != null)
            settingsPanel.SetActive(true);
    }


    public void CloseSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);
    }


    public void ResetSave()
    {
        SaveSystem.ResetProgress();
        SaveSystem.ClearTutorialReturnScene();

        Debug.Log("Game progress has been reset.");
    }


    public void QuitGame()
    {
        Debug.Log("Quitting Hall Pass...");
        Application.Quit();
    }
}
