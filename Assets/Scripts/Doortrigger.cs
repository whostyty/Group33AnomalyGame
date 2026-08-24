using UnityEngine;
using TMPro;
using System.Collections;

public class DoorTrigger : MonoBehaviour
{
    [Header("Trigger Settings")]
    public string playerTag = "Player";
    public bool triggerOnce = true;

    [Header("Popup Object")]
    public GameObject popupObject;
    public float popDuration = 0.25f;
    public float popScale = 1f;

    [Header("Popup Text")]
    public TMP_Text popupText;
    public string popupMessage = "Object has now appeared";
    public float textDisplayDuration = 3f;

    private bool hasTriggered = false;
    private Coroutine scaleRoutine;
    private Coroutine textRoutine;

    private void Awake()
    {
        if (popupObject != null)
            popupObject.SetActive(false);

        if (popupText != null)
        {
            popupText.text = popupMessage;
            popupText.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;
        if (triggerOnce && hasTriggered) return;

        hasTriggered = true;
        ShowPopup();
    }

    private void ShowPopup()
    {
        if (popupObject != null)
        {
            popupObject.SetActive(true);
            popupObject.transform.localScale = Vector3.zero;

            if (scaleRoutine != null)
                StopCoroutine(scaleRoutine);
            scaleRoutine = StartCoroutine(PopScale());
        }

        if (popupText != null)
        {
            if (textRoutine != null)
                StopCoroutine(textRoutine);
            textRoutine = StartCoroutine(ShowTextTemporarily());
        }
    }

    private IEnumerator PopScale()
    {
        float elapsed = 0f;
        Vector3 target = Vector3.one * popScale;

        while (elapsed < popDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / popDuration;
            t = 1f - Mathf.Pow(1f - t, 3f);
            popupObject.transform.localScale = Vector3.Lerp(Vector3.zero, target, t);
            yield return null;
        }

        popupObject.transform.localScale = target;
    }

    private IEnumerator ShowTextTemporarily()
    {
        popupText.gameObject.SetActive(true);
        yield return new WaitForSeconds(textDisplayDuration);
        popupText.gameObject.SetActive(false);
    }
}