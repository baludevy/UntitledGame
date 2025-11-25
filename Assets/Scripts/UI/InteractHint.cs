using System;
using TMPro;
using UnityEngine;

public class InteractUI : MonoBehaviour
{
    public TMP_Text hintText;
    public TMP_Text keyText;

    private Vector3 targetScale = Vector3.zero;
    private Vector3 velocity;
    private readonly float smoothTime = 0.1f;

    private bool isVisible;

    private void Awake()
    {
        transform.localScale = Vector3.zero;
    }

    private void Update()
    {
        transform.localScale = Vector3.SmoothDamp(transform.localScale, targetScale, ref velocity, smoothTime);
    }

    public void Initialize(string hintText, string keyText)
    {
        this.hintText.text = hintText;
        this.keyText.text = keyText;
        Show();
    }

    private void Show()
    {
        isVisible = true;
        targetScale = Vector3.one;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        if (!isVisible) return;
        isVisible = false;
        targetScale = new Vector3(0.2f, 0.2f, 0.2f);
        Invoke(nameof(DisableSelf), smoothTime * 1.1f);
    }

    private void DisableSelf()
    {
        if (!isVisible)
            gameObject.SetActive(false);
    }
}