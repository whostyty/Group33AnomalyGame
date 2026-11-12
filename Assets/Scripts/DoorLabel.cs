using System.Collections;
using UnityEngine;

public class DoorLabel : MonoBehaviour
{
    [Header("Hover")]
    public float bobHeight = 0.08f;
    public float bobSpeed = 2f;

    [Header("Pop In")]
    public float popDuration = 0.3f;

    [Header("Facing")]
    public bool faceCamera = true;

    private Transform cam;
    private Vector3 basePosition;
    private Vector3 baseScale;
    private bool initialised;

    private void OnEnable()
    {
        if (!initialised)
        {
            basePosition = transform.position;
            baseScale = transform.localScale;
            initialised = true;
        }

        if (cam == null)
        {
            FPController fp = FindFirstObjectByType<FPController>();
            if (fp != null && fp.cameraTransform != null)
                cam = fp.cameraTransform;
            else if (Camera.main != null)
                cam = Camera.main.transform;
        }

        StartCoroutine(PopIn());
    }

    private void LateUpdate()
    {
        transform.position = basePosition + Vector3.up * (Mathf.Sin(Time.unscaledTime * bobSpeed) * bobHeight);

        if (faceCamera && cam != null)
        {
            Vector3 away = transform.position - cam.position;
            away.y = 0f;
            if (away.sqrMagnitude > 0.0001f)
                transform.rotation = Quaternion.LookRotation(away);
        }
    }

    private IEnumerator PopIn()
    {
        float elapsed = 0f;
        transform.localScale = Vector3.zero;

        while (elapsed < popDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(elapsed / popDuration);
            float c1 = 1.70158f;
            float c3 = c1 + 1f;
            float eased = 1f + c3 * Mathf.Pow(k - 1f, 3f) + c1 * Mathf.Pow(k - 1f, 2f);
            transform.localScale = baseScale * eased;
            yield return null;
        }

        transform.localScale = baseScale;
    }
}