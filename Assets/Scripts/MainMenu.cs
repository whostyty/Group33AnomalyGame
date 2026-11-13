using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Scenes")]
    public string tutorialSceneName = "Tutorial Scene";

    [Header("Menu Panels")]
    public GameObject mainMenuPanel;
    public GameObject settingsPanel;

    public void BeginGame()
    {
        SaveSystem.ClearTutorialReturnScene();
        string destination = tutorialSceneName;
        if (SaveSystem.HasSave() && Application.CanStreamedLevelBeLoaded(SaveSystem.GetSavedLevel()))
            destination = SaveSystem.GetSavedLevel();
        LoadScene(destination);
    }

    public void OpenTutorial()
    {
        if (SaveSystem.HasSave() && Application.CanStreamedLevelBeLoaded(SaveSystem.GetSavedLevel()))
            SaveSystem.SetTutorialReturnScene(SaveSystem.GetSavedLevel());
        else
            SaveSystem.ClearTutorialReturnScene();
        LoadScene(tutorialSceneName);
    }

    public void OpenSettings()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
    }

    public void ResetSave()
    {
        SaveSystem.ResetProgress();
        SaveSystem.ClearTutorialReturnScene();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void LoadScene(string destination)
    {
        if (string.IsNullOrEmpty(destination) || !Application.CanStreamedLevelBeLoaded(destination))
        {
            Debug.LogError($"Scene '{destination}' is not included in the Build Settings.", this);
            return;
        }
        Time.timeScale = 1f;
        SceneManager.LoadScene(destination);
    }
}
