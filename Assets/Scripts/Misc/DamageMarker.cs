using UnityEngine;
using TMPro;

public class DamageMarker : MonoBehaviour
{
    public TMP_Text text;
    private float lifetime = 2f;
    private float floatSpeed = 1f;
    private Color startColor;
    private float timer;
    private float cameraOffset = 0.4f;

    private void Start()
    {
        startColor = text.color;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        Transform playerCamera = PlayerCamera.Instance.transform;

        transform.position += Vector3.up * floatSpeed * Time.deltaTime;

        Vector3 dir = (playerCamera.transform.position - transform.position).normalized;
        transform.position -= dir * -cameraOffset * Time.deltaTime;

        float fade = 1f - (timer / lifetime);
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