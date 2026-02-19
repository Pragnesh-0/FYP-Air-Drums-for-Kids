using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    public AudioMixer masterGroup;

    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider drumVolumeSlider;
    public Slider soundEffectVolumeSlider;




    void Start()
    {
        masterVolumeSlider.onValueChanged.AddListener(delegate{applyVolume(masterVolumeSlider.value, "MasterVolume");});
        musicVolumeSlider.onValueChanged.AddListener(delegate{applyVolume(musicVolumeSlider.value, "MusicVolume");});
        drumVolumeSlider.onValueChanged.AddListener(delegate{applyVolume(drumVolumeSlider.value, "DrumVolume");});
        soundEffectVolumeSlider.onValueChanged.AddListener(delegate{applyVolume(soundEffectVolumeSlider.value, "SoundEffectVolume");});
    }


    public void applyVolume(float volume, string type)
    {
        masterGroup.SetFloat(type, 20f * Mathf.Log10(volume));
    }

    public void setSettings(GameAudioSettings audioSettings)
    {
        masterVolumeSlider.value = audioSettings.masterVolume;
        musicVolumeSlider.value = audioSettings.musicPlayerVoume;
        drumVolumeSlider.value = audioSettings.drumSoundVolume;
        soundEffectVolumeSlider.value = audioSettings.soundEffectVolume;

        applyVolume(masterVolumeSlider.value, "MasterVolume");
        applyVolume(musicVolumeSlider.value, "MusicVolume");
        applyVolume(drumVolumeSlider.value, "DrumVolume");
        applyVolume(soundEffectVolumeSlider.value, "SoundEffectVolume");
    }


    public GameAudioSettings getSettings()
    {
        GameAudioSettings gas = new GameAudioSettings();
        gas.masterVolume = masterVolumeSlider.value;
        gas.musicPlayerVoume = musicVolumeSlider.value;
        gas.drumSoundVolume = drumVolumeSlider.value;
        gas.soundEffectVolume = soundEffectVolumeSlider.value;
        return gas;
    }

}
