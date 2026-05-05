using UnityEngine;

public class PatientIdleAnimation : MonoBehaviour
{
    [Header("References")]
    public Transform body;
    public Transform head;

    private Transform leftEye;
    private Transform rightEye;
    private Transform nose;
    private Transform mouth;

    [Header("Breathing (Body vs Head)")]
    [Range(0f, 0.1f)]
    public float verticalAmplitude = 0.02f;

    [Range(0.1f, 5f)]
    public float verticalSpeed = 1.5f;

    [Range(0f, 2f)]
    public float headDelay = 0.5f;

    [Range(0.5f, 1.5f)]
    public float headSpeedMultiplier = 0.85f;

    [Header("Face Drift (Desynced Motion)")]
    [Range(0f, 0.1f)]
    public float driftAmplitude = 0.01f;

    [Range(0.1f, 5f)]
    public float driftSpeed = 1.2f;

    private Vector3 bodyStartPos;
    private Vector3 headStartPos;

    private Vector3 leftEyeStart;
    private Vector3 rightEyeStart;
    private Vector3 noseStart;
    private Vector3 mouthStart;

    private bool initialized = false;

    void Start()
    {
        if (body != null)
            bodyStartPos = body.localPosition;

        if (head != null)
            headStartPos = head.localPosition;
    }

    void Update()
    {
        if (!initialized) return;

        float time = Time.time;

        AnimateBreathing(time);
        AnimateFaceDrift(time);
    }

    // Called AFTER head is generated
    public void RefreshReferences(GameObject headInstance)
    {
        leftEye = FindDeepChild(headInstance.transform, "LeftEye");
        rightEye = FindDeepChild(headInstance.transform, "RightEye");
        nose = FindDeepChild(headInstance.transform, "Nose");
        mouth = FindDeepChild(headInstance.transform, "Mouth");

        if (leftEye != null) leftEyeStart = leftEye.localPosition;
        if (rightEye != null) rightEyeStart = rightEye.localPosition;
        if (nose != null) noseStart = nose.localPosition;
        if (mouth != null) mouthStart = mouth.localPosition;

        initialized = true;
    }

    void AnimateBreathing(float time)
    {
        if (body == null || head == null) return;

        float bodyOffset = Mathf.Sin(time * verticalSpeed) * verticalAmplitude;

        float headOffset = Mathf.Sin((time * verticalSpeed * headSpeedMultiplier) + headDelay)
                         * verticalAmplitude * 1.2f;

        body.localPosition = bodyStartPos + new Vector3(0, bodyOffset, 0);
        head.localPosition = headStartPos + new Vector3(0, headOffset, 0);
    }

    void AnimateFaceDrift(float time)
    {
        ApplyCircularMotion(leftEye, leftEyeStart, time, 0.3f);
        ApplyCircularMotion(rightEye, rightEyeStart, time, 1.1f);
        ApplyCircularMotion(nose, noseStart, time, 2.2f);
        ApplyCircularMotion(mouth, mouthStart, time, 3.4f);
    }

    void ApplyCircularMotion(Transform target, Vector3 startPos, float time, float offset)
    {
        if (target == null) return;

        float x = Mathf.Cos((time * driftSpeed) + offset) * driftAmplitude;
        float y = Mathf.Sin((time * driftSpeed * 0.9f) + offset) * driftAmplitude;

        target.localPosition = startPos + new Vector3(x, y, 0);
    }

    Transform FindDeepChild(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
                return child;

            Transform result = FindDeepChild(child, name);
            if (result != null)
                return result;
        }
        return null;
    }
}