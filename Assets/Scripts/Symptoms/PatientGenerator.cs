using UnityEngine;
using System.Collections.Generic;

public class PatientGenerator : MonoBehaviour
{
    public int minTags = 1;
    public int maxTags = 3;

    public PatientData GeneratePatient()
    {
        PatientData patient = new PatientData();

        int count = Random.Range(minTags, maxTags + 1);

        for (int i = 0; i < count; i++)
        {
            SymptomTag tag = (SymptomTag)Random.Range(0, 3);

            if (!ContainsTag(patient, tag))
            {
                SymptomInstance instance = new SymptomInstance();
                instance.tag = tag;
                instance.intensity = Random.Range(1, 4); // 1–3

                patient.symptoms.Add(instance);
            }
        }

        patient.dialogue = GenerateDialogue(patient.symptoms);

        return patient;
    }

    bool ContainsTag(PatientData patient, SymptomTag tag)
    {
        foreach (var s in patient.symptoms)
            if (s.tag == tag) return true;

        return false;
    }

    string GenerateDialogue(List<SymptomInstance> symptoms)
    {
        List<string> fragments = new List<string>();

        foreach (var s in symptoms)
        {
            fragments.Add(GetPhrase(s.tag, s.intensity));
        }

        return string.Join(" ", fragments);
    }

    string GetPhrase(SymptomTag tag, int intensity)
{
    string keyword = GetKeyword(tag);

    switch (tag)
    {
        case SymptomTag.RED:
            return GetByIntensity(intensity,
                $"Siento un leve {keyword} en mi cuerpo.",
                $"El {keyword} en mi cuerpo no se detiene.",
                $"Un {keyword} me consume por dentro."
            );

        case SymptomTag.BLUE:
            return GetByIntensity(intensity,
                $"Tengo un poco de {keyword}.",
                $"No dejo de sentir {keyword}.",
                $"El {keyword} me atraviesa los huesos."
            );

        case SymptomTag.GREEN:
            return GetByIntensity(intensity,
                $"Mi {keyword} está algo revuelto.",
                $"Mi {keyword} no se calma.",
                $"Mi {keyword} se retuerce sin control."
            );

        case SymptomTag.YELLOW:
            return GetByIntensity(intensity,
                $"Mi {keyword} se siente débil.",
                $"Mi {keyword} está apagado.",
                $"Siento que mi {keyword} se rompe."
            );

        case SymptomTag.BLACK:
            return GetByIntensity(intensity,
                $"Siento una {keyword} cerca de mí.",
                $"La {keyword} no me deja.",
                $"Una {keyword} me consume."
            );
    }

    return "";
}

string GetByIntensity(int intensity, string low, string mid, string high)
{
    if (intensity == 1) return low;
    if (intensity == 2) return mid;
    return high;
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

    string GetRandom(string[] options, int intensity)
    {
        // intensity maps to stronger phrases
        int index = Mathf.Clamp(intensity - 1, 0, options.Length - 1);
        return options[index];
    }
}