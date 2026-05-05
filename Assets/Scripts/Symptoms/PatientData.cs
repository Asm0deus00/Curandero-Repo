using System.Collections.Generic;

[System.Serializable]
public class SymptomInstance
{
    public SymptomTag tag;
    public int intensity; // 1 = low, 2 = medium, 3 = high
}

public class PatientData
{
    public List<SymptomInstance> symptoms = new List<SymptomInstance>();
    public string dialogue;
}