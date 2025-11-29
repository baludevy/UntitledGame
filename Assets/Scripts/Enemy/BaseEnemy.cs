using UnityEngine;
using UnityEngine.UI;

public class BaseEnemy : MonoBehaviour, IDamageable, IEnemy
{
    public EnemyData data;

    private Rigidbody rb;
    private Transform player;

    [SerializeField] private GameObject damageableInfo;
    [SerializeField] private Image healthBar;

    protected float knockbackTimer;
    private bool flash;
    private float currentHealth;

    public string Name => data.enemyName;
    public string Description => data.description;
    public Element Element => data.element;

    public float MoveSpeed => data.moveSpeed;

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

    protected virtual void Start()
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

    public virtual void Tick()
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

        Vector3 targetVelocity = distanceToPlayer.normalized * MoveSpeed;
        Vector3 current = rb.velocity;
        Vector3 change = new Vector3(
            targetVelocity.x - current.x,
            0,
            targetVelocity.z - current.z
        );

        rb.AddForce(change * 6f, ForceMode.Acceleration);
        transform.rotation = Quaternion.LookRotation(distanceToPlayer.normalized);
    }

    public void DetectWall()
    {
        if (Physics.Raycast(transform.position + Vector3.up, transform.forward, out RaycastHit hit, 1f))
        {
            if (!hit.collider.CompareTag("Enemy") && !hit.collider.CompareTag("Player"))
                Climb();
        }
    }

    private void Climb()
    {
        rb.AddForce(Vector3.up * 1.25f, ForceMode.VelocityChange);
    }

    public void ApplyKnockback(Vector3 direction, float force)
    {
        rb.AddForce(direction.normalized * force, ForceMode.VelocityChange);
        knockbackTimer = 0.2f;
    }

    public void TakeDamage(float damage, bool doFlash = true)
    {
        flash = doFlash;
        CurrentHealth -= damage;

        if (healthBar != null)
            healthBar.fillAmount = CurrentHealth / MaxHealth;

        if (CurrentHealth <= 0)
            Destroy(gameObject);
    }

    private void Update()
    {
        if (!flash) return;
        flash = false;
        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
        Effects.Flash(renderers);
    }

    public void ShowCanvas()
    {
        if (damageableInfo != null) damageableInfo.SetActive(true);
    }

    public void HideCanvas()
    {
        if (damageableInfo != null) damageableInfo.SetActive(false);
    }

    public Rigidbody GetRigidbody() => rb;
}