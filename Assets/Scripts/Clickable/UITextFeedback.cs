using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;

public class UITextFeedback : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("References")]
    private RectTransform rect;

    [Header("Wiggle")]
    [Range(2f, 20f)]
    public float amplitude = 8f;

    [Range(2f, 12f)]
    public float frequency = 6f;

    [Range(0.1f, 1f)]
    public float duration = 0.3f;

    [Header("Hover")]
    public float hoverScale = 1.05f;
    public float hoverSpeed = 8f;

    [Header("Hover Animation")]
    public float pulseSpeed = 4f;
    public float pulseAmount = 0.01f;

    private Vector2 basePos;
    private Vector3 baseScale;

    private bool isHovering = false;
    private Coroutine wiggleRoutine;

    void Awake()
    {
        rect = GetComponent<RectTransform>();

        basePos = rect.anchoredPosition;
        baseScale = transform.localScale;
    }

    void Update()
    {
        float pulse = isHovering 
            ? Mathf.Sin(Time.time * pulseSpeed) * pulseAmount 
            : 0f;

        Vector3 targetScale = isHovering 
            ? baseScale * (hoverScale + pulse) 
            : baseScale;

        transform.localScale = Vector3.Lerp(
            transform.localScale,
            targetScale,
            Time.deltaTime * hoverSpeed
        );
    }

    // 🟡 HOVER ENTER
    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
    }

    // 🔵 HOVER EXIT
    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
    }

    // 🔴 CALLED BY BUTTON (NOT automatically)
    public void PlayClickFeedback()
    {
        if (wiggleRoutine != null)
            StopCoroutine(wiggleRoutine);

        wiggleRoutine = StartCoroutine(Wiggle());
    }

    IEnumerator Wiggle()
    {
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;

            float t = time / duration;
            float damping = 1f - t;

            float offset = Mathf.Sin(time * frequency * Mathf.PI * 2f)
                         * amplitude
                         * damping;

            Vector2 current = rect.anchoredPosition;

            rect.anchoredPosition = new Vector2(
                basePos.x + offset,
                current.y
            );

            yield return null;
        }

        // smooth return
        float returnTime = 0f;
        float returnDuration = 0.1f;

        while (returnTime < returnDuration)
        {
            returnTime += Time.deltaTime;
            float t = returnTime / returnDuration;

            Vector2 current = rect.anchoredPosition;

            float newX = Mathf.Lerp(current.x, basePos.x, t);

            rect.anchoredPosition = new Vector2(newX, current.y);

            yield return null;
        }

        rect.anchoredPosition = basePos;
    }
}