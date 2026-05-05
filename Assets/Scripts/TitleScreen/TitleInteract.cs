using UnityEngine;

public class TitleInteract : MonoBehaviour
{
    public Transform jawTop;
    public Transform jawBottom;
    public Transform title;

    [Header("Hover")]
    public float hoverScale = 1.1f;
    public float jawOpenAmount = 0.15f;
    public float speed = 6f;

    private Vector3 titleBaseScale;
    private Vector3 topStart;
    private Vector3 bottomStart;

    private bool isHovering = false;

    void Start()
    {
        titleBaseScale = title.localScale;
        topStart = jawTop.localPosition;
        bottomStart = jawBottom.localPosition;
    }

    void OnMouseEnter()
    {
        isHovering = true;
    }

    void OnMouseExit()
    {
        isHovering = false;
    }

    void OnMouseDown()
    {
        FindObjectOfType<TitleManager>().StartGame();
    }

    void Update()
    {
        // TARGET VALUES
        float targetScale = isHovering ? hoverScale : 1f;
        float targetJaw = isHovering ? jawOpenAmount : 0f;

        // SMOOTH SCALE
        title.localScale = Vector3.Lerp(
            title.localScale,
            titleBaseScale * targetScale,
            Time.deltaTime * speed
        );

        // SMOOTH JAW OPEN
        Vector3 topTarget = topStart + new Vector3(0, targetJaw, 0);
        Vector3 bottomTarget = bottomStart + new Vector3(0, -targetJaw, 0);

        jawTop.localPosition = Vector3.Lerp(jawTop.localPosition, topTarget, Time.deltaTime * speed);
        jawBottom.localPosition = Vector3.Lerp(jawBottom.localPosition, bottomTarget, Time.deltaTime * speed);
    }
}