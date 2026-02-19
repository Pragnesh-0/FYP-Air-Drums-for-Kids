using UnityEngine;
[System.Serializable]
public class Settings
{
    public string equippedKit;
    public GameAudioSettings gameAudioData;
    public AccessibilitySettings accessibilitySettings;
    public CalibrationSettings calibrationSettings;

}

[System.Serializable]
public class GameAudioSettings
{
    public float masterVolume;
    public float soundEffectVolume;
    public float musicPlayerVoume;
    public float drumSoundVolume;
}


[System.Serializable]
public class AccessibilitySettings
{
    public bool pedalEnabled;
}

[System.Serializable]
public class CalibrationSettings
{
    public float noise;
    public float lag;
    public float reboundThreshold;

}


