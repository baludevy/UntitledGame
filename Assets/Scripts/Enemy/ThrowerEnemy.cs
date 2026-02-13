using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ThrowerEnemy : MonoBehaviour, IDamageable, IEnemy {
    public EnemyData data;

    [Header("UI")] [SerializeField] private GameObject damageableInfo;
    [SerializeField] private Image healthBar;

    [Header("Movement")] [SerializeField] public float moveSpeed = 3f;

    [Header("Throwing")] [SerializeField] private GameObject weapon;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float attackCooldown = 1.25f;
    [SerializeField] private float attackRange = 10f; // world units
    [SerializeField] private float attackAccuracy = 1f; // 0..1 (1 = full lead)
    [SerializeField] private float minArcHeight = 1f; // minimum arc height
    [SerializeField] private float arcScale = 2f; // how “lobbed” it is
    [SerializeField] private float arcDistanceRef = 12.5f; // matches your old code

    private Rigidbody rb;
    private Transform player;

    private float currentHealth;

    private float knockbackTimer;
    private readonly float knockbackDuration = 0.2f;

    private bool flash;

    private float attackTimer;
    private Vector3 futurePos;

    private Vector3 weaponOriginalScale;

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

        if (weapon) weaponOriginalScale = weapon.transform.localScale;

        EnemyController.Instance?.RegisterEnemy(this);
    }

    private void OnDestroy() {
        EnemyController.Instance?.UnregisterEnemy(this);
    }

    public void Tick() {
        if (player == null) {
            if (PlayerMovement.Instance) player = PlayerMovement.Instance.transform;
            if (player == null) return;
        }

        if (attackTimer > 0f)
            attackTimer -= Time.fixedDeltaTime;

        if (knockbackTimer > 0f) {
            knockbackTimer -= Time.fixedDeltaTime;
            return;
        }

        Vector3 toPlayer = player.position - transform.position;
        toPlayer.y = 0f;
        float sqrDist = toPlayer.sqrMagnitude;
        transform.rotation = Quaternion.LookRotation(toPlayer.normalized);

        if (sqrDist < 0.01f) return;

        float sqrAttackRange = attackRange * attackRange;

        // Move toward player if out of range
        if (sqrDist > sqrAttackRange) {
            Vector3 targetVelocity = toPlayer.normalized * moveSpeed;
            Vector3 current = rb.velocity;
            Vector3 change = new Vector3(targetVelocity.x - current.x, 0f, targetVelocity.z - current.z);
            rb.AddForce(change * 6f, ForceMode.Acceleration);

            return;
        }

        // In range: aim + attack
        UpdateFuturePos();


        if (attackTimer <= 0f)
            Attack();
    }

    private void UpdateFuturePos() {
        if (!weapon) {
            futurePos = player.position;
            return;
        }

        Vector3 playerPos = player.position;

        Rigidbody prb = PlayerMovement.Instance ? PlayerMovement.Instance.GetComponent<Rigidbody>() : null;
        Vector3 playerVel = prb ? prb.velocity : Vector3.zero;

        Vector3 startPos = weapon.transform.position;

        // --- your old predictive-lob timing (ported) ---
        Vector3 toPlayer = playerPos - startPos;
        float distance = new Vector2(toPlayer.x, toPlayer.z).magnitude;
        float yOffset = playerPos.y - startPos.y;

        float arc = Mathf.Max(minArcHeight, arcScale * (distance / arcDistanceRef));
        float h = Mathf.Max(arc, yOffset + 0.25f);

        float g = Mathf.Abs(Physics.gravity.y);
        float tUp = Mathf.Sqrt(2f * h / g);
        float tDown = Mathf.Sqrt(2f * Mathf.Max(0.0001f, (h - yOffset)) / g);
        float time = Mathf.Max(0.01f, tUp + tDown);

        float predictionFactor = attackAccuracy;
        futurePos = playerPos + playerVel * (time * predictionFactor);
        // ---------------------------------------------
    }

    private void Attack() {
        if (!weapon || !bulletPrefab) return;

        Vector3 startPos = weapon.transform.position;
        Vector3 targetPos = futurePos;

        // --- your old ballistic velocity solve (ported) ---
        Vector3 horizontal = new Vector3(targetPos.x - startPos.x, 0f, targetPos.z - startPos.z);
        float distance = horizontal.magnitude;
        float yOffset = targetPos.y - startPos.y;

        float arc = Mathf.Max(minArcHeight, arcScale * (distance / arcDistanceRef));
        float h = Mathf.Max(arc, yOffset + 0.25f);

        float g = Mathf.Abs(Physics.gravity.y);
        float tUp = Mathf.Sqrt(2f * h / g);
        float tDown = Mathf.Sqrt(2f * Mathf.Max(0.0001f, (h - yOffset)) / g);
        float time = Mathf.Max(0.01f, tUp + tDown);

        Vector3 velocity = horizontal / time;
        velocity.y = Mathf.Sqrt(2f * g * h);
        // -----------------------------------------------

        GameObject bullet = Instantiate(bulletPrefab, startPos, Quaternion.identity);
        if (bullet.TryGetComponent(out Rigidbody brb)) {
            brb.useGravity = true;
            brb.velocity = velocity;
        }

        if (weapon) StartCoroutine(ReloadWeapon());
        attackTimer = attackCooldown;
    }

    public void DetectWall() {
        if (Physics.Raycast(transform.position + Vector3.up, transform.forward, out RaycastHit hit, 1f)) {
            if (!hit.collider.CompareTag("Enemy") && !hit.collider.CompareTag("Player"))
                Climb();
        }
    }

    private void Climb() {
        rb.AddForce(Vector3.up * 1.5f, ForceMode.VelocityChange);
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

    private IEnumerator ReloadWeapon() {
        weapon.transform.localScale = Vector3.zero;
        weapon.gameObject.SetActive(true);

        Vector3 startScale = Vector3.zero;
        Vector3 endScale = weaponOriginalScale;
        float t = 0f;

        while (t < attackCooldown) {
            t += Time.deltaTime;
            float progress = t / attackCooldown;
            weapon.transform.localScale = Vector3.Lerp(startScale, endScale, progress);
            yield return null;
        }

        weapon.transform.localScale = endScale;
    }
}