
using UnityEngine;

public static class SaveSystem
{
    private const string LevelKey = "SavedLevel";

    // Check whether the player has saved progress
    public static bool HasSave()
    {
        return PlayerPrefs.HasKey(LevelKey);
    }

    // Save the next level to play
    public static void SaveLevel(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning("Cannot save an empty scene name.");
            return;
        }

        PlayerPrefs.SetString(LevelKey, sceneName);
        PlayerPrefs.Save();

        Debug.Log("Progress saved: " + sceneName);
    }

    // Get the player's saved level
    public static string GetSavedLevel()
    {
        return PlayerPrefs.GetString(LevelKey, "");
    }

    // Reset game progress
    public static void ResetProgress()
    {
        PlayerPrefs.DeleteKey(LevelKey);
        PlayerPrefs.Save();

        Debug.Log("Game progress reset.");
    }


    private const string TutorialReturnKey = "TutorialReturnScene";

    // Remember which scene opened the tutorial
    public static void SetTutorialReturnScene(string sceneName)
    {
        PlayerPrefs.SetString(TutorialReturnKey, sceneName);
        PlayerPrefs.Save();
    }

    // Check whether the tutorial was opened from another scene
    public static bool HasTutorialReturnScene()
    {
        return PlayerPrefs.HasKey(TutorialReturnKey);
    }

    // Retrieve the scene to return to
    public static string GetTutorialReturnScene()
    {
        return PlayerPrefs.GetString(TutorialReturnKey, "");
    }

    // Clear the temporary tutorial return destination
    public static void ClearTutorialReturnScene()
    {
        PlayerPrefs.DeleteKey(TutorialReturnKey);
        PlayerPrefs.Save();
    }

}
