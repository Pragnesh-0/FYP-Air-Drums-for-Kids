using UnityEngine;
using System.IO;


[System.Serializable]
public class SettingsData: MonoBehaviour
{
    
    static string cachedSettings="";

    public static void saveSettings(Settings newSettings)
    {
        if (JsonUtility.ToJson(newSettings) == JsonUtility.ToJson(cachedSettings))
        {
            return;
        }
        cachedSettings = JsonUtility.ToJson(newSettings);
        Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, "Settings"));
        string path = Path.Combine(Application.persistentDataPath, "Settings");
        string data = JsonUtility.ToJson(newSettings);
        File.WriteAllText(Path.Combine(path,"settings.json"), data);
    }

    public static Settings getSettings()
    {
        Settings settings = new Settings();
        settings.accessibilitySettings = new AccessibilitySettings();
        settings.gameAudioData = new GameAudioSettings();
        settings.calibrationSettings = new CalibrationSettings();
        Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, "Settings"));
        string path = Path.Combine(Application.persistentDataPath, "Settings");

        if (!File.Exists(Path.Combine(path,"settings.json")))
        {
            settings.equippedKit = "Default";

            settings.gameAudioData.masterVolume = 0.25f;
            settings.gameAudioData.soundEffectVolume = 1f;
            settings.gameAudioData.drumSoundVolume = 1f;
            settings.gameAudioData.musicPlayerVoume = 1f;

            settings.calibrationSettings.reboundThreshold = 0.5f;
            settings.calibrationSettings.noise = 0.5f;
            settings.calibrationSettings.lag = 0.5f;

            settings.accessibilitySettings.pedalEnabled = false;

            saveSettings(settings);
        }
        else
        {
            string data = File.ReadAllText(Path.Combine(path,"settings.json"));
            settings = JsonUtility.FromJson<Settings>(data);
        }

        return settings;
    }

}