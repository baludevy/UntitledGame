using UnityEngine;
using UnityEngine.UI;

public class BaseEnemy : MonoBehaviour, IDamageable, IEnemy
{
    public EnemyData data;

    private Rigidbody rb;
    private Transform player;
    [SerializeField] private GameObject damageableInfo;
    [SerializeField] private Image healthBar;
    public float moveSpeed = 3f;

    // Object avoidance settings
    [Header("Object Avoidance")]
    [SerializeField] private float avoidanceDistance = 2f;
    [SerializeField] private float avoidanceForce = 5f;
    [SerializeField] private LayerMask obstacleLayerMask = -1; // Adjust to layers you want to avoid (e.g., walls, not enemies/players)

    private float knockbackTimer;
    private readonly float knockbackDuration = 0.2f;

    private bool flash;
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

        Vector3 directionToPlayer = player.position - transform.position;
        directionToPlayer.y = 0;
        if (directionToPlayer.sqrMagnitude < 0.01f) return;

        // Object avoidance: Check for obstacles and apply avoidance force if needed
        Vector3 avoidanceForceVector = Vector3.zero;
        if (PerformObjectAvoidance(ref avoidanceForceVector))
        {
            rb.AddForce(avoidanceForceVector, ForceMode.Acceleration);
        }

        // Pursuit movement
        Vector3 targetVelocity = directionToPlayer.normalized * moveSpeed;
        Vector3 current = rb.velocity;
        Vector3 change = new Vector3(targetVelocity.x - current.x, 0, targetVelocity.z - current.z);
        rb.AddForce(change * 6f, ForceMode.Acceleration);
        transform.rotation = Quaternion.LookRotation(directionToPlayer.normalized);
    }

    private bool PerformObjectAvoidance(ref Vector3 avoidanceForceVector)
    {
        // Cast a ray forward in the movement direction
        Vector3 forwardDirection = transform.forward;
        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, forwardDirection, out RaycastHit hit, avoidanceDistance, obstacleLayerMask))
        {
            // Ignore hits on enemies or player
            if (hit.collider.CompareTag("Enemy") || hit.collider.CompareTag("Player"))
            {
                return false;
            }

            // Apply avoidance force based on hit normal (perpendicular to surface for smoother avoidance)
            Vector3 avoidanceDirection = Vector3.ProjectOnPlane(-hit.normal, Vector3.up).normalized;
            avoidanceForceVector = avoidanceDirection * avoidanceForce;
            return true;
        }

        // Optional: Add side raycasts for better anticipation (e.g., left and right)
        float sideAngle = 45f;
        Vector3 leftDirection = Quaternion.Euler(0, -sideAngle, 0) * forwardDirection;
        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, leftDirection, out RaycastHit leftHit, avoidanceDistance, obstacleLayerMask) &&
            !leftHit.collider.CompareTag("Enemy") && !leftHit.collider.CompareTag("Player"))
        {
            Vector3 leftAvoidance = Vector3.ProjectOnPlane(-leftHit.normal, Vector3.up).normalized * avoidanceForce * 0.5f;
            avoidanceForceVector += leftAvoidance;
        }

        Vector3 rightDirection = Quaternion.Euler(0, sideAngle, 0) * forwardDirection;
        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, rightDirection, out RaycastHit rightHit, avoidanceDistance, obstacleLayerMask) &&
            !rightHit.collider.CompareTag("Enemy") && !rightHit.collider.CompareTag("Player"))
        {
            Vector3 rightAvoidance = Vector3.ProjectOnPlane(-rightHit.normal, Vector3.up).normalized * avoidanceForce * 0.5f;
            avoidanceForceVector += rightAvoidance;
        }

        return avoidanceForceVector != Vector3.zero;
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
        Debug.Log("a");
        rb.AddForce(Vector3.up * 1.75f, ForceMode.VelocityChange);
    }

    public void ApplyKnockback(Vector3 direction, float force)
    {
        direction.y = 0;
        rb.AddForce(direction.normalized * force, ForceMode.VelocityChange);
        knockbackTimer = knockbackDuration;
    }

    public void TakeDamage(float damage, bool doFlash = true)
    {
        flash = true;
        currentHealth -= damage;
        healthBar.fillAmount = currentHealth / MaxHealth;
        if (currentHealth <= 0) Destroy(gameObject);
    }

    private void Update()
    {
        if (flash)
        {
            flash = false;
            MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
            Effects.Flash(renderers);
        }
    }
    
    public void ShowCanvas()
    {
        damageableInfo.SetActive(true);
    }
    
    public void HideCanvas()
    {
        damageableInfo.SetActive(false);
    }

    public Rigidbody GetRigidbody() => rb;
}