using UnityEngine;

public class Oiiai : MonoBehaviour
{
    public float flyUpSpeed = 20f;
    public float bobAmount = 0.5f;
    public float bobSpeed = 2f;
    public float spinSpeed = 360f;
    public float shakeIntensity = 0.2f;

    private bool spinning;
    private bool shaking;
    private bool flyingUp;
    private Vector3 startPos;
    private Vector3 originalPosition;
    private float shakeTimer;

    public AudioClip oiiai;
    private AudioSource audioSource;
    
    void Start()
    {
        startPos = transform.position;
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    {
        if (spinning && !shaking && !flyingUp)
        {
            transform.position = startPos + Vector3.up * Mathf.Sin(Time.time * bobSpeed) * bobAmount;
            transform.Rotate(Vector3.up * spinSpeed * Time.deltaTime);
        }

        if (shaking)
        {
            float x = Random.Range(-1f, 1f) * shakeIntensity;
            float z = Random.Range(-1f, 1f) * shakeIntensity;
            transform.position = originalPosition + new Vector3(x, 0f, z);
        }

        if (flyingUp)
        {
            transform.position += Vector3.up * flyUpSpeed * Time.deltaTime;
        }
    }

    public void TriggerSpin()
    {
        if (!spinning)
        {
            audioSource.PlayOneShot(oiiai);
            spinning = true;
            originalPosition = transform.position;
            Invoke(nameof(StartShake), 5f);
        }
    }

    private void StartShake()
    {
        shaking = true;
        Invoke(nameof(OiiaiLaunch), 1f);
    }

    public void OiiaiLaunch()
    {
        shaking = false;
        flyingUp = true;
        Invoke(nameof(DestroyOiiai), 1f);
    }

    private void DestroyOiiai()
    {
        Destroy(gameObject);
    }
}