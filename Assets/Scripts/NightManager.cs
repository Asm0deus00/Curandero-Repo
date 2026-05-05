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

    // Saved in Awake so RestoreUI is always safe to call
    private Vector2 mixOriginalPos;
    private Vector2 timerOriginalPos;

    [Header("Camera Transition")]
    public SimpleCameraMove cameraMove;
    public Transform gameAnchor;   // Anchor for the patient/game view
    public Transform titleAnchor;  // Anchor for the title screen

    [Header("Progression")]
    public int currentNight = 1;
    public int maxNights = 3;

    [Header("Debug")]
    public bool enableDebugSkip = true;

    private float timer;
    private bool nightActive = false;
    private bool isInTransition = false; // Guards pause and double-EndNight calls

    private int totalPatients = 0;
    private int correctPatients = 0;

    // ----------------------------
    void Awake()
    {
        // Save UI positions here so RestoreUI() is always safe,
        // even if called before StartNight() (e.g. from ResetGameState).
        if (mixButton != null)
            mixOriginalPos = mixButton.anchoredPosition;

        if (timerText != null)
            timerOriginalPos = timerText.anchoredPosition;
    }

    // ----------------------------
    public void StartNight()
    {
        timer = nightDuration;
        nightActive = true;
        isInTransition = false;

        totalPatients = 0;
        correctPatients = 0;

        SetOverlayAlpha(0f);

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
        // Guard against being called twice (timer tick + debug skip race)
        if (isInTransition) return;

        nightActive = false;
        isInTransition = true;

        dialogue.HideDialogue();

        StartCoroutine(EndSequence());
    }

    // ----------------------------
    IEnumerator EndSequence()
    {
        // Disable GameManager immediately so ProcessTreatment can't
        // fire a new Transition() while the end sequence is running.
        gameManager.enabled = false;

        HideUI();

        // 1. Move patient off screen first (while camera may still be at mix station)
        if (gameManager != null)
            yield return StartCoroutine(gameManager.ForcePatientExit());

        // 2. Move camera back to the patient/game view before fading in,
        //    so the result message always appears in the right place.
        cameraMove.MoveTo(gameAnchor);
        yield return new WaitForSeconds(cameraMove.moveDuration);

        // 3. Fade screen to black
        yield return StartCoroutine(FadeIn());

        yield return new WaitForSeconds(0.5f);

        // 4. Show result message.
        // Restore text alpha to fully visible before typing — FadeOutText() leaves
        // it at 0, which would make subsequent nights render invisible text.
        Color textColor = resultText.textUI.color;
        textColor.a = 1f;
        resultText.textUI.color = textColor;

        float ratio = totalPatients == 0 ? 0f : (float)correctPatients / totalPatients;
        string message = GetResultMessage(ratio);

        resultText.StartTyping(message);

        while (resultText.IsTyping)
            yield return null;

        yield return new WaitForSeconds(3.5f);

        // 5. Fade out the result text, then clear it
        yield return StartCoroutine(FadeOutText());

        resultText.StopTyping();
        resultText.ClearText();

        // 6. Branch: game over / next night / final night
        if (ratio < 0.3f)
        {
            yield return StartCoroutine(ReturnToTitle());
        }
        else if (currentNight < maxNights)
        {
            currentNight++;

            // Fade out overlay so the next night starts clean
            yield return StartCoroutine(FadeOut());

            RestoreUI();

            gameManager.enabled = true;
            isInTransition = false;
            StartNight();
        }
        else
        {
            yield return StartCoroutine(ReturnToTitle());
        }
    }

    // ----------------------------
    // Fade out only the result text (not the black overlay).
    // Bug fix: was incorrectly resetting alpha to 1 after the loop.
    IEnumerator FadeOutText()
    {
        TextMeshProUGUI text = resultText.textUI;

        float t = 1f;
        Color c = text.color;

        while (t > 0f)
        {
            t -= Time.deltaTime * 0.5f;
            c.a = Mathf.Max(t, 0f);
            text.color = c;
            yield return null;
        }

        // Leave alpha at 0 (was wrongly set to 1 before)
        c.a = 0f;
        text.color = c;
    }

    // ----------------------------
    IEnumerator ReturnToTitle()
    {
        yield return new WaitForSeconds(1f);

        cameraMove.MoveTo(titleAnchor);
        yield return new WaitForSeconds(cameraMove.moveDuration);

        // Fade out overlay after arriving at title
        yield return StartCoroutine(FadeOut());

        ResetGameState();
    }

    // ----------------------------
    public void ResetGameState()
    {
        currentNight = 1;
        isInTransition = false;

        // UI positions were saved in Awake, so this is always safe
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
    public float GetRemainingTime() => timer;

    public bool IsNightActive() => nightActive;

    public bool IsInTransition() => isInTransition;
}