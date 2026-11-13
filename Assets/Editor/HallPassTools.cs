using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class HallPassTools
{
    [MenuItem("Hall Pass/Doors/Create Movable Bathroom Stall Door")]
    public static void CreateStallDoor()
    {
        const string prefabPath = "Assets/Prefabs/HallPass_Stall_Door.prefab";
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (prefab == null)
        {
            EditorUtility.DisplayDialog("Hall Pass", "The stall door prefab could not be found at " + prefabPath, "OK");
            return;
        }

        Transform location = Selection.activeTransform;
        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, SceneManager.GetActiveScene());
        if (instance == null) return;
        Undo.RegisterCreatedObjectUndo(instance, "Create Hall Pass Stall Door");
        instance.transform.position = location != null ? location.position : Vector3.zero;
        instance.transform.rotation = location != null ? location.rotation : Quaternion.identity;
        Selection.activeGameObject = instance;
        EditorSceneManager.MarkSceneDirty(instance.scene);
        EditorGUIUtility.PingObject(instance);
    }

    [MenuItem("Hall Pass/Anomalies/Make Selected Object The Room Anomaly")]
    public static void SetRoomAnomaly()
    {
        GameObject selected = Selection.activeGameObject;
        RoomManager room = Object.FindFirstObjectByType<RoomManager>(FindObjectsInactive.Include);
        if (selected == null || room == null)
        {
            EditorUtility.DisplayDialog("Hall Pass", "Select an anomaly GameObject in a scene containing a RoomManager.", "OK");
            return;
        }

        Undo.RecordObject(room, "Select Hall Pass Anomaly");
        room.anomalyObject = selected;
        room.chooseRandomAnomalyFromList = false;
        EditorUtility.SetDirty(room);

        AnomalyObserver observer = Object.FindFirstObjectByType<AnomalyObserver>(FindObjectsInactive.Include);
        if (observer != null)
        {
            Undo.RecordObject(observer, "Update Anomaly Observer");
            observer.roomManager = room;
            observer.followRoomManagerSelection = true;
            observer.target = selected.transform;
            EditorUtility.SetDirty(observer);
        }

        EditorSceneManager.MarkSceneDirty(room.gameObject.scene);
        Selection.activeGameObject = room.gameObject;
        EditorGUIUtility.PingObject(room);
        Debug.Log("Hall Pass anomaly set to: " + selected.name + " in " + room.gameObject.scene.name);
    }

    [MenuItem("Hall Pass/Tools/Inspect Door and Anomaly Setup")]
    public static void InspectScene()
    {
        Scene scene = SceneManager.GetActiveScene();
        InteractiveDoor[] doors = Object.FindObjectsByType<InteractiveDoor>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        AnswerDoor[] answers = Object.FindObjectsByType<AnswerDoor>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        RoomManager[] rooms = Object.FindObjectsByType<RoomManager>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        int doorCount = 0;
        int answerCount = 0;
        foreach (InteractiveDoor door in doors)
        {
            if (door.gameObject.scene != scene) continue;
            doorCount++;
            if (door.doorPanel == null) Debug.LogWarning("Door has no Panel: " + door.name, door);
        }
        foreach (AnswerDoor answer in answers)
            if (answer.gameObject.scene == scene) answerCount++;

        RoomManager roomInScene = null;
        foreach (RoomManager room in rooms)
            if (room.gameObject.scene == scene) { roomInScene = room; break; }

        if (roomInScene != null && roomInScene.anomalyObject == null)
            Debug.LogWarning("No anomaly GameObject has been selected in RoomManager", roomInScene);
        Debug.Log("Hall Pass scene: " + scene.name + ". Interactive doors: " + doorCount + ". Answer triggers: " + answerCount + ". Room Manager: " + (roomInScene != null));
    }
}
