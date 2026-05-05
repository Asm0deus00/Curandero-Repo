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

    string BuildColoredDialogue(PatientData patient)
    {
        string text = patient.dialogue;

        foreach (var s in patient.symptoms)
        {
            string keyword = GetKeyword(s.tag);
            string color = GetColor(s.tag);

            text = text.Replace(
                keyword,
                $"<color={color}>{keyword}</color>"
            );
        }

        return text;
    }

    string GetKeyword(SymptomTag tag)
    {
        switch (tag)
        {
            case SymptomTag.RED: return "fuego";
            case SymptomTag.BLUE: return "frío";
            case SymptomTag.GREEN: return "estómago";
            case SymptomTag.YELLOW: return "espíritu";
            case SymptomTag.BLACK: return "sombra";
        }

        return "";
    }

    string GetColor(SymptomTag tag)
    {
        switch (tag)
        {
            case SymptomTag.RED: return "#bc1e1e";
            case SymptomTag.BLUE: return "#1763ae";
            case SymptomTag.GREEN: return "#62a90b";
            case SymptomTag.YELLOW: return "#ba720d";
            case SymptomTag.BLACK: return "#5f009b";
        }

        return "#FFFFFF";
    }
}