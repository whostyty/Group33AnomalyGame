using System.Collections;
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
        if (player == null)
            Debug.LogError($"{name}: RoomManager could not find an FPController in the scene.", this);

        if (roomStartPoint == null)
            Debug.LogWarning($"{name}: RoomManager has no Room Start Point, so wrong answers cannot reset the player.", this);

        if (randomizeAnomaly)
            hasAnomaly = Random.value > 0.5f;

        if (anomalyObject != null)
            anomalyObject.SetActive(hasAnomaly);
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
            SendPlayerToRoomStart();
        }
    }

    private void SendPlayerToRoomStart()
    {
        if (player == null || roomStartPoint == null) return;
        player.TeleportTo(roomStartPoint.position, roomStartPoint.rotation);
    }


    private IEnumerator LoadNextRoom()
    {
        if (loadDelay > 0f)
            yield return new WaitForSecondsRealtime(loadDelay);

        string destination = nextSceneName;


        bool returningFromTutorial =
            SceneManager.GetActiveScene().name == "Tutorial Scene" &&
            SaveSystem.HasTutorialReturnScene();

        if (returningFromTutorial)
        {
            destination = SaveSystem.GetTutorialReturnScene();
        }

        if (string.IsNullOrEmpty(destination))
        {
            Debug.LogError(
                $"{name}: No destination scene has been assigned.",
                this
            );

            resolving = false;
            yield break;
        }

        if (!Application.CanStreamedLevelBeLoaded(destination))
        {
            Debug.LogError(
                $"{name}: Scene '{destination}' is not in the build scene list.",
                this
            );

            resolving = false;
            yield break;
        }

        if (returningFromTutorial)
        {

            SaveSystem.ClearTutorialReturnScene();

            Debug.Log("Returning from tutorial to: " + destination);
        }
        else
        {

            SaveSystem.SaveLevel(destination);

            Debug.Log("Progress saved: " + destination);
        }

        SceneManager.LoadScene(destination);
    }


}