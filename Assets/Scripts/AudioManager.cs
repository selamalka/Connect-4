using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioMixer audioMixer; // Reference to the Audio Mixer

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource gameSource;
    [SerializeField] private AudioSource uiSource;

    [SerializeField] private List<AudioClipData> audioClipData; // A single list for all audio clips

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        PlayMusic("Music");
    }

    public void PlayAudio(AudioType audioType, string clipName)
    {
        var clip = FindClip(audioType, clipName);
        if (clip != null)
        {
            GetAudioSource(audioType).PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"AudioManager: Clip not found for type: {audioType}, name: {clipName}");
        }
    }

    public void PlayMusic(string clipName)
    {
        var clip = FindClip(AudioType.Music, clipName);
        if (clip != null)
        {
            musicSource.clip = clip;
            musicSource.Play();
        }
        else
        {
            Debug.LogWarning($"AudioManager: Music clip not found: {clipName}");
        }
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void SetVolume(AudioType audioType, float volume)
    {
        string mixerParameter = GetMixerParameter(audioType);
        if (!string.IsNullOrEmpty(mixerParameter))
        {
            // Convert volume (0-1) to decibels for the mixer (-80 to 0)
            float decibels = Mathf.Lerp(-80f, 0f, Mathf.Clamp01(volume));
            audioMixer.SetFloat(mixerParameter, decibels);
        }
    }

    private AudioSource GetAudioSource(AudioType audioType)
    {
        return audioType switch
        {
            AudioType.UI => uiSource,
            AudioType.Game => gameSource,
            AudioType.Music => musicSource,
            _ => throw new System.ArgumentException($"Invalid AudioType: {audioType}")
        };
    }

    private AudioClip FindClip(AudioType audioType, string clipName)
    {
        foreach (var data in audioClipData)
        {
            if (data.audioType == audioType && data.clipName == clipName)
            {
                return data.clip;
            }
        }
        return null;
    }

    private string GetMixerParameter(AudioType audioType)
    {
        return audioType switch
        {
            AudioType.UI => "UIVolume",
            AudioType.Game => "GameVolume",
            AudioType.Music => "MusicVolume",
            _ => null
        };
    }
}
