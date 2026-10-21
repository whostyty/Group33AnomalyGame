using UnityEngine;
using UnityEngine.Events;

public class AnomalyObserver : MonoBehaviour
{
    [Header("References (auto-found if empty)")]
    public Transform target;
    public Transform eye;

    [Header("Detection")]
    public float viewAngle = 50f;
    public float viewDistance = 25f;
    public float lookTimeRequired = 0.5f;
    public LayerMask obstructionMask = ~0;

    [Header("Events")]
    public UnityEvent onSpotted = new UnityEvent();

    public bool HasSpotted { get; private set; }

    private float seenTimer;

    private void Awake()
    {
        if (eye == null)
        {
            FPController fp = FindFirstObjectByType<FPController>();
            if (fp != null) eye = fp.cameraTransform;
        }

        if (target == null)
        {
            RoomManager room = FindFirstObjectByType<RoomManager>();
            if (room != null && room.anomalyObject != null)
                target = room.anomalyObject.transform;
        }

        if (eye == null)
            Debug.LogWarning($"{name}: AnomalyObserver has no Eye (player camera).", this);
        if (target == null)
            Debug.LogWarning($"{name}: AnomalyObserver has no Target (anomaly object).", this);
    }

    private void Update()
    {
        if (HasSpotted || target == null || eye == null) return;

        if (!target.gameObject.activeInHierarchy)
        {
            seenTimer = 0f;
            return;
        }

        if (IsTargetVisible())
        {
            seenTimer += Time.deltaTime;
            if (seenTimer >= lookTimeRequired)
            {
                HasSpotted = true;
                onSpotted.Invoke();
            }
        }
        else
        {
            seenTimer = 0f;
        }
    }

    private bool IsTargetVisible()
    {
        Vector3 targetPoint = GetTargetPoint();
        Vector3 toTarget = targetPoint - eye.position;
        float distance = toTarget.magnitude;

        if (distance < 0.01f) return true;
        if (distance > viewDistance) return false;
        if (Vector3.Angle(eye.forward, toTarget) > viewAngle * 0.5f) return false;

        if (Physics.Raycast(eye.position, toTarget / distance, out RaycastHit hit, distance, obstructionMask, QueryTriggerInteraction.Ignore))
        {
            if (!hit.transform.IsChildOf(target))
                return false;
        }

        return true;
    }

    private Vector3 GetTargetPoint()
    {
        Renderer r = target.GetComponentInChildren<Renderer>();
        return r != null ? r.bounds.center : target.position;
    }
}