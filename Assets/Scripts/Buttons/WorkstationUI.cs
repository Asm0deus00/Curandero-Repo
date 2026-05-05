using UnityEngine;

public class WorkstationUI : MonoBehaviour
{
    public SimpleCameraMove cam;

    public Transform patientView;
    public Transform mixView;
    public Transform ritualView;

    public void GoToMix()
    {
        cam.MoveTo(mixView);
    }

    public void GoToRitual()
    {
        cam.MoveTo(ritualView);
    }

    public void GoToPatient()
    {
        cam.MoveTo(patientView);
    }
}