using UnityEngine;
using UnityEngine;

// Ensure BaseEnemy is accessible or defined

public class Bullet : MonoBehaviour
{
    private float damage = 10f;
    private float speed;
    private readonly float lifetime = 10f;
    private bool parriable = true;
    private bool isParried = false;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        Destroy(gameObject, lifetime);
    }

    public void Launch(Vector3 direction, float speed, float initialDamage)
    {
        this.speed = speed;
        damage = initialDamage;
        if (rb == null) rb = GetComponent<Rigidbody>();
        rb.velocity = direction * speed;
    }

    public void Parry(Vector3 newDirection)
    {
        if (!parriable) return;

        isParried = true;
        rb.velocity = newDirection * (speed * 3f);
        gameObject.layer = LayerMask.NameToLayer("PlayerBullet");
        rb.mass = 0;
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject hit = collision.gameObject;

        if (isParried)
        {
            Debug.Log(hit.gameObject.name);

            if (hit.TryGetComponent(out BaseEnemy enemy))
            {
                Debug.Log("asdasdasdasd");
                PlayerCombat.DamageEnemy(damage, 10f, enemy, collision.contacts[0].point, collision.contacts[0].normal);
            }

            Destroy(gameObject);
            return;
        }

        if (hit.CompareTag("Player"))
        {
            ApplyDamageToPlayer();
            Destroy(gameObject);
            return;
        }

        if (!hit.CompareTag("Enemy"))
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

    public bool GetParriable() => parriable;
}