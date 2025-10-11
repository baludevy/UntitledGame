using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageMarker : MonoBehaviour
{
    public TMP_Text damageText;
    private float floatHeight = 0.5f;
    private float floatDuration = 0.5f;
    private float displayDuration = 1f;
    private Vector3 startPosition;
    private Color startColor;

    public void Show(int damage)
    {
        damageText.text = damage.ToString();
        startPosition = transform.position;
        startColor = damageText.color;

        StartCoroutine(FloatAndFade());
    }

    private IEnumerator FloatAndFade()
    {
        float elapsed = 0f;
        Vector3 endPosition = startPosition + Vector3.up * floatHeight;

        while (elapsed < floatDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / floatDuration;

            transform.position = Vector3.Lerp(startPosition, endPosition, t);

            if (PlayerCamera.Instance != null && PlayerCamera.Instance.transform != null)
            {
                
                // ai
                transform.LookAt(2 * transform.position - PlayerCamera.Instance.transform.position);
            }

            yield return null;
        }
        
        yield return new WaitForSeconds(displayDuration);
        
        elapsed = 0f;
        float fadeDuration = 0.5f;
        Vector3 fadeStartPosition = transform.position;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;

            Color newColor = startColor;
            newColor.a = Mathf.Lerp(1f, 0f, t);
            damageText.color = newColor;

            if (PlayerCamera.Instance != null && PlayerCamera.Instance.transform != null)
            {
                transform.LookAt(2 * transform.position - PlayerCamera.Instance.transform.position);
            }

            yield return null;
        }

        Destroy(gameObject);
    }
}