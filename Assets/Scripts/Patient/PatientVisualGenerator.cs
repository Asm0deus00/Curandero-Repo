using UnityEngine;

public class PatientVisualGenerator : MonoBehaviour
{
    [Header("Body Parts")]
    public Sprite[] bodies;
    public Sprite[] noses;
    public Sprite[] mouths;

    [Header("Eyes")]
    public Sprite[] leftEyeSprites;
    public Sprite[] rightEyeSprites;

    [Header("Head Prefabs")]
    public GameObject[] headPrefabs;
    public Transform headParent;

    private GameObject currentHead;

    [Header("Renderers (Assigned at runtime)")]
    public SpriteRenderer bodyRenderer;

    private SpriteRenderer leftEyeRenderer;
    private SpriteRenderer rightEyeRenderer;
    private SpriteRenderer noseRenderer;
    private SpriteRenderer mouthRenderer;

    [Header("Anchors (from prefab)")]
    private Transform leftEyeAnchor;
    private Transform rightEyeAnchor;
    private Transform noseAnchor;
    private Transform mouthAnchor;

    [Header("Settings")]
    [Range(0f, 1f)]
    public float sameEyesChance = 0.5f;

/*
    void Start()
    {
        GeneratePatient();
    }
*/
    public void GeneratePatient()
    {
        // BODY
        bodyRenderer.sprite = GetRandom(bodies);
        SetupClickable(bodyRenderer.gameObject);

        // HEAD
        GenerateHead();

        // SAFETY CHECK
        if (leftEyeRenderer == null || rightEyeRenderer == null || noseRenderer == null || mouthRenderer == null)
        {
            Debug.LogError("Missing facial feature references. Check prefab structure.");
            return;
        }

        // EYES
        GenerateEyes();

        // FACE
        GenerateFace();

        // POSITION
        SnapFeaturesToAnchors();

        // CLICKABLE FEATURES
        SetupClickable(leftEyeRenderer.gameObject);
        SetupClickable(rightEyeRenderer.gameObject);
        SetupClickable(noseRenderer.gameObject);
        SetupClickable(mouthRenderer.gameObject);

        // IDLE ANIMATION
        PatientIdleAnimation idle = GetComponent<PatientIdleAnimation>();
        if (idle != null)
            idle.RefreshReferences(currentHead);
    }

    void GenerateHead()
    {
        if (headPrefabs == null || headPrefabs.Length == 0)
            return;

        // Destroy old head
        if (currentHead != null)
            Destroy(currentHead);

        // Instantiate new head
        GameObject prefab = headPrefabs[Random.Range(0, headPrefabs.Length)];
        currentHead = Instantiate(prefab, headParent);

        // ✅ Head root now has SpriteRenderer
        SetupClickable(currentHead);

        // ANCHORS
        leftEyeAnchor = FindChild("EyeAnchor_L");
        rightEyeAnchor = FindChild("EyeAnchor_R");
        noseAnchor = FindChild("NoseAnchor");
        mouthAnchor = FindChild("MouthAnchor");

        // FEATURES
        leftEyeRenderer = FindRenderer("LeftEye");
        rightEyeRenderer = FindRenderer("RightEye");
        noseRenderer = FindRenderer("Nose");
        mouthRenderer = FindRenderer("Mouth");
    }

    void GenerateEyes()
    {
        if (leftEyeSprites.Length == 0 || rightEyeSprites.Length == 0)
            return;

        bool sameEyes = Random.value < sameEyesChance;

        if (sameEyes)
        {
            int index = Random.Range(0, Mathf.Min(leftEyeSprites.Length, rightEyeSprites.Length));

            leftEyeRenderer.sprite = leftEyeSprites[index];
            rightEyeRenderer.sprite = rightEyeSprites[index];
        }
        else
        {
            leftEyeRenderer.sprite = GetRandom(leftEyeSprites);
            rightEyeRenderer.sprite = GetRandom(rightEyeSprites);
        }
    }

    void GenerateFace()
    {
        noseRenderer.sprite = GetRandom(noses);
        mouthRenderer.sprite = GetRandom(mouths);
    }

    void SnapFeaturesToAnchors()
    {
        if (leftEyeAnchor != null)
            leftEyeRenderer.transform.position = leftEyeAnchor.position;

        if (rightEyeAnchor != null)
            rightEyeRenderer.transform.position = rightEyeAnchor.position;

        if (noseAnchor != null)
            noseRenderer.transform.position = noseAnchor.position;

        if (mouthAnchor != null)
            mouthRenderer.transform.position = mouthAnchor.position;
    }

    // 🔴 CLICKABLE SETUP
    void SetupClickable(GameObject obj)
    {
        if (obj == null) return;

        Collider2D col = obj.GetComponent<Collider2D>();

        if (col == null)
            col = obj.AddComponent<PolygonCollider2D>();

        col.isTrigger = false;

        if (obj.GetComponent<ClickablePart>() == null)
            obj.AddComponent<ClickablePart>();
    }

    // 🔴 SAFE FIND HELPERS

    Transform FindChild(string name)
    {
        Transform t = currentHead.transform.Find(name);

        if (t == null)
            Debug.LogError(name + " NOT FOUND in head prefab");

        return t;
    }

    SpriteRenderer FindRenderer(string name)
    {
        Transform t = currentHead.transform.Find(name);

        if (t == null)
        {
            Debug.LogError(name + " NOT FOUND in head prefab");
            return null;
        }

        SpriteRenderer sr = t.GetComponent<SpriteRenderer>();

        if (sr == null)
            Debug.LogError(name + " has no SpriteRenderer");

        return sr;
    }

    Sprite GetRandom(Sprite[] array)
    {
        if (array == null || array.Length == 0)
            return null;

        return array[Random.Range(0, array.Length)];
    }
}