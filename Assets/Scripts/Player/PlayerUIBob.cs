using System;
using UnityEngine;

public class UIBob : MonoBehaviour
{
    public RectTransform uiElement;
    public float multiplier = 0.3f;

    private Vector3 uiBobOffset;

    private Vector3 defaultPos;

    private void Awake()
    {
        defaultPos = uiElement.localPosition;
    }

    private void LateUpdate()
    {
        if (PlayerCamera.Instance != null)
        {
            uiBobOffset = -PlayerCamera.Instance.bobOffset * multiplier;
            uiElement.localPosition = defaultPos + uiBobOffset;
        }
    }
}