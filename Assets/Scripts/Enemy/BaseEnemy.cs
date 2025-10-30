using System;
using UnityEngine;

public class BaseEnemy : MonoBehaviour, IDamageable
{
    public EnemyData data;

    private Rigidbody rb;
    private Transform player;
    public float moveSpeed = 3f;

    private float knockbackTimer;
    private readonly float knockbackDuration = 0.2f;

    #region damage stuff idk

    public float MaxHealth
    {
        get => data.MaxHealth;
        set { }
    }

    private float currentHealth;

    public float CurrentHealth
    {
        get => currentHealth;
        set => currentHealth = Mathf.Clamp(value, 0, MaxHealth);
    }

    #endregion

    private void Start()
    {
        CurrentHealth = MaxHealth;

        rb = GetComponent<Rigidbody>();
        player = PlayerMovement.Instance.transform;

        EnemyController.Instance?.RegisterEnemy(this);
    }

    private void OnDestroy()
    {
        EnemyController.Instance?.UnregisterEnemy(this);
    }

    public void Tick()
    {
        if (player == null) return;

        if (knockbackTimer > 0)
        {
            knockbackTimer -= Time.fixedDeltaTime;
            return;
        }

        Vector3 distanceToPlayer = (player.position - transform.position);
        distanceToPlayer.y = 0;

        if (distanceToPlayer.sqrMagnitude < 0.01f) return;

        Vector3 targetVelocity = distanceToPlayer.normalized * moveSpeed;
        Vector3 current = rb.velocity;
        Vector3 change = new Vector3(
            targetVelocity.x - current.x,
            0,
            targetVelocity.z - current.z
        );

        rb.AddForce(change * 6f, ForceMode.Acceleration);
        transform.rotation = Quaternion.LookRotation(distanceToPlayer.normalized);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy")) return;

        foreach (ContactPoint contact in collision.contacts)
        {
            if (Vector3.Dot(contact.normal, Vector3.up) < 0.3)
            {
                Climb();
                break;
            }
        }
    }

    private void Climb()
    {
        rb.AddForce(Vector3.up * 0.8f, ForceMode.VelocityChange);
    }

    public void ApplyKnockback(Vector3 direction, float force)
    {
        direction.y = 0;
        rb.AddForce(direction.normalized * force, ForceMode.VelocityChange);
        knockbackTimer = knockbackDuration;
    }

    public void TakeDamage(float damage, bool crit)
    {
        Debug.Log($"{data.Name} took {damage}");
    }

    public Rigidbody GetRigidbody() => rb;
}