using UnityEngine;
using System.Collections.Generic;

public class Bowl : MonoBehaviour
{
    public List<BowlItem> items = new List<BowlItem>();

    public Transform dropPoint;

    [Header("Hover")]
    public float hoverScale = 1.1f;
    public float smoothSpeed = 8f;

    [Header("Wiggle Physics")]
    public float wiggleForce = 20f;
    public float damping = 4f;
    public float stiffness = 120f;

    private Vector3 baseScale;
    private bool isHovering = false;

    private float angle = 0f;
    private float angularVelocity = 0f;

    // 🔑 FIX: start ABOVE bowl layer
    private int sortingCounter = 1;

    private SpriteRenderer bowlRenderer;

    void Start()
    {
        baseScale = transform.localScale;

        // 🔑 Get bowl renderer to stack above it
        bowlRenderer = GetComponent<SpriteRenderer>();

        if (bowlRenderer != null)
        {
            sortingCounter = bowlRenderer.sortingOrder + 1;
        }
    }

    void Update()
    {
        HandleScale();
        UpdateWiggle();
    }

    void HandleScale()
    {
        float targetScale = isHovering ? hoverScale : 1f;

        transform.localScale = Vector3.Lerp(
            transform.localScale,
            baseScale * targetScale,
            Time.deltaTime * smoothSpeed
        );
    }

    void UpdateWiggle()
    {
        float acceleration = -stiffness * angle - damping * angularVelocity;

        angularVelocity += acceleration * Time.deltaTime;
        angle += angularVelocity * Time.deltaTime;

        transform.localRotation = Quaternion.Euler(0, 0, angle);
    }

    void OnMouseEnter()
    {
        isHovering = true;
    }

    void OnMouseExit()
    {
        isHovering = false;
    }

    public void AddItem(BowlItem item)
    {
        if (item == null) return;

        items.Add(item);

        item.transform.SetParent(transform);

        Vector3 localOffset = RandomOffset();
        item.transform.localPosition = dropPoint.localPosition + localOffset;

        // 🔑 FIX: robust layering
        SpriteRenderer sr = item.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sortingOrder = sortingCounter;
            sortingCounter++;
        }

        // 🔑 Add energy to wiggle (feels responsive)
        angularVelocity += Random.Range(-wiggleForce, wiggleForce);
    }

    Vector3 RandomOffset()
    {
        return new Vector3(
            Random.Range(-0.2f, 0.2f),
            Random.Range(-0.2f, 0.2f),
            0f
        );
    }

    public Dictionary<SymptomTag, int> GetIntensityMap()
    {
        Dictionary<SymptomTag, int> map = new Dictionary<SymptomTag, int>();

        foreach (var item in items)
        {
            if (item == null || item.data == null)
                continue;

            SymptomTag type = item.data.symptomType;
            int value = (int)item.data.intensity;

            if (!map.ContainsKey(type))
                map[type] = 0;

            map[type] += value;
        }

        return map;
    }

    public void ClearBowl()
    {
        foreach (var item in items)
        {
            if (item != null)
                Destroy(item.gameObject);
        }

        items.Clear();

        // 🔑 Reset properly above bowl
        if (bowlRenderer != null)
            sortingCounter = bowlRenderer.sortingOrder + 1;
        else
            sortingCounter = 1;
    }
}