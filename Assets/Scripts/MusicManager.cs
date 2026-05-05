using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [Header("Music Settings")]
    public AudioClip backgroundMusic;

    [Range(0f, 1f)]
    public float volume = 0.4f;

    private AudioSource audioSource;

    void Awake()
    {
        // Prevent duplicates
        if (FindObjectsOfType<MusicManager>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.clip = backgroundMusic;
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;
        audioSource.volume = volume;

        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    void Update()
    {
        // Keeps inspector slider synced during play mode
        audioSource.volume = volume;
    }
}