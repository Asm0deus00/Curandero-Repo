using UnityEngine;
using TMPro;

public class NightTimerUI : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public NightManager nightManager;

    void Update()
    {
        if (nightManager == null || !nightManager.IsNightActive())
        {
            timerText.text = "";
            return;
        }

        float time = nightManager.GetRemainingTime();

        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);

        timerText.text = $"{minutes:00}:{seconds:00}";
    }
}