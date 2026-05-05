using UnityEngine;

public class CursorController : MonoBehaviour
{
    public Texture2D idleCursor;
    public Texture2D clickCursor;

    [Header("Hotspot (click point)")]
    public Vector2 hotspot = Vector2.zero;

    void Start()
    {
        SetIdle();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SetClick();
        }

        if (Input.GetMouseButtonUp(0))
        {
            SetIdle();
        }
    }

    void SetIdle()
    {
        Cursor.SetCursor(idleCursor, hotspot, CursorMode.Auto);
    }

    void SetClick()
    {
        Cursor.SetCursor(clickCursor, hotspot, CursorMode.Auto);
    }
}