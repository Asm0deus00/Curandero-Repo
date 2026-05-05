using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PatientWiggle : MonoBehaviour
{
    [Header("Wiggle Settings")]
    [Range(0.01f, 0.2f)]
    public float amplitude = 0.05f;

    [Range(2f, 10f)]
    public float frequency = 5f;

    [Range(0.1f, 1f)]
    public float duration = 0.4f;

    [Header("Per Part Multipliers")]
    public float bodyMultiplier = 0.4f;
    public float headMultiplier = 0.7f;
    public float featureMultiplier = 1f;

    // 🔴 Prevent stacking
    private Dictionary<Transform, Coroutine> activeWiggles = new Dictionary<Transform, Coroutine>();

    public void PlayWiggle(Transform target)
    {
        if (target == null) return;

        // Stop previous wiggle on this target
        if (activeWiggles.ContainsKey(target))
        {
            StopCoroutine(activeWiggles[target]);
            activeWiggles.Remove(target);
        }

        Coroutine c = StartCoroutine(WiggleRoutine(target));
        activeWiggles[target] = c;
    }
    

    IEnumerator WiggleRoutine(Transform target)
    {
        Vector3 startPos = target.localPosition;

        float time = 0f;

        float multiplier = GetMultiplier(target.name);

        while (time < duration)
        {
            time += Time.deltaTime;

            float t = time / duration;
            float damping = 1f - t;

            float offset = Mathf.Sin(time * frequency * Mathf.PI * 2f)
                         * amplitude
                         * multiplier
                         * damping;

            Vector3 current = target.localPosition;

            target.localPosition = new Vector3(
                startPos.x + offset,
                current.y, // preserve idle Y
                current.z
            );

            yield return null;
        }

        // ✅ SOFT RETURN (only X, never touch Y)
        float returnTime = 0f;
        float returnDuration = 0.1f;

        while (returnTime < returnDuration)
        {
            returnTime += Time.deltaTime;
            float t = returnTime / returnDuration;

            Vector3 current = target.localPosition;

            float newX = Mathf.Lerp(current.x, startPos.x, t);

            target.localPosition = new Vector3(
                newX,
                current.y, // keep idle
                current.z
            );

            yield return null;
        }

        // 🔴 FINAL FIX — DO NOT TOUCH Y
        Vector3 final = target.localPosition;
        target.localPosition = new Vector3(startPos.x, final.y, final.z);

        activeWiggles.Remove(target);
    }

    float GetMultiplier(string name)
    {
        if (name.Contains("Body"))
            return bodyMultiplier;

        if (name.Contains("Head"))
            return headMultiplier;

        return featureMultiplier;
    }
}