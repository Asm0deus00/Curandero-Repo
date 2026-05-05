using UnityEngine;

public class ClickAudio : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioClip clickSound;

    [Range(0f, 1f)]
    public float volume = 1f;
}