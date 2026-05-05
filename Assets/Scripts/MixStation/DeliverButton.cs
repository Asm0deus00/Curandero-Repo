using UnityEngine;
using System.Collections.Generic;

public class DeliverButton : MonoBehaviour
{
    public Bowl bowl;
    public GameManager gameManager;

    public void OnDeliver()
    {
        Debug.Log("DELIVER CLICKED");

        if (bowl == null || gameManager == null)
        {
            Debug.LogError("DeliverButton: Missing references!");
            return;
        }

        if (bowl.items.Count == 0)
        {
            Debug.Log("Bowl is empty");
            return;
        }

        Dictionary<SymptomTag, int> mix = bowl.GetIntensityMap();

        gameManager.ProcessTreatment(mix);

        bowl.ClearBowl();
    }
}