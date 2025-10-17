using UnityEngine;

public class UIBob : MonoBehaviour
{
    public RectTransform uiElement;
    public float multiplier = 0.3f;

    Vector2 defaultPos;

    void Awake()
    {
        defaultPos = uiElement.anchoredPosition;
    }

    void LateUpdate()
    {
        if (PlayerCamera.Instance == null) return;

        Vector3 offset = -PlayerCamera.Instance.bobOffset * multiplier;
        uiElement.anchoredPosition = defaultPos + new Vector2(offset.x, offset.y);
    }
}