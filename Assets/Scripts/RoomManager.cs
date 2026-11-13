using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviour
{
    [Header("Room Setup")]
    public bool hasAnomaly = true;
    public bool randomizeAnomaly = false;
    public GameObject anomalyObject;
    public Transform roomStartPoint;

    [Header("Optional Multiple Anomalies")]
    public bool chooseRandomAnomalyFromList = false;
    public List<GameObject> anomalyOptions = new List<GameObject>();

    [Header("Progression")]
    public string nextSceneName;
    public float loadDelay = 0f;

    [Header("Events")]
    public UnityEvent onCorrectAnswer = new UnityEvent();
    public UnityEvent onWrongAnswer = new UnityEvent();

    private FPController player;
    private bool resolving;

    private void Awake()
    {
        player = FindFirstObjectByType<FPController>();
        if (randomizeAnomaly) hasAnomaly = Random.value > 0.5f;

        if (chooseRandomAnomalyFromList)
        {
            List<GameObject> valid = anomalyOptions.FindAll(item => item != null);
            if (valid.Count > 0)
                anomalyObject = valid[Random.Range(0, valid.Count)];
            else
                Debug.LogWarning($"{name}: No anomaly objects have been added to Anomaly Options.", this);
        }

        foreach (GameObject candidate in anomalyOptions)
        {
            if (candidate != null && candidate != anomalyObject)
                candidate.SetActive(false);
        }
        if (anomalyObject != null) anomalyObject.SetActive(hasAnomaly);
        if (player == null) Debug.LogWarning($"{name}: No FPController found.", this);
        if (roomStartPoint == null) Debug.LogWarning($"{name}: Assign Room Start Point to reset wrong answers.", this);
    }

    public void SubmitAnswer(bool choseAnomalyDoor)
    {
        if (resolving) return;
        if (choseAnomalyDoor == hasAnomaly)
        {
            resolving = true;
            onCorrectAnswer.Invoke();
            StartCoroutine(LoadNextRoom());
        }
        else
        {
            onWrongAnswer.Invoke();
            if (player != null && roomStartPoint != null)
                player.TeleportTo(roomStartPoint.position, roomStartPoint.rotation);
        }
    }

    private IEnumerator LoadNextRoom()
    {
        if (loadDelay > 0f) yield return new WaitForSecondsRealtime(loadDelay);
        string destination = nextSceneName;
        bool returning = SceneManager.GetActiveScene().name == "Tutorial Scene" && SaveSystem.HasTutorialReturnScene();
        if (returning) destination = SaveSystem.GetTutorialReturnScene();
        if (string.IsNullOrEmpty(destination) || !Application.CanStreamedLevelBeLoaded(destination))
        {
            Debug.LogError($"{name}: Destination '{destination}' not found in the build scene list.", this);
            resolving = false;
            yield break;
        }
        if (returning) SaveSystem.ClearTutorialReturnScene();
        else SaveSystem.SaveLevel(destination);
        SceneManager.LoadScene(destination);
    }
}
