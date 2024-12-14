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

    public void PlayAudio(AudioType audioType, string clipName)
    {
        var clip = FindClip(audioType, clipName);
        if (clip != null)
        {
            AudioSource audioSource = GetAudioSource(audioType);
            audioSource.pitch = 1f;
            
            GetAudioSource(audioType).PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"AudioManager: Clip not found for type: {audioType}, name: {clipName}");
        }
    }
    public void PlayAudioWithRandomPitch(AudioType audioType, string clipName, float minPitch = 0.9f, float maxPitch = 1.1f)
    {
        var clip = FindClip(audioType, clipName);
        if (clip != null)
        {
            AudioSource audioSource = GetAudioSource(audioType);

            // Randomize the pitch
            float randomPitch = Random.Range(minPitch, maxPitch);
            audioSource.pitch = randomPitch;

            // Play the audio
            audioSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"AudioManager: Clip not found for type: {audioType}, name: {clipName}");
        }
    }

    private AudioSource GetAudioSource(AudioType audioType)
    {
        switch (audioType)
        {
            case AudioType.UI:
                return uiSource;
            case AudioType.Game:
                return gameSource;
            case AudioType.Music:
                return musicSource;
            default:
                return null;
        }
    }
    private AudioClip FindClip(AudioType audioType, string clipName)
    {
        foreach (var data in audioClipData)
        {
            if (data.AudioType == audioType && data.ClipName == clipName)
            {
                return data.Clip;
            }
        }
        return null;
    }
}
