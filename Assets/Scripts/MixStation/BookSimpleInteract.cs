using UnityEngine;
using System.Collections;

public class BookSimpleInteract : MonoBehaviour
{
    [Header("References")]
    public Camera cam;
    public LayerMask interactLayer;

    public Transform recipePanel;

    [Header("Panel Positions")]
    public Vector3 hiddenPos;
    public Vector3 shownPos;

    [Header("Animation")]
    public float animationDuration = 0.4f;

    [Header("Hover")]
    public float hoverScale = 1.05f;
    public float hoverSpeed = 8f;

    [Header("Hover Pulse")]
    public float pulseSpeed = 4f;
    public float pulseAmount = 0.015f;

    [Header("Outline")]
    public SpriteRenderer outlineRenderer;

    [Header("Wiggle")]
    public float wiggleAmount = 0.05f;
    public float wiggleDuration = 0.2f;

    private Vector3 baseScale;
    private bool isHovering;
    private bool isOpen;

    private Coroutine panelAnim;

    void Start()
    {
        baseScale = transform.localScale;

        if (cam == null)
            cam = Camera.main;

        if (outlineRenderer != null)
            outlineRenderer.enabled = false;

        // Ensure panel starts hidden
        if (recipePanel != null)
            recipePanel.position = hiddenPos;
    }

    void Update()
    {
        HandleHover();
        HandleClick();
        AnimateScale();
    }

    void HandleHover()
    {
        Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity, interactLayer);

        isHovering = hit.collider != null && hit.collider.gameObject == gameObject;

        if (outlineRenderer != null)
            outlineRenderer.enabled = isHovering;
    }

    void HandleClick()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity, interactLayer);

        if (hit.collider == null) return;

        // CLICK BOOK
        if (hit.collider.gameObject == gameObject)
        {
            StartCoroutine(Wiggle());
            TogglePanel();
            return;
        }

        // CLICK PANEL
        if (recipePanel != null && hit.collider.transform.IsChildOf(recipePanel))
        {
            TogglePanel(false);
        }
    }

    void TogglePanel(bool? forceState = null)
    {
        bool targetState = forceState ?? !isOpen;
        isOpen = targetState;

        if (panelAnim != null)
            StopCoroutine(panelAnim);

        panelAnim = StartCoroutine(AnimatePanelRoutine());
    }

    IEnumerator AnimatePanelRoutine()
    {
        Vector3 start = recipePanel.position;
        Vector3 target = isOpen ? shownPos : hiddenPos;

        float time = 0f;

        while (time < animationDuration)
        {
            time += Time.deltaTime;
            float t = time / animationDuration;

            // Smooth easing (same up/down)
            t = t * t * (3f - 2f * t); // SmoothStep

            recipePanel.position = Vector3.Lerp(start, target, t);

            yield return null;
        }

        recipePanel.position = target;
    }

    void AnimateScale()
    {
        float pulse = isHovering
            ? Mathf.Sin(Time.time * pulseSpeed) * pulseAmount
            : 0f;

        Vector3 target = isHovering
            ? baseScale * (hoverScale + pulse)
            : baseScale;

        transform.localScale = Vector3.Lerp(
            transform.localScale,
            target,
            Time.deltaTime * hoverSpeed
        );
    }

    IEnumerator Wiggle()
    {
        Vector3 startPos = transform.position;

        float time = 0f;

        while (time < wiggleDuration)
        {
            time += Time.deltaTime;

            float offset = Mathf.Sin(time * 40f) * wiggleAmount;

            transform.position = startPos + new Vector3(offset, 0f, 0f);

            yield return null;
        }

        transform.position = startPos;
    }
}