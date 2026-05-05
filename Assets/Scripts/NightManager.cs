using UnityEngine;
using System.Collections;
using TMPro;

public class NightManager : MonoBehaviour
{
    [Header("Timing")]
    public float nightDuration = 180f;

    [Header("References")]
    public GameManager gameManager;
    public DialogueController dialogue;
    public TypewriterEffect resultText;
    public SpriteRenderer blackOverlay;

    [Header("UI Fix (Move Offscreen)")]
    public RectTransform mixButton;
    
    public RectTransform timerText;

    private Vector2 mixOriginalPos;
    private Vector2 timerOriginalPos;

    [Header("Camera Transition")]
    public Transform cameraTransform;
    public Vector3 titlePosition;
    public float cameraMoveSpeed = 2f;

    [Header("Progression")]
    public int currentNight = 1;
    public int maxNights = 3;

    [Header("Debug")]
    public bool enableDebugSkip = true;

    private float timer;
    private bool nightActive = false;

    private int totalPatients = 0;
    private int correctPatients = 0;

    // ----------------------------
    public void StartNight()
    {
        timer = nightDuration;
        nightActive = true;

        totalPatients = 0;
        correctPatients = 0;

        SetOverlayAlpha(0f);

        if (mixButton != null)
            mixOriginalPos = mixButton.anchoredPosition;


        if (timerText != null)
            timerOriginalPos = timerText.anchoredPosition;

        RestoreUI();

        gameManager.StartGame();
    }

    // ----------------------------
    void Update()
    {
        if (enableDebugSkip && nightActive && Input.GetKeyDown(KeyCode.Q))
        {
            EndNight();
            return;
        }

        if (!nightActive) return;

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            EndNight();
        }
    }

    // ----------------------------
    public void RegisterResult(bool success)
    {
        if (!nightActive) return;

        totalPatients++;

        if (success)
            correctPatients++;
    }

    // ----------------------------
    void EndNight()
    {
        nightActive = false;

        dialogue.HideDialogue();

        StartCoroutine(EndSequence());
    }

    // ----------------------------
    IEnumerator EndSequence()
    {
        gameManager.enabled = false;

        HideUI();

        if (gameManager != null)
            yield return StartCoroutine(gameManager.ForcePatientExit());

        yield return StartCoroutine(FadeIn());

        yield return new WaitForSeconds(0.5f);

        float ratio = totalPatients == 0 ? 0f : (float)correctPatients / totalPatients;

        string message = GetResultMessage(ratio);

        resultText.StartTyping(message);

        while (resultText.IsTyping)
            yield return null;

        yield return new WaitForSeconds(3.5f);

        yield return StartCoroutine(FadeOutText());

        resultText.StopTyping();
        resultText.ClearText();

        if (ratio < 0.3f)
        {
            yield return StartCoroutine(ReturnToTitle());
        }
        else if (currentNight < maxNights)
        {
            currentNight++;

            yield return StartCoroutine(FadeOut());

            RestoreUI();

            gameManager.enabled = true;
            StartNight();
        }
        else
        {
            yield return StartCoroutine(ReturnToTitle());
        }
    }

    // ----------------------------
    IEnumerator FadeOutText()
    {
        TextMeshProUGUI text = resultText.textUI;

        float t = 1f;
        Color c = text.color;

        while (t > 0f)
        {
            t -= Time.deltaTime * 0.5f;
            c.a = t;
            text.color = c;

            yield return null;
        }

        c.a = 1f;
        text.color = c;
    }

    // ----------------------------
    IEnumerator ReturnToTitle()
    {
        yield return new WaitForSeconds(1f);

        // Move camera
        while (Vector3.Distance(cameraTransform.position, titlePosition) > 0.05f)
        {
            cameraTransform.position = Vector3.Lerp(
                cameraTransform.position,
                titlePosition,
                Time.deltaTime * cameraMoveSpeed
            );

            yield return null;
        }

        cameraTransform.position = titlePosition;

        // 🔴 FADE OUT OVERLAY AFTER ARRIVAL
        yield return StartCoroutine(FadeOut());

        ResetGameState();
    }

    // ----------------------------
    public void ResetGameState()
    {
        currentNight = 1;

        RestoreUI();

        resultText.ClearText();

        gameManager.enabled = false;

        TitleManager title = FindObjectOfType<TitleManager>();
        if (title != null)
            title.gameObject.SetActive(true);
    }

    // ----------------------------
    void HideUI()
    {
        Vector2 off = new Vector2(9999, 9999);

        if (mixButton != null)
            mixButton.anchoredPosition = off;

        if (timerText != null)
            timerText.anchoredPosition = off;
    }

    void RestoreUI()
    {
        if (mixButton != null)
            mixButton.anchoredPosition = mixOriginalPos;

        if (timerText != null)
            timerText.anchoredPosition = timerOriginalPos;
    }

    // ----------------------------
    IEnumerator FadeIn()
    {
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime;
            SetOverlayAlpha(t);
            yield return null;
        }

        SetOverlayAlpha(1f);
    }

    IEnumerator FadeOut()
    {
        float t = 1f;

        while (t > 0f)
        {
            t -= Time.deltaTime;
            SetOverlayAlpha(t);
            yield return null;
        }

        SetOverlayAlpha(0f);
    }

    void SetOverlayAlpha(float value)
    {
        if (blackOverlay == null) return;

        Color c = blackOverlay.color;
        c.a = Mathf.Clamp01(value);
        blackOverlay.color = c;
    }

    // ----------------------------
    string GetResultMessage(float ratio)
    {
        if (ratio < 0.3f)
            return "Tus manos trajeron más muerte que vida. Los espíritus no escucharon tus mezclas. La comunidad ha decidido tu destino.";

        if (ratio < 0.5f)
            return "Algunos encontraron alivio en tus manos. Otros no resistieron. Aún no comprendes del todo. La noche continúa.";

        return "Tus decisiones trajeron equilibrio entre cuerpo y espíritu. Pero el conocimiento aún no es completo. Otra noche te espera.";
    }

    // ----------------------------
    public float GetRemainingTime()
    {
        return timer;
    }

    public bool IsNightActive()
    {
        return nightActive;
    }
}