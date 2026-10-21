using UnityEngine;

public static class SaveSystem
{
    private const string LevelKey = "SavedLevel";


    public static bool HasSave()
    {
        return PlayerPrefs.HasKey(LevelKey);
    }


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


    public static string GetSavedLevel()
    {
        return PlayerPrefs.GetString(LevelKey, "");
    }


    public static void ResetProgress()
    {
        PlayerPrefs.DeleteKey(LevelKey);
        PlayerPrefs.Save();

        Debug.Log("Game progress reset.");
    }


    private const string TutorialReturnKey = "TutorialReturnScene";


    public static void SetTutorialReturnScene(string sceneName)
    {
        PlayerPrefs.SetString(TutorialReturnKey, sceneName);
        PlayerPrefs.Save();
    }


    public static bool HasTutorialReturnScene()
    {
        return PlayerPrefs.HasKey(TutorialReturnKey);
    }


    public static string GetTutorialReturnScene()
    {
        return PlayerPrefs.GetString(TutorialReturnKey, "");
    }


    public static void ClearTutorialReturnScene()
    {
        PlayerPrefs.DeleteKey(TutorialReturnKey);
        PlayerPrefs.Save();
    }

}
