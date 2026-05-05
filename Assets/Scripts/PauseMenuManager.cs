using UnityEngine;
using System.Collections;

public class PauseMenuManager : MonoBehaviour
{
    [Header("Pause Menu")]
    public SpriteRenderer pausedTitle;
    public float slideDuration = 0.35f;

    [Header("Idle Animation")]
    public float floatSpeed = 1.5f;
    public float floatAmount = 0.05f;
    public float pulseSpeed = 2f;
    public float pulseAmount = 0.02f;

    private Vector3 hiddenLocalPosition;
    private Vector3 shownLocalPosition = new Vector3(0f, -3.45f, 6f);
    private bool isPaused = false;
    private Coroutine slideRoutine;

    private Vector3 titleBaseScale;
    private Vector3 titleBasePos;

    void Awake()
    {
        hiddenLocalPosition = transform.localPosition;

        if (pausedTitle)
        {
            titleBaseScale = pausedTitle.transform.localScale;
            titleBasePos   = pausedTitle.transform.localPosition;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            TogglePause();

        if (isPaused && pausedTitle)
            AnimateTitle();
    }

    void AnimateTitle()
    {
        // Gentle float up and down
        float floatOffset = Mathf.Sin(Time.unscaledTime * floatSpeed) * floatAmount;
        pausedTitle.transform.localPosition = new Vector3(
            titleBasePos.x,
            titleBasePos.y + floatOffset,
            titleBasePos.z
        );

        // Subtle breathing scale
        float pulse = Mathf.Sin(Time.unscaledTime * pulseSpeed) * pulseAmount;
        pausedTitle.transform.localScale = titleBaseScale * (1f + pulse);
    }

    public void TogglePause()
    {
        if (isPaused) Resume();
        else Pause();
    }

    void Pause()
    {
        isPaused = true;
        Time.timeScale = 0f;

        if (slideRoutine != null) StopCoroutine(slideRoutine);
        slideRoutine = StartCoroutine(SlideMenu(hiddenLocalPosition, shownLocalPosition, true));
    }

    public void Resume()
    {
        isPaused = false;
        Time.timeScale = 1f;

        if (slideRoutine != null) StopCoroutine(slideRoutine);
        slideRoutine = StartCoroutine(SlideMenu(shownLocalPosition, hiddenLocalPosition, false));
    }

    IEnumerator SlideMenu(Vector3 from, Vector3 to, bool isEntering)
    {
        float elapsed = 0f;

        while (elapsed < slideDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / slideDuration;
            float smoothT = isEntering ? Mathf.SmoothStep(0f, 1f, t) : t * t;
            transform.localPosition = Vector3.Lerp(from, to, smoothT);
            yield return null;
        }

        transform.localPosition = to;
    }

    void OnApplicationQuit() => PlayerPrefs.Save();
}