using UnityEngine;
using UnityEngine.Events;

public class AnomalyObserver : MonoBehaviour
{
    [Header("References")]
    public RoomManager roomManager;
    public bool followRoomManagerSelection = true;
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
        if (roomManager == null) roomManager = FindFirstObjectByType<RoomManager>();
    }

    private void Start()
    {
        RefreshTargetFromRoom();
    }

    public void RefreshTargetFromRoom()
    {
        if (!followRoomManagerSelection || roomManager == null) return;
        Transform newTarget = roomManager.anomalyObject != null ? roomManager.anomalyObject.transform : null;
        if (newTarget != target)
        {
            target = newTarget;
            HasSpotted = false;
            seenTimer = 0f;
        }
    }

    private void Update()
    {
        if (followRoomManagerSelection && roomManager != null && roomManager.anomalyObject != null && target != roomManager.anomalyObject.transform)
            RefreshTargetFromRoom();
        if (HasSpotted || target == null || eye == null) return;
        if (!target.gameObject.activeInHierarchy)
        {
            seenTimer = 0f;
            return;
        }
        if (IsTargetVisible())
        {
            seenTimer += Time.deltaTime;
            if (seenTimer >= Mathf.Max(0f, lookTimeRequired))
            {
                HasSpotted = true;
                onSpotted.Invoke();
            }
        }
        else seenTimer = 0f;
    }

    private bool IsTargetVisible()
    {
        Vector3 targetPoint = GetTargetPoint();
        Vector3 toTarget = targetPoint - eye.position;
        float distance = toTarget.magnitude;
        if (distance < 0.01f) return true;
        if (distance > viewDistance || Vector3.Angle(eye.forward, toTarget) > viewAngle * 0.5f)
            return false;
        if (Physics.Raycast(eye.position, toTarget / distance, out RaycastHit hit, distance, obstructionMask, QueryTriggerInteraction.Ignore))
            return hit.transform == target || hit.transform.IsChildOf(target);
        return true;
    }

    private Vector3 GetTargetPoint()
    {
        Renderer r = target.GetComponentInChildren<Renderer>();
        return r != null ? r.bounds.center : target.position;
    }
}
