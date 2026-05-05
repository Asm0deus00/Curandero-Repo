using UnityEngine;
using System.Collections;

public class TitleController : MonoBehaviour
{
    public Transform jawTop;
    public Transform jawBottom;
    public Transform title;

    [Header("Idle")]
    public float idleJawAmplitude = 0.04f;
    public float idleJawSpeed = 1.5f;

    public float idleScaleAmplitude = 0.015f;
    public float idleScaleSpeed = 1.2f;

    [Header("Hover")]
    public float hoverScale = 1.1f;
    public float hoverJawOpen = 0.12f;

    [Header("Click")]
    public float clickJawClose = 0.2f;
    public float clickDelay = 0.4f;

    public float smoothSpeed = 8f;

    private Vector3 topStart;
    private Vector3 bottomStart;
    private Vector3 baseScale;

    private bool isHovering = false;
    private bool isClicked = false;

    void Start()
    {
        topStart = jawTop.localPosition;
        bottomStart = jawBottom.localPosition;
        baseScale = title.localScale;
    }

    // Reset state every time this object is enabled (e.g. when returning to title).
    void OnEnable()
    {
        isClicked = false;
        isHovering = false;
    }

    void Update()
    {
        float targetJaw;
        float targetScale;

        if (isClicked)
        {
            // CLICK → force close
            targetJaw = -clickJawClose;
            targetScale = 1f;
        }
        else if (isHovering)
        {
            // HOVER → FULL override (no idle at all)
            targetJaw = hoverJawOpen;
            targetScale = hoverScale;
        }
        else
        {
            // IDLE only when not hovering
            float time = Time.time;

            float idleJaw = Mathf.Sin(time * idleJawSpeed) * idleJawAmplitude;
            float idleScale = 1 + Mathf.Sin(time * idleScaleSpeed) * idleScaleAmplitude;

            targetJaw = idleJaw;
            targetScale = idleScale;
        }

        // Apply movement cleanly (no mixing)
        Vector3 topTarget = topStart + new Vector3(0, targetJaw, 0);
        Vector3 bottomTarget = bottomStart + new Vector3(0, -targetJaw, 0);

        jawTop.localPosition = Vector3.Lerp(
            jawTop.localPosition,
            topTarget,
            Time.deltaTime * smoothSpeed
        );

        jawBottom.localPosition = Vector3.Lerp(
            jawBottom.localPosition,
            bottomTarget,
            Time.deltaTime * smoothSpeed
        );

        title.localScale = Vector3.Lerp(
            title.localScale,
            baseScale * targetScale,
            Time.deltaTime * smoothSpeed
        );
    }

    void OnMouseEnter()
    {
        if (!isClicked)
            isHovering = true;
    }

    void OnMouseExit()
    {
        if (!isClicked)
            isHovering = false;
    }

    void OnMouseDown()
    {
        if (!isClicked)
            StartCoroutine(ClickSequence());
    }

    IEnumerator ClickSequence()
    {
        isClicked = true;
        isHovering = false;

        yield return new WaitForSeconds(clickDelay);

        FindObjectOfType<TitleManager>().StartGame();

        // Reset after handing off to TitleManager.
        // The game object will be deactivated by NightManager shortly after,
        // but resetting here ensures clean state if StartGame ever fails.
        isClicked = false;
    }
}