using UnityEngine;

public class ClickManager : MonoBehaviour
{
    [Header("General Audio")]
    private AudioSource audioSource;

    [Header("Patient Body Audio")]
    public AudioClip bodyClickSound;

    [Range(0f, 1f)]
    public float bodyVolume = 0.7f;

    [Header("Pitch Randomization")]
    public float minPitch = 0.85f;
    public float maxPitch = 1.15f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            Collider2D hit = Physics2D.OverlapPoint(mouseWorld);

            if (hit != null)
            {
                Debug.Log("Hit: " + hit.name);

                // Object-specific audio
                ClickAudio clickAudio = hit.GetComponent<ClickAudio>();

                if (clickAudio != null && clickAudio.clickSound != null)
                {
                    audioSource.PlayOneShot(clickAudio.clickSound, clickAudio.volume);
                }

                // Patient body-part logic
                ClickablePart part = hit.GetComponent<ClickablePart>();

                if (part != null)
                {
                    PlayBodySound();
                    part.OnClicked();
                }
            }
        }
    }

    void PlayBodySound()
    {
        if (bodyClickSound == null)
            return;

        GameObject tempAudio = new GameObject("TempBodyAudio");

        AudioSource tempSource = tempAudio.AddComponent<AudioSource>();

        tempSource.clip = bodyClickSound;
        tempSource.volume = bodyVolume;
        tempSource.pitch = Random.Range(minPitch, maxPitch);
        tempSource.spatialBlend = 0f;

        tempSource.Play();

        Destroy(tempAudio, bodyClickSound.length + 0.1f);
    }
}