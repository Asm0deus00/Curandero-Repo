using UnityEngine;

public class BowlItem : MonoBehaviour
{
    public IngredientData data;

    public void SetData(IngredientData newData)
    {
        data = newData;
    }

    public int GetValue()
    {
        return (int)data.intensity;
    }
}