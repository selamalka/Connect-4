using UnityEngine;

[CreateAssetMenu(fileName = "NewAudioClipData", menuName = "Audio/AudioClipData")]
public class AudioClipData : ScriptableObject
{
    public AudioType audioType;   // Type of audio: UI, Game, or Music
    public string clipName;       // Descriptive name for the clip
    public AudioClip clip;        // Reference to the audio clip
}