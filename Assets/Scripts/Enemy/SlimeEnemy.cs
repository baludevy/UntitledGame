using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SlimeEnemy : MonoBehaviour, IDamageable, IEnemy {
    public EnemyData data;

    [Header("UI")] [SerializeField] private GameObject damageableInfo;
    [SerializeField] private Image healthBar;

    [Header("Movement")] [SerializeField] private float hopCooldown = 0.6f;
    [SerializeField] private float hopForwardSpeed = 6f;
    [SerializeField] private float hopUpSpeed = 4.5f;

    [Header("Combat")] [SerializeField] private float contactDamage = 10f;
    [SerializeField] private float contactKnockback = 6f;
    [SerializeField] private float contactKnockbackUp = 0.0f;
    [SerializeField] private float damageCooldown = 0.75f;

    [Header("Grounding")] [SerializeField] private LayerMask groundMask;
    [SerializeField] private float groundCheckRadius = 0.25f;
    [SerializeField] private float groundCheckDown = 0.35f;

    [Header("Knockback")] [SerializeField] private float knockbackDuration = 0.2f;

    [Header("Wall Climbing")] [SerializeField]
    private float wallStickForce = 5f;

    [SerializeField] private float maxClimbAngle = 85f;
    [SerializeField] private LayerMask climbableMask;
    [SerializeField] private float climbAssistUpForce = 1.5f;
    [SerializeField] private float climbDetectDistance = 1f;

    private Rigidbody rb;
    private Transform player;

    private float currentHealth;

    private float hopTimer;
    private float knockbackTimer;
    private float damageTimer;

    private bool isGrounded;
    private bool flash;

    private Vector3 surfaceNormal = Vector3.up;

    public string Name => data.name;
    public string Description => data.description;
    public Element Element => data.element;

    public float MaxHealth {
        get => data.maxHealth;
        set => data.maxHealth = value;
    }

    public float CurrentHealth {
        get => currentHealth;
        set => currentHealth = Mathf.Clamp(value, 0, MaxHealth);
    }

    private void Start() {
        CurrentHealth = MaxHealth;
        rb = GetComponent<Rigidbody>();
        player = PlayerMovement.Instance ? PlayerMovement.Instance.transform : null;
        EnemyController.Instance?.RegisterEnemy(this);
    }

    private void OnDestroy() {
        EnemyController.Instance?.UnregisterEnemy(this);
    }

    private void FixedUpdate() {
        Vector3 origin = transform.position + Vector3.up * 0.05f;
        Vector3 checkPos = origin + Vector3.down * groundCheckDown;
        isGrounded = Physics.CheckSphere(checkPos, groundCheckRadius, groundMask, QueryTriggerInteraction.Ignore);
    }

    private void UpdateSurfaceNormal() {
        if (Physics.Raycast(transform.position + Vector3.up * 0.2f, -transform.up, out RaycastHit hit, 1.5f,
                groundMask | climbableMask)) {
            surfaceNormal = hit.normal;
        }
        else {
            surfaceNormal = Vector3.up;
        }
    }

    public void Tick() {
        if (player == null) {
            if (PlayerMovement.Instance) player = PlayerMovement.Instance.transform;
            if (player == null) return;
        }

        if (hopTimer > 0f) hopTimer -= Time.fixedDeltaTime;
        if (knockbackTimer > 0f) {
            knockbackTimer -= Time.fixedDeltaTime;
            return;
        }

        Vector3 toPlayer = player.position - transform.position;
        toPlayer.y = 0f;

        if (toPlayer.sqrMagnitude < 0.0001f) return;

        transform.rotation = Quaternion.LookRotation(toPlayer.normalized);

        UpdateSurfaceNormal();

        float angle = Vector3.Angle(surfaceNormal, Vector3.up);
        bool onWall = angle > 30f;

        if (!isGrounded && !onWall) return;

        if (hopTimer <= 0f) {
            Vector3 flatVel = Vector3.ProjectOnPlane(rb.velocity, surfaceNormal);
            rb.velocity = flatVel;

            Vector3 moveDir = Vector3.ProjectOnPlane(toPlayer.normalized, surfaceNormal).normalized;

            if (onWall) {
                moveDir += surfaceNormal * -wallStickForce * 0.3f;
            }

            rb.AddForce(moveDir * hopForwardSpeed, ForceMode.VelocityChange);

            Vector3 surfaceUp = surfaceNormal;
            rb.AddForce(surfaceUp * hopUpSpeed * (onWall ? 1.2f : 1f), ForceMode.VelocityChange);

            hopTimer = hopCooldown;
        }

        DetectWall();
    }

    private void DetectWall() {
        if (Physics.Raycast(transform.position + Vector3.up * 0.3f, transform.forward, out RaycastHit hit,
                climbDetectDistance)) {
            if (!hit.collider.CompareTag("Enemy") && !hit.collider.CompareTag("Player")) {
                Climb();
            }
        }
    }

    private void Climb() {
        Debug.Log("climb");
        rb.AddForce(Vector3.up * climbAssistUpForce, ForceMode.VelocityChange);
    }

    private void OnCollisionStay(Collision collision) {
        if (!collision.collider.CompareTag("Player")) return;

        if (damageTimer > 0f) {
            damageTimer -= Time.deltaTime;
            return;
        }

        PlayerStatistics.Instance.Health.TakeDamage(contactDamage);

        Vector3 knockbackDirection = (collision.transform.position - transform.position).normalized;

        PlayerMovement playerMov = collision.gameObject.GetComponent<PlayerMovement>();
        if (playerMov != null) {
            playerMov.ApplyKnockback(
                knockbackDirection,
                contactKnockback,
                contactKnockbackUp,
                0.18f
            );
        }

        damageTimer = damageCooldown;
    }

    public void ApplyKnockback(Vector3 direction, float force) {
        direction.y = 0f;
        rb.AddForce(direction.normalized * force, ForceMode.VelocityChange);
        knockbackTimer = knockbackDuration;
    }

    public void TakeDamage(float damage, bool doFlash = true) {
        flash = doFlash;
        currentHealth -= damage;

        if (healthBar) healthBar.fillAmount = currentHealth / MaxHealth;
        if (currentHealth <= 0f) Destroy(gameObject);
    }

    private void Update() {
        if (!flash) return;
        flash = false;

        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
        Effects.Flash(renderers);
    }

    public void ShowCanvas() {
        if (damageableInfo) damageableInfo.SetActive(true);
    }

    public void HideCanvas() {
        if (damageableInfo) damageableInfo.SetActive(false);
    }

    public Rigidbody GetRigidbody() => rb;
}