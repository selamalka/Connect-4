using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private AudioMixer mainMixer;

    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider gameVolumeSlider;
    [SerializeField] private Slider uiVolumeSlider;

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
        SetValuesToDefault();
    }

    private void SaveSettings()
    {
        PlayerPrefs.SetFloat("MusicVolume", musicVolumeSlider.value);
        PlayerPrefs.SetFloat("GameVolume", gameVolumeSlider.value);
        PlayerPrefs.SetFloat("UIVolume", uiVolumeSlider.value);
    }
    private void LoadSettings()
    {
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume");
        float gameVolume = PlayerPrefs.GetFloat("GameVolume");
        float uiVolume = PlayerPrefs.GetFloat("UIVolume");

        musicVolumeSlider.value = musicVolume;
        gameVolumeSlider.value = gameVolume;
        uiVolumeSlider.value = uiVolume;

        musicVolumeSlider.onValueChanged.Invoke(musicVolumeSlider.value);
        gameVolumeSlider.onValueChanged.Invoke(gameVolumeSlider.value);
        uiVolumeSlider.onValueChanged.Invoke(uiVolumeSlider.value);
    }
    private void DefaultSettings()
    {
        musicVolumeSlider.value = 0.5f;
        gameVolumeSlider.value = 0.8f;
        uiVolumeSlider.value = 0.8f;

        musicVolumeSlider.onValueChanged.Invoke(musicVolumeSlider.value);
        gameVolumeSlider.onValueChanged.Invoke(gameVolumeSlider.value);
        uiVolumeSlider.onValueChanged.Invoke(uiVolumeSlider.value);
    }
    public void SetValuesToDefault()
    {
        musicVolumeSlider.value = 0.5f;
        gameVolumeSlider.value = 0.8f;
        uiVolumeSlider.value = 0.8f;

        PlayerPrefs.SetFloat("MusicVolume", musicVolumeSlider.value);
        PlayerPrefs.SetFloat("GameVolume", gameVolumeSlider.value);
        PlayerPrefs.SetFloat("UIVolume", uiVolumeSlider.value);

        musicVolumeSlider.onValueChanged.Invoke(musicVolumeSlider.value);
        gameVolumeSlider.onValueChanged.Invoke(gameVolumeSlider.value);
        uiVolumeSlider.onValueChanged.Invoke(uiVolumeSlider.value);
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