using UnityEngine;

public class TitleManager : MonoBehaviour
{
    public WorkstationUI uiController;
    public GameManager gameManager;
    public NightManager nightManager;

    public void StartGame()
    {
        uiController.GoToPatient();
        nightManager.StartNight();
    }
}