using UnityEngine;

public static class HallPassSettings
{
    private const string LookKey = "HallPass.LookSensitivity";
    private const string BrightnessKey = "HallPass.Brightness";

    public static float LookValue => Mathf.Clamp01(PlayerPrefs.GetFloat(LookKey, 0.35f));
    public static float BrightnessValue => Mathf.Clamp01(PlayerPrefs.GetFloat(BrightnessKey, 0.5f));
    public static float MouseSensitivity => Mathf.Lerp(0.03f, 0.14f, LookValue);
    public static float GamepadLookSpeed => Mathf.Lerp(70f, 240f, LookValue);
    public static float Exposure => Mathf.Lerp(-1f, 1.3f, BrightnessValue);

    public static void SetLookValue(float value)
    {
        PlayerPrefs.SetFloat(LookKey, Mathf.Clamp01(value));
        PlayerPrefs.Save();
    }

    public static void SetBrightnessValue(float value)
    {
        PlayerPrefs.SetFloat(BrightnessKey, Mathf.Clamp01(value));
        PlayerPrefs.Save();
    }
}
