using UnityEngine;
using TMPro;

public class HitMarker : MonoBehaviour
{
    public TMP_Text text;

    private Transform cam;
    private float timer;
    public float lifetime = 1.2f;
    private bool isActive;

    public Vector3 startScale = Vector3.one * 2f;
    public Vector3 midScale = Vector3.one * 3f;
    public Vector3 endScale = Vector3.one * 1f;
    public float scaleUpTime = 0.5f;
    public float holdTime = 0.3f;

    public float distanceScaleFactor = 0.2f;
    public float minDistanceScale = 0.8f;
    public float maxDistanceScale = 8f;

    private void OnEnable()
    {
        cam = PlayerCamera.Instance.transform;
        transform.localScale = startScale;
        timer = 0f;
        isActive = true;
    }

    private void Update()
    {
        if (!isActive) return;

        timer += Time.deltaTime;
        if (timer >= lifetime)
        {
            isActive = false;
            gameObject.SetActive(false);
            return;
        }

        float t = timer / lifetime;
        float scaleUpEnd = scaleUpTime / lifetime;
        float holdEnd = scaleUpEnd + (holdTime / lifetime);

        Vector3 baseScale;
        if (t < scaleUpEnd)
        {
            float s = t / scaleUpEnd;
            s *= s * s;
            baseScale = Vector3.Lerp(startScale, midScale, s);
        }
        else if (t < holdEnd)
        {
            baseScale = midScale;
        }
        else
        {
            float s = (t - holdEnd) / (1f - holdEnd);
            s *= s * s;
            baseScale = Vector3.Lerp(midScale, endScale, s);
        }

        float dist = Vector3.Distance(transform.position, cam.position);
        float distanceScale = Mathf.Clamp(dist * distanceScaleFactor, minDistanceScale, maxDistanceScale);

        transform.localScale = baseScale * distanceScale;

        Vector3 forward = (transform.position - cam.position).normalized;
        if (forward.sqrMagnitude > 0f) transform.rotation = Quaternion.LookRotation(forward);
    }

    public void ShowDamage(float damage, Color color)
    {
        text.text = $"{damage:F0}";
        text.color = color;
    }

    public void ShowText(string message, Color color)
    {
        text.text = message;
        text.color = color;
    }
}