using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

/// <summary>
/// Manages the functionality of the Settings menu
/// </summary>
public class SettingsManager : MonoBehaviour
{
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider gameVolumeSlider;
    [SerializeField] private Slider uiVolumeSlider;

    private AudioMixer mainMixer;

    private void OnEnable()
    {
        UIManager.OnSaveSettings += SaveSettings;
        UIManager.OnCancelSettings += LoadSettings;
        UIManager.OnDefaultSettings += DefaultSettings;
    }

    private void OnDisable()
    {
        UIManager.OnSaveSettings -= SaveSettings;
        UIManager.OnCancelSettings -= LoadSettings;
        UIManager.OnDefaultSettings -= DefaultSettings;
    }

    private void Start()
    {
        mainMixer = AudioManager.Instance.AudioMixer;
    }

    private void SaveSettings()
    {
        // Save the current slider values to PlayerPrefs for persistence
        PlayerPrefs.SetFloat("MusicVolume", musicVolumeSlider.value);
        PlayerPrefs.SetFloat("GameVolume", gameVolumeSlider.value);
        PlayerPrefs.SetFloat("UIVolume", uiVolumeSlider.value);
    }
    private void LoadSettings()
    {
        // Load saved volume levels from PlayerPrefs
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        float gameVolume = PlayerPrefs.GetFloat("GameVolume", 0.8f);
        float uiVolume = PlayerPrefs.GetFloat("UIVolume", 0.8f);

        // Update slider values with loaded data
        musicVolumeSlider.value = musicVolume;
        gameVolumeSlider.value = gameVolume;
        uiVolumeSlider.value = uiVolume;

        // Trigger slider events to apply changes in the AudioMixer
        musicVolumeSlider.onValueChanged.Invoke(musicVolumeSlider.value);
        gameVolumeSlider.onValueChanged.Invoke(gameVolumeSlider.value);
        uiVolumeSlider.onValueChanged.Invoke(uiVolumeSlider.value);
    }
    private void DefaultSettings()
    {
        // Reset slider values to default levels
        musicVolumeSlider.value = 0.5f;
        gameVolumeSlider.value = 0.8f;
        uiVolumeSlider.value = 0.8f;

        // Trigger slider events to apply default values in the AudioMixer
        musicVolumeSlider.onValueChanged.Invoke(musicVolumeSlider.value);
        gameVolumeSlider.onValueChanged.Invoke(gameVolumeSlider.value);
        uiVolumeSlider.onValueChanged.Invoke(uiVolumeSlider.value);
    }

    private float ConvertLinearToLog(float sliderValue)
    {
        // Convert linear slider value to logarithmic scale for AudioMixer
        return Mathf.Log10(sliderValue) * 40;
    }

    public void SetMusicVolumeBySettings(float value)
    {
        // Set music volume in the AudioMixer using a logarithmic scale
        mainMixer.SetFloat("MusicVolume", ConvertLinearToLog(value));
    }
    public void SetGameVolumeBySettings(float value)
    {
        // Set game sound effects volume in the AudioMixer using a logarithmic scale
        mainMixer.SetFloat("GameVolume", ConvertLinearToLog(value));
    }
    public void SetUIVolumeBySettings(float value)
    {
        // Set UI sounds volume in the AudioMixer using a logarithmic scale
        mainMixer.SetFloat("UIVolume", ConvertLinearToLog(value));
    }
}
