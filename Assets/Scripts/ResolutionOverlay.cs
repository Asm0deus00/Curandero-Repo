using UnityEngine;

public class ResolutionOverlay : MonoBehaviour
{
    void OnGUI()
    {
        float ratio = (float)Screen.width / Screen.height;

        string text =
            $"W: {Screen.width}\n" +
            $"H: {Screen.height}\n" +
            $"Ratio: {ratio:F3}";

        GUI.Label(new Rect(10, 10, 200, 60), text);
    }
}