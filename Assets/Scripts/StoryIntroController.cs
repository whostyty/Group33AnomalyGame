using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class StoryIntroController : MonoBehaviour
{
    [Header("Story UI")]
    public GameObject storyPanel;
    public TMP_Text storyText;
    public RectTransform textViewport;
    public Button continueButton;
    public GameObject skipText;

    [Header("Your Backstory")]
    [TextArea(5, 20)]
    public string backstory =
        "Enter your backstory here.";

    [Header("Typewriter Settings")]
    public float typingSpeed = 0.04f;

    [Header("Automatic Scrolling")]
    [Range(0.4f, 0.95f)]
    public float scrollTriggerPoint = 0.75f;

    public float scrollDuration = 0.4f;
    public float bottomPadding = 30f;

    [Header("Tutorial References")]
    public TutorialManager tutorialManager;
    public FPController playerController;
    public AnomalyObserver anomalyObserver;

    private RectTransform textRect;

    private Coroutine typingCoroutine;
    private Coroutine scrollingCoroutine;

    private bool textFinished;
    private bool introActive = true;

    private float currentScrollOffset;

    private void Start()
    {

        if (tutorialManager != null)
            tutorialManager.enabled = false;

        if (anomalyObserver != null)
            anomalyObserver.enabled = false;


        if (playerController != null)
            playerController.enabled = false;


        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;


        if (storyPanel != null)
            storyPanel.SetActive(true);


        if (skipText != null)
            skipText.SetActive(true);


        if (continueButton != null)
        {
            continueButton.gameObject.SetActive(false);
            continueButton.onClick.AddListener(ContinueTutorial);
        }

        if (storyText == null || textViewport == null)
        {
            Debug.LogError(
                "StoryText or TextViewport is not assigned!"
            );
            return;
        }

        textRect = storyText.rectTransform;


        textRect.anchorMin = new Vector2(0f, 1f);
        textRect.anchorMax = new Vector2(1f, 1f);
        textRect.pivot = new Vector2(0.5f, 1f);

        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;


        storyText.textWrappingMode = TextWrappingModes.Normal;
        storyText.enableAutoSizing = false;
        storyText.overflowMode = TextOverflowModes.Overflow;


        storyText.text = backstory;
        storyText.maxVisibleCharacters = 0;


        storyText.ForceMeshUpdate();

        float requiredHeight =
            storyText.preferredHeight + bottomPadding;

        textRect.SetSizeWithCurrentAnchors(
            RectTransform.Axis.Vertical,
            requiredHeight
        );

        textRect.anchoredPosition = Vector2.zero;

        currentScrollOffset = 0f;


        storyText.ForceMeshUpdate();

        typingCoroutine = StartCoroutine(TypeStory());
    }

    private void Update()
    {
        if (!introActive || textFinished)
            return;


        if (Keyboard.current != null &&
            Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            SkipTyping();
        }
    }

    private IEnumerator TypeStory()
    {
        storyText.ForceMeshUpdate();

        int totalCharacters =
            storyText.textInfo.characterCount;

        int previousLine = -1;

        for (int i = 0; i < totalCharacters; i++)
        {
            storyText.maxVisibleCharacters = i + 1;

            int currentLine =
                storyText.textInfo.characterInfo[i].lineNumber;


            if (currentLine != previousLine)
            {
                previousLine = currentLine;
                ScrollToLine(currentLine);
            }

            yield return new WaitForSecondsRealtime(
                Mathf.Max(0.001f, typingSpeed)
            );
        }

        FinishTyping();
    }

    private void ScrollToLine(int lineIndex)
    {
        if (storyText == null || textViewport == null)
            return;

        TMP_TextInfo info = storyText.textInfo;

        if (lineIndex < 0 || lineIndex >= info.lineCount)
            return;

        TMP_LineInfo line = info.lineInfo[lineIndex];


        Vector3 lineWorldPosition =
            textRect.TransformPoint(
                new Vector3(0f, line.descender, 0f)
            );

        Vector3 viewportPosition =
            textViewport.InverseTransformPoint(
                lineWorldPosition
            );


        float triggerY =
            textViewport.rect.yMax -
            textViewport.rect.height * scrollTriggerPoint;


        if (viewportPosition.y >= triggerY)
            return;

        float additionalScroll =
            triggerY - viewportPosition.y;

        float targetOffset =
            currentScrollOffset + additionalScroll;


        if (scrollingCoroutine != null)
            StopCoroutine(scrollingCoroutine);

        scrollingCoroutine =
            StartCoroutine(SmoothScroll(targetOffset));
    }

    private IEnumerator SmoothScroll(float targetOffset)
    {
        float startOffset = currentScrollOffset;
        float elapsed = 0f;

        float duration =
            Mathf.Max(0.01f, scrollDuration);

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(
                elapsed / duration
            );


            t = t * t * (3f - 2f * t);

            currentScrollOffset =
                Mathf.Lerp(startOffset, targetOffset, t);

            textRect.anchoredPosition =
                new Vector2(0f, currentScrollOffset);

            yield return null;
        }

        currentScrollOffset = targetOffset;

        textRect.anchoredPosition =
            new Vector2(0f, currentScrollOffset);

        scrollingCoroutine = null;
    }

    private void SkipTyping()
    {
        if (textFinished)
            return;


        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }


        if (scrollingCoroutine != null)
        {
            StopCoroutine(scrollingCoroutine);
            scrollingCoroutine = null;
        }


        storyText.maxVisibleCharacters = int.MaxValue;
        storyText.ForceMeshUpdate();


        int lastLine = storyText.textInfo.lineCount - 1;

        if (lastLine >= 0)
        {
            TMP_LineInfo line =
                storyText.textInfo.lineInfo[lastLine];

            Vector3 worldPosition =
                textRect.TransformPoint(
                    new Vector3(0f, line.descender, 0f)
                );

            Vector3 viewportPosition =
                textViewport.InverseTransformPoint(
                    worldPosition
                );

            float triggerY =
                textViewport.rect.yMax -
                textViewport.rect.height * scrollTriggerPoint;

            float additionalScroll =
                Mathf.Max(0f, triggerY - viewportPosition.y);

            currentScrollOffset += additionalScroll;

            textRect.anchoredPosition =
                new Vector2(0f, currentScrollOffset);
        }

        FinishTyping();
    }

    private void FinishTyping()
    {
        if (textFinished)
            return;

        textFinished = true;


        if (skipText != null)
            skipText.SetActive(false);


        if (continueButton != null)
            continueButton.gameObject.SetActive(true);
    }

    public void ContinueTutorial()
    {
        if (!textFinished || !introActive)
            return;

        introActive = false;


        if (storyPanel != null)
            storyPanel.SetActive(false);


        if (tutorialManager != null)
            tutorialManager.enabled = true;

        if (anomalyObserver != null)
            anomalyObserver.enabled = true;


        if (playerController != null)
            playerController.enabled = true;


        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnDestroy()
    {
        if (continueButton != null)
        {
            continueButton.onClick.RemoveListener(
                ContinueTutorial
            );
        }
    }
}
