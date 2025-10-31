using UnityEngine;

public class BaseEnemy : MonoBehaviour, IDamageable, IEnemy
{
    public EnemyData data;

    private Rigidbody rb;
    private Transform player;
    public float moveSpeed = 3f;

    private float knockbackTimer;
    private readonly float knockbackDuration = 0.2f;

    private bool flash;
    private Color flashColor;

    public string Name => data.name;
    public string Description => data.description;
    public Element Element => data.element;

    private float currentHealth;

    public float MaxHealth
    {
        get => data.maxHealth;
        set => data.maxHealth = value;
    }

    public float CurrentHealth
    {
        get => currentHealth;
        set => currentHealth = Mathf.Clamp(value, 0, MaxHealth);
    }

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

        Vector3 distanceToPlayer = player.position - transform.position;
        distanceToPlayer.y = 0;
        if (distanceToPlayer.sqrMagnitude < 0.01f) return;

        Vector3 targetVelocity = distanceToPlayer.normalized * moveSpeed;
        Vector3 current = rb.velocity;
        Vector3 change = new Vector3(targetVelocity.x - current.x, 0, targetVelocity.z - current.z);
        rb.AddForce(change * 6f, ForceMode.Acceleration);
        transform.rotation = Quaternion.LookRotation(distanceToPlayer.normalized);

        DetectWall();
    }

    private void DetectWall()
    {
        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, transform.forward, out RaycastHit hit, 1f))
        {
            if (!hit.collider.CompareTag("Enemy") && !hit.collider.CompareTag("Player"))
                Climb();
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

    public void TakeDamage(float damage, Color color, bool doFlash = true)
    {
        flashColor = color;
        flash = true;
        currentHealth -= damage;
        if (currentHealth <= 0) Destroy(gameObject);
    }

    private void Update()
    {
        if (flash)
        {
            flash = false;
            MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
            StartCoroutine(Effects.Flash(renderers, flashColor));
        }
    }

    public Rigidbody GetRigidbody() => rb;
}