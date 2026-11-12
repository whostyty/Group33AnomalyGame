using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class HallPassBrightness : MonoBehaviour
{
    private static HallPassBrightness instance;
    private Volume volume;
    private VolumeProfile runtimeProfile;
    private ColorAdjustments colorAdjustments;

    public static void EnsureExists()
    {
        if (instance != null)
            return;

        GameObject settingsObject = new GameObject("HallPass Brightness");
        DontDestroyOnLoad(settingsObject);
        instance = settingsObject.AddComponent<HallPassBrightness>();
    }

    public static void Refresh()
    {
        EnsureExists();
        if (instance != null && instance.colorAdjustments != null)
            instance.colorAdjustments.postExposure.value = HallPassSettings.Exposure;
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        volume = gameObject.AddComponent<Volume>();
        volume.isGlobal = true;
        volume.priority = 100f;
        volume.weight = 1f;
        runtimeProfile = ScriptableObject.CreateInstance<VolumeProfile>();
        colorAdjustments = runtimeProfile.Add<ColorAdjustments>(false);
        colorAdjustments.postExposure.overrideState = true;
        colorAdjustments.postExposure.value = HallPassSettings.Exposure;
        volume.sharedProfile = runtimeProfile;
    }

    private void OnDestroy()
    {
        if (instance != this)
            return;

        instance = null;
        if (runtimeProfile != null)
            Destroy(runtimeProfile);
    }
}
