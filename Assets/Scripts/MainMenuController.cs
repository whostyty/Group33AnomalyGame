
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private const string TutorialScene = "Tutorial Scene";

    // Begin Game: Continue progress or start tutorial
    public void BeginGame()
    {
        SaveSystem.ClearTutorialReturnScene();
        string sceneToLoad = TutorialScene;

        if (SaveSystem.HasSave())
        {
            string savedLevel = SaveSystem.GetSavedLevel();

            if (Application.CanStreamedLevelBeLoaded(savedLevel))
            {
                sceneToLoad = savedLevel;
            }
            else
            {
                Debug.LogWarning(
                    "Saved level not found: " + savedLevel +
                    ". Loading tutorial instead."
                );
            }
        }

        Debug.Log("Loading: " + sceneToLoad);
        SceneManager.LoadScene(sceneToLoad);
    }

    // Replay the tutorial manually

    public void OpenTutorial()
    {
        // Use the saved level as the return destination
        if (SaveSystem.HasSave())
        {
            string savedLevel = SaveSystem.GetSavedLevel();

            if (Application.CanStreamedLevelBeLoaded(savedLevel))
            {
                SaveSystem.SetTutorialReturnScene(savedLevel);
            }
            else
            {
                SaveSystem.ClearTutorialReturnScene();
            }
        }
        else
        {
            // First-time players follow normal tutorial progression
            SaveSystem.ClearTutorialReturnScene();
        }

        SceneManager.LoadScene("Tutorial Scene");
    }


    // Open settings
    public void OpenSettings()
    {
        SceneManager.LoadScene("Settings");
    }

    // Quit the game
    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }

    // Optional: Reset progress for testing
    public void ResetSave()
    {
        SaveSystem.ResetProgress();
    }
}
