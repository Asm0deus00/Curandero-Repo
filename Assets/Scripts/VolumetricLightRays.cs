using UnityEngine;
using System.Collections.Generic;

public class VolumetricLightRays : MonoBehaviour
{
    [Header("Setup")]
    public GameObject lightColumnPrefab;
    public int maxRays = 8;

    [Header("Beam Axis")]
    public Vector2 beamDirection = new Vector2(-1f, -0.3f);
    public float beamLength = 3f;
    public Vector2 axisOffset = Vector2.zero; // <-- move axis in inspector

    [Header("Orbit")]
    public float orbitRadius = 0.08f;
    public float orbitSpeed = 0.6f;

    [Header("Flicker")]
    public float flickerSpeed = 1.2f;
    public float flickerAmount = 0.2f;

    [Header("Spawn / Despawn")]
    public float spawnInterval = 0.6f;
    public float lifeTime = 3.5f;

    private List<RayData> rays = new List<RayData>();
    private float spawnTimer;

    void Start()
    {
        beamDirection.Normalize();
    }

    void Update()
    {
        float time = Time.time;

        HandleSpawning(time);
        AnimateRays(time);
    }

    void HandleSpawning(float time)
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval && rays.Count < maxRays)
        {
            spawnTimer = 0f;
            SpawnRay(time);
        }
    }

    void SpawnRay(float time)
    {
        GameObject obj = Instantiate(lightColumnPrefab, transform);

        float t = Random.Range(0f, beamLength);

        RayData data = new RayData();
        data.transform = obj.transform;

        // Base position along axis
        data.baseOffset = axisOffset + beamDirection * t;

        // Perpendicular vector
        data.perp = new Vector2(-beamDirection.y, beamDirection.x);

        data.phase = Random.Range(0f, 10f);
        data.birthTime = time;
        data.lifeTime = lifeTime * Random.Range(0.7f, 1.3f);

        // Store original alpha
        data.baseAlpha = obj.GetComponent<SpriteRenderer>().color.a;

        // 🔥 Align column with beam direction
        float angle = Mathf.Atan2(beamDirection.y, beamDirection.x) * Mathf.Rad2Deg;
        obj.transform.localRotation = Quaternion.Euler(0, 0, angle);

        rays.Add(data);
    }

    void AnimateRays(float time)
    {
        for (int i = rays.Count - 1; i >= 0; i--)
        {
            RayData r = rays[i];

            float age = time - r.birthTime;
            float life01 = age / r.lifeTime;

            if (life01 >= 1f)
            {
                Destroy(r.transform.gameObject);
                rays.RemoveAt(i);
                continue;
            }

            float angle = time * orbitSpeed + r.phase;

            Vector2 offset =
                r.perp * Mathf.Sin(angle) * orbitRadius +
                beamDirection * Mathf.Cos(angle) * orbitRadius * 0.2f;

            Vector2 finalPos = r.baseOffset + offset;

            r.transform.localPosition = finalPos;

            // Fade + flicker
            float fade = Mathf.Sin(life01 * Mathf.PI);
            float flicker = 1f + Mathf.Sin(time * flickerSpeed + r.phase) * flickerAmount;

            SpriteRenderer sr = r.transform.GetComponent<SpriteRenderer>();

            if (sr != null)
            {
                Color c = sr.color;
                c.a = Mathf.Clamp01(r.baseAlpha * fade * flicker);
                sr.color = c;
            }
        }
    }

    // 🟡 GIZMO (visual axis in Scene view)
    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;

        Vector2 dir = beamDirection.normalized;
        Vector2 start = (Vector2)transform.position + axisOffset;
        Vector2 end = start + dir * beamLength;

        Gizmos.DrawLine(start, end);

        // small markers
        Gizmos.DrawSphere(start, 0.05f);
        Gizmos.DrawSphere(end, 0.05f);
    }

    class RayData
    {
        public Transform transform;
        public Vector2 baseOffset;
        public Vector2 perp;

        public float phase;
        public float birthTime;
        public float lifeTime;

        public float baseAlpha;
    }
}