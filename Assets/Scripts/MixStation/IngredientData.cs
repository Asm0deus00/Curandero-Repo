using UnityEngine;

public enum Intensity
{
    Low = 1,
    Medium = 2,
    High = 3
}

[System.Serializable]
public class IngredientData
{
    public string ingredientName;

    public SymptomTag symptomType;

    public Intensity intensity;
}