using UnityEngine;
using TMPro;

public class DamageMarker : MonoBehaviour
{
    public TMP_Text text;
    [SerializeField] private float lifetime = 3f;
    [SerializeField] private float startFloatSpeed = 5f;
    [SerializeField] private float endFloatSpeed = 0f;
    [SerializeField] private float maxHeightOffset = 1f;
    private Color startColor;
    private float timer;
    private readonly float cameraOffset = 0.4f;
    private float initialY;

    private void Start()
    {
        startColor = text.color;
        initialY = transform.position.y;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        float t = timer / lifetime;
        
        float easedT = t * t;
        float currentSpeed = Mathf.Lerp(startFloatSpeed, endFloatSpeed, easedT);

        transform.position += Vector3.up * currentSpeed * Time.deltaTime;
        
        float clampedY = Mathf.Clamp(transform.position.y, initialY, initialY + maxHeightOffset);
        transform.position = new Vector3(transform.position.x, clampedY, transform.position.z);

        Transform playerCamera = PlayerCamera.Instance.transform;

        Vector3 dir = (playerCamera.transform.position - transform.position).normalized;
        transform.position -= dir * -cameraOffset * Time.deltaTime;

        float fade = 1f - t;
        text.color = new Color(startColor.r, startColor.g, startColor.b, fade);

        transform.LookAt(playerCamera.transform);
        transform.Rotate(0, 180, 0);

        if (timer >= lifetime) Destroy(gameObject);
    }

    public void ShowDamage(float newDamage)
    {
        float currentDamage = float.Parse(text.text);
        currentDamage += newDamage;
        text.text = $"{currentDamage:F0}";
    }
}