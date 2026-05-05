using UnityEngine;

public class DialogueController : MonoBehaviour
{
    public TypewriterEffect typewriter;

    public void ShowPatient(PatientData patient)
    {
        if (typewriter == null)
        {
            Debug.LogError("Typewriter not assigned!");
            return;
        }

        string text = BuildColoredDialogue(patient);
        typewriter.StartTyping(text);
    }

    public void HideDialogue()
    {
        if (typewriter == null) return;

        typewriter.StopTyping();
        typewriter.ClearText();
    }

    // Builds the dialogue string with colored symptom keywords.
    // Each symptom is replaced independently on the original string to avoid
    // a later replacement corrupting a <color> tag inserted by an earlier one.
    string BuildColoredDialogue(PatientData patient)
    {
        string text = patient.dialogue;

        foreach (var s in patient.symptoms)
        {
            string keyword = GetKeyword(s.tag);
            if (string.IsNullOrEmpty(keyword)) continue;

            // Skip if this keyword is not present in the dialogue — avoids
            // a silent no-op that could hide a data authoring mistake in debug.
            if (!text.Contains(keyword))
            {
                Debug.LogWarning($"Symptom keyword '{keyword}' not found in patient dialogue.");
                continue;
            }

            string color = GetColor(s.tag);
            string colored = $"<color={color}>{keyword}</color>";

            // Replace only the plain (un-tagged) occurrences of the keyword
            // by anchoring the replacement to text that is NOT already inside
            // a color tag. Simple approach: replace the first untagged match.
            text = ReplaceFirstUntagged(text, keyword, colored);
        }

        return text;
    }

    // Replaces the first occurrence of 'keyword' that is not already preceded
    // by a '<color' tag, to avoid double-wrapping on repeated dialogue builds.
    string ReplaceFirstUntagged(string text, string keyword, string replacement)
    {
        int index = text.IndexOf(keyword);
        while (index >= 0)
        {
            // Check whether this occurrence is already inside a color tag
            int tagStart = text.LastIndexOf("<color", index);
            int tagEnd   = text.LastIndexOf("</color>", index);

            bool alreadyTagged = tagStart >= 0 && tagStart > tagEnd;

            if (!alreadyTagged)
            {
                return text.Substring(0, index) + replacement + text.Substring(index + keyword.Length);
            }

            index = text.IndexOf(keyword, index + 1);
        }

        return text;
    }

    string GetKeyword(SymptomTag tag)
    {
        switch (tag)
        {
            case SymptomTag.RED:    return "fuego";
            case SymptomTag.BLUE:   return "frío";
            case SymptomTag.GREEN:  return "estómago";
            case SymptomTag.YELLOW: return "espíritu";
            case SymptomTag.BLACK:  return "sombra";
        }

        return "";
    }

    string GetColor(SymptomTag tag)
    {
        switch (tag)
        {
            case SymptomTag.RED:    return "#bc1e1e";
            case SymptomTag.BLUE:   return "#1763ae";
            case SymptomTag.GREEN:  return "#62a90b";
            case SymptomTag.YELLOW: return "#ba720d";
            case SymptomTag.BLACK:  return "#5f009b";
        }

        return "#FFFFFF";
    }
}