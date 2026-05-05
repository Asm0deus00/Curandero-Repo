using UnityEngine;

public class DraggableItem : MonoBehaviour
{
    public IngredientData data;

    [Header("Uses")]
    public int maxUses = 5;
    public int remainingUses = 5;

    [Header("Visual")]
    public GameObject dragVisualPrefab;
    public GameObject emptyOverlay;

    [Header("Hover")]
    public float hoverScale = 1.08f;
    public float breatheAmplitude = 0.03f;
    public float breatheSpeed = 2f;

    [Header("Drag Physics")]
    public float tiltStrength = 0.5f;     // how much it tilts from movement
    public float maxTilt = 25f;           // clamp rotation
    public float followSmooth = 15f;      // position smoothing
    public float rotationSmooth = 12f;    // rotation smoothing

    private GameObject currentDrag;
    private Camera cam;

    private Vector3 baseScale;
    private bool isHovering = false;

    // drag physics
    private Vector3 dragVelocity;
    private Vector3 lastMousePos;

    void Start()
    {
        cam = Camera.main;
        remainingUses = maxUses;
        baseScale = transform.localScale;

        UpdateOverlay();
    }

    void Update()
    {
        HandleHover();

        if (currentDrag == null) return;

        Vector3 mouse = cam.ScreenToWorldPoint(Input.mousePosition);
        mouse.z = 0;

        // --- POSITION SMOOTH FOLLOW ---
        currentDrag.transform.position = Vector3.Lerp(
            currentDrag.transform.position,
            mouse,
            Time.deltaTime * followSmooth
        );

        // --- VELOCITY CALCULATION ---
        dragVelocity = (mouse - lastMousePos) / Time.deltaTime;
        lastMousePos = mouse;

        HandleDragTilt();
    }

    void OnMouseEnter()
    {
        if (remainingUses > 0)
            isHovering = true;
    }

    void OnMouseExit()
    {
        isHovering = false;
    }

    void OnMouseDown()
    {
        if (remainingUses <= 0) return;

        currentDrag = Instantiate(dragVisualPrefab);
        currentDrag.transform.position = transform.position;

        lastMousePos = cam.ScreenToWorldPoint(Input.mousePosition);
    }

    void OnMouseUp()
    {
        if (currentDrag == null) return;

        Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        Collider2D hit = Physics2D.OverlapPoint(mousePos);

        if (hit != null && hit.GetComponent<Bowl>() != null)
        {
            Bowl bowl = hit.GetComponent<Bowl>();

            BowlItem item = currentDrag.GetComponent<BowlItem>();
            item.SetData(data);

            bowl.AddItem(item);

            remainingUses--;
            UpdateOverlay();
        }
        else
        {
            Destroy(currentDrag);
        }

        currentDrag = null;
    }

    void HandleHover()
    {
        float targetScale = 1f;

        if (isHovering && currentDrag == null)
        {
            float breathe = 1 + Mathf.Sin(Time.time * breatheSpeed) * breatheAmplitude;
            targetScale = hoverScale * breathe;
        }

        transform.localScale = Vector3.Lerp(
            transform.localScale,
            baseScale * targetScale,
            Time.deltaTime * 10f
        );
    }

    void HandleDragTilt()
    {
        // use horizontal velocity for tilt
        float tilt = -dragVelocity.x * tiltStrength;

        tilt = Mathf.Clamp(tilt, -maxTilt, maxTilt);

        Quaternion targetRot = Quaternion.Euler(0, 0, tilt);

        currentDrag.transform.rotation = Quaternion.Lerp(
            currentDrag.transform.rotation,
            targetRot,
            Time.deltaTime * rotationSmooth
        );
    }

    void UpdateOverlay()
    {
        if (emptyOverlay != null)
            emptyOverlay.SetActive(remainingUses <= 0);
    }
}