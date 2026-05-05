using UnityEngine;
using UnityEngine.UI;

public class UICursor : MonoBehaviour
{
    public RectTransform cursorTransform;
    public Image cursorImage;

    public Sprite idleSprite;
    public Sprite clickSprite;

    void Start()
    {
        Cursor.visible = false;
    }

    void Update()
    {
        // Follow mouse
        cursorTransform.position = Input.mousePosition;

        // Change sprite on click
        if (Input.GetMouseButton(0))
            cursorImage.sprite = clickSprite;
        else
            cursorImage.sprite = idleSprite;
    }
}