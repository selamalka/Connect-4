using UnityEngine;

[CreateAssetMenu(fileName = "NewAudioClipData", menuName = "Audio/AudioClipData")]
public class AudioClipData : ScriptableObject
{
    public AudioType AudioType;   // Type of audio: UI, Game, or Music
    public string ClipName;       // Descriptive name for the clip
    public AudioClip Clip;        // Reference to the audio clip
}