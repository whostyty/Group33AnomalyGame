using UnityEngine;

public class AnswerDoor : MonoBehaviour
{
    public RoomManager room;
    public bool isAnomalyDoor;
    public string playerTag = "Player";

    private void Reset()
    {
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.isTrigger = true;
    }

    private void Awake()
    {
        Collider col = GetComponent<Collider>();
        if (col == null || !col.isTrigger)
            Debug.LogWarning($"{name}: AnswerDoor needs a Collider with Is Trigger enabled.", this);

        if (room == null)
            room = FindFirstObjectByType<RoomManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        if (room == null)
        {
            Debug.LogError($"{name}: AnswerDoor has no RoomManager assigned.", this);
            return;
        }

        room.SubmitAnswer(isAnomalyDoor);
    }
}