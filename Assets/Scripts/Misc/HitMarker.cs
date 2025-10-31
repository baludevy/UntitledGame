using UnityEngine;
using TMPro;

public class HitMarker : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    private Transform cam;
    private float timer;
    [SerializeField] private float lifetime = 1.2f;
    private float rLifetime;
    private bool isActive;

    [SerializeField] private Vector3 startScale = Vector3.one * 2f;
    [SerializeField] private Vector3 midScale = Vector3.one * 3f;
    [SerializeField] private Vector3 endScale = Vector3.one * 1f;
    [SerializeField] private float scaleUpTime = 0.5f;
    [SerializeField] private float holdTime = 0.3f;

    [SerializeField] private float distanceScaleFactor = 0.2f;
    [SerializeField] private float minDistanceScale = 0.8f;
    [SerializeField] private float maxDistanceScale = 8f;

    [SerializeField] private float floatAmount = 0.5f;
    private Vector3 startPos;
    private Transform textTransform;

    private void OnEnable()
    {
        cam = PlayerCamera.Instance.transform;
        textTransform = text.transform;
        rLifetime = lifetime + Random.Range(0f, 0.3f);
        textTransform.localScale = startScale;
        timer = 0f;
        isActive = true;
        startPos = transform.position;
    }

    private void LateUpdate()
    {
        if (!isActive) return;

        timer += Time.deltaTime;
        if (timer >= rLifetime)
        {
            isActive = false;
            gameObject.SetActive(false);
            return;
        }

        float t = timer / rLifetime;
        float scaleUpEnd = scaleUpTime / rLifetime;
        float holdEnd = scaleUpEnd + (holdTime / rLifetime);

        Vector3 baseScale;
        float heightOffset;

        if (t < scaleUpEnd)
        {
            float s = t / scaleUpEnd;
            s = 1f - Mathf.Pow(1f - s, 3f);
            baseScale = Vector3.Lerp(startScale, midScale, s);
            heightOffset = Mathf.Lerp(0f, floatAmount, s);
        }
        else if (t < holdEnd)
        {
            baseScale = midScale;
            heightOffset = floatAmount;
        }
        else
        {
            float s = (t - holdEnd) / (1f - holdEnd);
            s = s * s * s;
            baseScale = Vector3.Lerp(midScale, endScale, s);
            heightOffset = Mathf.Lerp(floatAmount, 0f, s);
        }

        float dist = Vector3.Distance(transform.position, cam.position);
        float distanceScale = Mathf.Clamp(dist * distanceScaleFactor, minDistanceScale, maxDistanceScale);
        Vector3 targetScale = baseScale * distanceScale;
        textTransform.localScale = Vector3.Lerp(textTransform.localScale, targetScale, Time.deltaTime * 20f);

        transform.position = startPos + Vector3.up * heightOffset;
        transform.rotation = Quaternion.LookRotation(transform.position - cam.position, Vector3.up);
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