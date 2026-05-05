using UnityEngine;

public class ClickablePart : MonoBehaviour
{
    private PatientWiggle wiggle;

    void Start()
    {
        wiggle = FindObjectOfType<PatientWiggle>();

        if (wiggle == null)
            Debug.LogError("NO WIGGLE FOUND on: " + gameObject.name);
    }

    public void OnClicked()
    {
        Debug.Log("Clicked: " + gameObject.name);

        if (wiggle != null)
        {
            wiggle.PlayWiggle(transform);
        }
        else
        {
            Debug.LogError("Wiggle is NULL on click: " + gameObject.name);
        }
    }
}