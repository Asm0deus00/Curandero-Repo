using UnityEngine;

public class LockResolution : MonoBehaviour
{
    void Awake()
    {
        // Force your working resolution
        Screen.SetResolution(1920, 1002, FullScreenMode.Windowed);

        // Keep cursor inside the window
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    void OnApplicationFocus(bool hasFocus)
    {
        // Re-apply confinement when returning to the game
        if (hasFocus)
        {
            Cursor.lockState = CursorLockMode.Confined;
        }
    }
}