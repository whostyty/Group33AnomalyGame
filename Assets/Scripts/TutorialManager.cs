using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public enum StepGoal { PressForward, LookAround, SpotAnomaly, UseDoor, Timed, OpenDoor }

    [System.Serializable]
    public class TutorialStep
    {
        [TextArea(2, 5)] public string message;
        public string prompt;
        public StepGoal goal;
        public float minDisplayTime = 1.5f;
        public bool showDoorLabels;
    }

    [Header("References (Auto-Find If Empty)")]
    public FPController player;
    public AnomalyObserver anomalyObserver;
    public RoomManager room;
    public InteractiveDoor tutorialDoor;

    [Header("Tutorial UI")]
    public GameObject panel;
    public TMP_Text messageText;
    public TMP_Text promptText;
    public float popDuration = 0.25f;
    public GameObject[] doorLabels;

    [Header("Tutorial Settings")]
    public float lookThreshold = 150f;
    public float feedbackSeconds = 4f;

    [TextArea(2, 5)] public string correctMessage = "Correct! Continue through the door. Remember: choose ANOMALY when something changes, otherwise choose NO CHANGE.";
    [TextArea(2, 5)] public string wrongMessage = "Wrong exit! Look again and try the other door.";

    [Header("Editable Tutorial Steps")]
    public List<TutorialStep> steps = new List<TutorialStep>
    {
        new TutorialStep { message = "Welcome to Hall Pass. Examine rooms and notice anything that changes.", prompt = "Press W to start", goal = StepGoal.PressForward, minDisplayTime = 2f },
        new TutorialStep { message = "Move with W, A, S and D. Move your mouse to look around.", prompt = "Move the mouse", goal = StepGoal.LookAround },
        new TutorialStep { message = "Walk up to the hallway door, look at it and press E to open it.", prompt = "Look at the door and press E", goal = StepGoal.OpenDoor, minDisplayTime = 0f },
        new TutorialStep { message = "Find the object that looks different. This tutorial contains an anomaly.", prompt = "Find the anomaly", goal = StepGoal.SpotAnomaly, minDisplayTime = 1f },
        new TutorialStep { message = "An anomaly is present. Look at the exit labels before choosing.", prompt = "Read both exit labels", goal = StepGoal.Timed, minDisplayTime = 6f, showDoorLabels = true },
        new TutorialStep { message = "Open the ANOMALY exit with E, then walk through it. If nothing changed, use NO CHANGE instead.", prompt = "Open the ANOMALY exit", goal = StepGoal.UseDoor, minDisplayTime = 0f, showDoorLabels = true }
    };

    private int index = -1;
    private float stepTimer;
    private float lookAccumulated;
    private bool goalReached;
    private bool finished;
    private CanvasGroup canvasGroup;
    private Coroutine popRoutine;
    private Coroutine feedbackRoutine;

    private void Awake()
    {
        if (player == null) player = FindFirstObjectByType<FPController>();
        if (anomalyObserver == null) anomalyObserver = FindFirstObjectByType<AnomalyObserver>();
        if (room == null) room = FindFirstObjectByType<RoomManager>();
        if (doorLabels == null || doorLabels.Length == 0)
        {
            DoorLabel[] labels = FindObjectsByType<DoorLabel>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            System.Collections.Generic.List<GameObject> choices = new System.Collections.Generic.List<GameObject>();
            foreach (DoorLabel label in labels)
                if (label.gameObject.scene == gameObject.scene)
                    choices.Add(label.gameObject);
            doorLabels = choices.ToArray();
        }
        if (panel != null)
        {
            canvasGroup = panel.GetComponent<CanvasGroup>();
            if (canvasGroup == null) canvasGroup = panel.AddComponent<CanvasGroup>();
        }
        else Debug.LogError($"{name}: No Tutorial Panel assigned.", this);
    }

    private void OnEnable()
    {
        if (room != null)
        {
            room.onCorrectAnswer.AddListener(HandleCorrect);
            room.onWrongAnswer.AddListener(HandleWrong);
        }
    }

    private void OnDisable()
    {
        if (room != null)
        {
            room.onCorrectAnswer.RemoveListener(HandleCorrect);
            room.onWrongAnswer.RemoveListener(HandleWrong);
        }
    }

    private void Start() { GoToStep(0); }

    private void Update()
    {
        if (finished || player == null || feedbackRoutine != null || index < 0 || index >= steps.Count) return;
        TutorialStep step = steps[index];
        stepTimer += Time.deltaTime;
        if (!goalReached) goalReached = CheckGoal(step);
        if (goalReached && stepTimer >= step.minDisplayTime) GoToStep(index + 1);
    }

    private bool CheckGoal(TutorialStep step)
    {
        switch (step.goal)
        {
            case StepGoal.PressForward: return player.MoveInput.y > 0.5f;
            case StepGoal.LookAround:
                lookAccumulated += player.LookInput.magnitude;
                return lookAccumulated >= lookThreshold;
            case StepGoal.SpotAnomaly: return anomalyObserver != null && anomalyObserver.HasSpotted;
            case StepGoal.OpenDoor: return tutorialDoor != null && tutorialDoor.HasBeenOpened;
            case StepGoal.Timed: return true;
            default: return false;
        }
    }

    private void GoToStep(int newIndex)
    {
        index = newIndex;
        stepTimer = 0f;
        lookAccumulated = 0f;
        goalReached = false;
        if (index >= steps.Count)
        {
            finished = true;
            if (panel != null) panel.SetActive(false);
            return;
        }
        UpdateDoorLabels(steps[index].showDoorLabels);
        ShowPanel(steps[index].message, steps[index].prompt);
    }

    private void UpdateDoorLabels(bool visible)
    {
        if (doorLabels == null) return;
        foreach (GameObject label in doorLabels)
            if (label != null) label.SetActive(visible);
    }

    private void HandleCorrect()
    {
        finished = true;
        if (feedbackRoutine != null) { StopCoroutine(feedbackRoutine); feedbackRoutine = null; }
        ShowPanel(correctMessage, "");
    }

    private void HandleWrong()
    {
        if (finished) return;
        if (feedbackRoutine != null) StopCoroutine(feedbackRoutine);
        feedbackRoutine = StartCoroutine(WrongFeedback());
    }

    private IEnumerator WrongFeedback()
    {
        ShowPanel(wrongMessage, "");
        yield return new WaitForSecondsRealtime(feedbackSeconds);
        feedbackRoutine = null;
        if (!finished && index >= 0 && index < steps.Count)
            ShowPanel(steps[index].message, steps[index].prompt);
    }

    private void ShowPanel(string message, string prompt)
    {
        if (panel == null) return;
        if (messageText != null) messageText.text = message;
        if (promptText != null)
        {
            promptText.text = prompt;
            promptText.gameObject.SetActive(!string.IsNullOrEmpty(prompt));
        }
        panel.SetActive(true);
        if (popRoutine != null) StopCoroutine(popRoutine);
        popRoutine = StartCoroutine(PopIn());
    }

    private IEnumerator PopIn()
    {
        if (canvasGroup == null) yield break;
        Transform t = panel.transform;
        Vector3 startScale = Vector3.one * 0.9f;
        float duration = Mathf.Max(0.001f, popDuration);
        float elapsed = 0f;
        canvasGroup.alpha = 0f;
        t.localScale = startScale;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(elapsed / duration);
            k = 1f - Mathf.Pow(1f - k, 3f);
            canvasGroup.alpha = k;
            t.localScale = Vector3.Lerp(startScale, Vector3.one, k);
            yield return null;
        }
        canvasGroup.alpha = 1f;
        t.localScale = Vector3.one;
    }
}
