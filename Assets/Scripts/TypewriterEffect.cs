using UnityEngine;
using TMPro;
using System.Collections;

public class TypewriterEffect : MonoBehaviour
{
    public TextMeshProUGUI textUI;

    [Header("Typing")]
    public float speed = 40f;

    [Header("Animation")]
    public float baseAmplitude = 2f;
    public float baseFrequency = 2f;

    private float time;

    // 🔴 STATE TRACKING
    public bool IsTyping { get; private set; }

    // ----------------------------
    public void StartTyping(string text)
    {
        StopAllCoroutines();

        // 🔴 ENSURE TEXT IS VISIBLE
        if (textUI != null)
            textUI.gameObject.SetActive(true);

        textUI.text = text;
        textUI.ForceMeshUpdate();
        textUI.maxVisibleCharacters = 0;

        IsTyping = true;

        StartCoroutine(TypeRoutine());
    }

    // ----------------------------
    IEnumerator TypeRoutine()
    {
        int total = textUI.textInfo.characterCount;
        float t = 0;

        while (textUI.maxVisibleCharacters < total)
        {
            t += Time.deltaTime * speed;
            textUI.maxVisibleCharacters = Mathf.FloorToInt(t);
            yield return null;
        }

        // 🔴 FINISHED TYPING
        IsTyping = false;
    }

    // ----------------------------
    public void StopTyping()
    {
        StopAllCoroutines();
        IsTyping = false;
    }

    // ----------------------------
    public void ClearText()
    {
        if (textUI == null) return;

        textUI.text = "";
        textUI.maxVisibleCharacters = 0;
        textUI.ForceMeshUpdate();

        // 🔴 FULLY HIDE TEXT OBJECT
        textUI.gameObject.SetActive(false);
    }

    // ----------------------------
    void Update()
    {
        AnimateCharacters();
    }

    // ----------------------------
    void AnimateCharacters()
    {
        if (textUI == null || !textUI.gameObject.activeSelf)
            return;

        textUI.ForceMeshUpdate();
        TMP_TextInfo textInfo = textUI.textInfo;

        time += Time.deltaTime;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            if (i >= textUI.maxVisibleCharacters)
                continue;

            var charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible)
                continue;

            int vertexIndex = charInfo.vertexIndex;
            int materialIndex = charInfo.materialReferenceIndex;

            Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

            Color32 c = charInfo.color;

            float intensity = 1f;

            if (c.r > 200) intensity = 1.2f;
            if (c.g > 150) intensity = 1.1f;
            if (c.b > 200) intensity = 1.15f;

            float amp = baseAmplitude * intensity;

            Vector3 offset = new Vector3(
                Mathf.Sin(time * baseFrequency + i) * 0.5f * intensity,
                Mathf.Sin(time * baseFrequency + i * 0.3f) * amp,
                0
            );

            // 🔴 RED → aggressive shake
            if (c.r > 200 && c.g < 100)
                offset += Random.insideUnitSphere * (1.5f * intensity);

            // 🔵 BLUE → vertical wave
            else if (c.b > 200)
                offset.y = Mathf.Sin(time * 5f + i) * (2f * intensity);

            // 🟢 GREEN → horizontal instability
            else if (c.g > 150)
                offset.x = Mathf.Sin(time * 3f + i) * (2f * intensity);

            // ⚫ BLACK → chaotic jitter
            else if (c.r < 100 && c.g < 100 && c.b < 100)
                offset += Random.insideUnitSphere * (2f * intensity);

            vertices[vertexIndex + 0] += offset;
            vertices[vertexIndex + 1] += offset;
            vertices[vertexIndex + 2] += offset;
            vertices[vertexIndex + 3] += offset;
        }

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
            textUI.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
        }
    }
}