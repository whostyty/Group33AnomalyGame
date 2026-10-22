using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public enum StepGoal { PressForward, LookAround, SpotAnomaly, UseDoor, Timed }

    [System.Serializable]
    public class TutorialStep
    {
        [TextArea(2, 5)] public string message;
        public string prompt;
        public StepGoal goal;
        public float minDisplayTime = 1.5f;
    }

    [Header("References (auto-found if empty)")]
    public FPController player;
    public AnomalyObserver anomalyObserver;
    public RoomManager room;

    [Header("UI")]
    public GameObject panel;
    public TMP_Text messageText;
    public TMP_Text promptText;
    public float popDuration = 0.25f;

    [Header("Tutorial Settings")]
    public float lookThreshold = 150f;
    public float feedbackSeconds = 4f;

    [TextArea(2, 4)]
    public string correctMessage = "Correct! That door leads to the next level.\nFrom here on, nothing will tell you whether a room has an anomaly. It is up to you to spot it.";

    [TextArea(2, 4)]
    public string wrongMessage = "Wrong door! In the real game, choosing wrongly sends you back to the start of the room, just like now.\nLook again and try the other door.";

    public List<TutorialStep> steps = new List<TutorialStep>
    {
        new TutorialStep
        {
            message = "Welcome to Hall Pass.\nIn this game you need to go through all the rooms and detect if anything changes.",
            prompt = "Press W to start",
            goal = StepGoal.PressForward,
            minDisplayTime = 2f
        },
        new TutorialStep
        {
            message = "Good! Use W, A, S and D to walk around the room.\nNow move your mouse to look around.",
            prompt = "Move the mouse to look around",
            goal = StepGoal.LookAround,
            minDisplayTime = 1.5f
        },
        new TutorialStep
        {
            message = "Now that you have learnt how to move and look around, try looking around for something that looks different.\nThere IS an anomaly in this room. In the real game, nothing will tell you.",
            prompt = "Find what has changed",
            goal = StepGoal.SpotAnomaly,
            minDisplayTime = 1f
        },
        new TutorialStep
        {
            message = "You found an anomaly!\nWhen a room has an anomaly, walk through the ANOMALY door. If nothing has changed, take the NO CHANGE door.",
            prompt = "Walk through the anomaly door",
            goal = StepGoal.UseDoor,
            minDisplayTime = 0f
        }
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

        if (panel != null)
        {
            canvasGroup = panel.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = panel.AddComponent<CanvasGroup>();
        }
        else
        {
            Debug.LogError($"{name}: TutorialManager has no Panel assigned.", this);
        }
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

    private void Start()
    {
        GoToStep(0);
    }

    private void Update()
    {
        if (finished || player == null || feedbackRoutine != null) return;
        if (index < 0 || index >= steps.Count) return;

        TutorialStep step = steps[index];
        stepTimer += Time.deltaTime;

        if (!goalReached)
            goalReached = CheckGoal(step);

        if (goalReached && stepTimer >= step.minDisplayTime)
            GoToStep(index + 1);
    }

    private bool CheckGoal(TutorialStep step)
    {
        switch (step.goal)
        {
            case StepGoal.PressForward:
                return player.MoveInput.y > 0.5f;

            case StepGoal.LookAround:
                lookAccumulated += player.LookInput.magnitude;
                return lookAccumulated >= lookThreshold;

            case StepGoal.SpotAnomaly:
                return anomalyObserver != null && anomalyObserver.HasSpotted;

            case StepGoal.Timed:
                return true;

            default:
                return false;
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

        ShowPanel(steps[index].message, steps[index].prompt);
    }

    private void HandleCorrect()
    {
        finished = true;
        if (feedbackRoutine != null)
        {
            StopCoroutine(feedbackRoutine);
            feedbackRoutine = null;
        }
        ShowPanel(correctMessage, "");
    }

    private void HandleWrong()
    {
        if (finished) return;
        if (feedbackRoutine != null)
            StopCoroutine(feedbackRoutine);
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

        if (messageText != null)
            messageText.text = message;

        if (promptText != null)
        {
            promptText.text = prompt;
            promptText.gameObject.SetActive(!string.IsNullOrEmpty(prompt));
        }

        panel.SetActive(true);

        if (popRoutine != null)
            StopCoroutine(popRoutine);
        popRoutine = StartCoroutine(PopIn());
    }

    private IEnumerator PopIn()
    {
        Transform t = panel.transform;
        Vector3 startScale = Vector3.one * 0.9f;
        float elapsed = 0f;

        canvasGroup.alpha = 0f;
        t.localScale = startScale;

        while (elapsed < popDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(elapsed / popDuration);
            k = 1f - Mathf.Pow(1f - k, 3f);
            canvasGroup.alpha = k;
            t.localScale = Vector3.Lerp(startScale, Vector3.one, k);
            yield return null;
        }

        canvasGroup.alpha = 1f;
        t.localScale = Vector3.one;
    }
}