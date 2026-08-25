using UnityEngine;
using TMPro;
using System.Collections;

public class LineOfSightDetector : MonoBehaviour
{
    [Header("Target")]
    public Transform target;
    public Transform eye;

    [Header("Detection Settings")]
    public float viewAngle = 60f;
    public float viewDistance = 20f;
    public LayerMask obstructionMask;
    public bool triggerOnce = true;

    [Header("Warning Text")]
    public TMP_Text warningText;
    public string warningMessage = "Anomaly spotted, return to original room.";
    public float warningDisplayDuration = 4f;

    public static bool AnomalySpotted { get; private set; } = false;

    private bool hasTriggered = false;
    private Coroutine textRoutine;

    private void Awake()
    {
        if (warningText != null)
        {
            warningText.text = warningMessage;
            warningText.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (triggerOnce && hasTriggered) return;
        if (target == null || eye == null) return;

        Vector3 toTarget = target.position - eye.position;
        float distance = toTarget.magnitude;
        if (distance > viewDistance) return;

        float angle = Vector3.Angle(eye.forward, toTarget);
        if (angle > viewAngle * 0.5f) return;

        if (Physics.Raycast(eye.position, toTarget.normalized, out RaycastHit hit, viewDistance, obstructionMask))
        {
            if (hit.transform != target) return;
        }

        SpotAnomaly();
    }

    private void SpotAnomaly()
    {
        hasTriggered = true;
        AnomalySpotted = true;

        if (warningText != null)
        {
            if (textRoutine != null)
                StopCoroutine(textRoutine);
            textRoutine = StartCoroutine(ShowWarningTemporarily());
        }
    }

    private IEnumerator ShowWarningTemporarily()
    {
        warningText.gameObject.SetActive(true);
        yield return new WaitForSeconds(warningDisplayDuration);
        warningText.gameObject.SetActive(false);
    }

    public static void ResetAnomalyState()
    {
        AnomalySpotted = false;
    }
}