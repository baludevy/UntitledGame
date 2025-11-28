using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage;
    public float lifetime = 5f;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        Destroy(gameObject, lifetime);
    }

    public void Launch(Vector3 direction, float speed, float initialDamage)
    {
        damage = initialDamage;
        if (rb == null) rb = GetComponent<Rigidbody>();
        rb.velocity = direction * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ApplyDamageToPlayer();
            Destroy(gameObject);
        }
        else if (!other.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }

    private void ApplyDamageToPlayer()
    {
        if (PlayerStatistics.Instance != null && PlayerStatistics.Instance.Health != null)
        {
            PlayerStatistics.Instance.Health.TakeDamage(damage);
        }
    }
}