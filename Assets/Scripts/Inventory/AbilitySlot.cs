using UnityEngine;
using UnityEngine.UI;

public class AbilitySlot : MonoBehaviour {
    [SerializeField] private Image fillImage;

    public void SetCooldown(float remaining, float total) {
        float t = (total <= 0f) ? 0f : Mathf.Clamp01(remaining / total);
        if (fillImage != null) fillImage.fillAmount = t;
    }

    public void SetReady() {
        if (fillImage != null) fillImage.fillAmount = 0f;
    }
}