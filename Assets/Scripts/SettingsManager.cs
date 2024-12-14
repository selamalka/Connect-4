using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private AudioMixer mainMixer;

    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider gameSFXVolumeSlider;
    [SerializeField] private Slider uiSFXVolumeSlider;

    [SerializeField] private GameObject settingsPanel;

    private void Start()
    {
        SetValuesToDefault();
    }

    public void SetValuesToDefault()
    {
        // Update slider values
        gameSFXVolumeSlider.value = 0.8f;
        uiSFXVolumeSlider.value = 0.8f;
        musicVolumeSlider.value = 0.5f;

        // Invoke the onValueChanged events manually
        gameSFXVolumeSlider.onValueChanged.Invoke(gameSFXVolumeSlider.value);
        uiSFXVolumeSlider.onValueChanged.Invoke(uiSFXVolumeSlider.value);
        musicVolumeSlider.onValueChanged.Invoke(musicVolumeSlider.value);
    }


    private float ConvertLinearToLog(float sliderValue)
    {
        return Mathf.Log10(sliderValue) * 40;
    }

    public void SetMusicVolumeBySettings(float value)
    {
        mainMixer.SetFloat("MusicVolume", ConvertLinearToLog(value));
    }

    public void SetGameVolumeBySettings(float value)
    {
        mainMixer.SetFloat("GameVolume", ConvertLinearToLog(value));
    }

    public void SetUIVolumeBySettings(float value)
    {
        mainMixer.SetFloat("UIVolume", ConvertLinearToLog(value));
    }
}