using UnityEngine;
using System.Collections.Generic;

public class MixDelivery : MonoBehaviour
{
    public Bowl bowl;

    public void Deliver()
    {
        var mix = bowl.GetIntensityMap();

        foreach (var kvp in mix)
        {
            Debug.Log(kvp.Key + " → " + kvp.Value);
        }

        Evaluate(mix);

        bowl.ClearBowl();

        //FindObjectOfType<UIController>().GoToPatient();
    }

    void Evaluate(Dictionary<SymptomTag, int> mix)
    {
        // Example placeholder
        foreach (var kvp in mix)
        {
            if (kvp.Value < 2)
                Debug.Log(kvp.Key + ": Too weak");

            else if (kvp.Value <= 3)
                Debug.Log(kvp.Key + ": Good");

            else
                Debug.Log(kvp.Key + ": Too strong");
        }
    }
}