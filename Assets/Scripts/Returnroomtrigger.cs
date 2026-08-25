using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnRoomTrigger : MonoBehaviour
{
    public string playerTag = "Player";
    public string sceneToLoad;
    public bool requireAnomalySpotted = true;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;
        if (requireAnomalySpotted && !LineOfSightDetector.AnomalySpotted) return;

        if (string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.LogError($"{name}: ReturnRoomTrigger has no scene assigned in sceneToLoad.", this);
            return;
        }

        SceneManager.LoadScene(sceneToLoad);
    }
}