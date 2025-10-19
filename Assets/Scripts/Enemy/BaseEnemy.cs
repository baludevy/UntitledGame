using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class BaseEnemy : MonoBehaviour, IDamageable
{
    [SerializeField] private int maxHealth;
    [SerializeField] private int damage;
    [SerializeField] private float attackCooldown;
    [SerializeField] private int attackRange;
    private int currentHealth;
    private float attackTimer;

    public int MaxHealth
    {
        get => maxHealth;
        set => maxHealth = value;
    }

    public int CurrentHealth
    {
        get => currentHealth;
        set => currentHealth = Mathf.Clamp(value, 0, MaxHealth);
    }

    private Vector3 originalScale;
    private Vector3 weaponOriginalScale;

    private NavMeshAgent agent;

    [SerializeField] private GameObject weapon;
    [SerializeField] private GameObject bulletPrefab;

    private void Start()
    {
        currentHealth = MaxHealth;
        originalScale = transform.localScale;
        weaponOriginalScale = weapon.transform.localScale;

        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = attackRange;
    }

    private void Update()
    {
        if (attackTimer > 0)
            attackTimer -= Time.deltaTime;

        if (!PlayerMovement.Instance) return;

        agent.destination = PlayerMovement.Instance.transform.position;

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (attackTimer <= 0)
                Attack();
        }
    }

    private void Attack()
    {
        Vector3 playerPos = PlayerMovement.Instance.transform.position;
        Vector3 playerVel = PlayerMovement.Instance.GetComponent<Rigidbody>().linearVelocity;

        Vector3 startPos = weapon.transform.position;
        Vector3 targetPos = playerPos;
        targetPos.y -= 0.5f;

        // done by ai ----------------
        Vector3 horizontal = new Vector3(targetPos.x - startPos.x, 0f, targetPos.z - startPos.z);
        float distance = horizontal.magnitude;
        float yOffset = targetPos.y - startPos.y;

        float arc = Mathf.Max(1f, 2f * (distance / 12.5f));
        float h = Mathf.Max(arc, yOffset + 0.25f);

        float g = Mathf.Abs(Physics.gravity.y);
        float tUp = Mathf.Sqrt(2f * h / g);
        float tDown = Mathf.Sqrt(2f * Mathf.Max(0.0001f, (h - yOffset)) / g);
        float time = Mathf.Max(0.01f, tUp + tDown);

        float predictionFactor = 0.7f;
        Vector3 futurePos = playerPos + playerVel * (time * predictionFactor);

        targetPos = futurePos;

        Vector3 lookDir = futurePos - transform.position;
        lookDir.y = 0f;
        if (lookDir.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.LookRotation(lookDir);

        horizontal = new Vector3(targetPos.x - startPos.x, 0f, targetPos.z - startPos.z);
        distance = horizontal.magnitude;
        yOffset = targetPos.y - startPos.y;

        arc = Mathf.Max(1f, 2f * (distance / 12.5f));
        h = Mathf.Max(arc, yOffset + 0.25f);

        tUp = Mathf.Sqrt(2f * h / g);
        tDown = Mathf.Sqrt(2f * Mathf.Max(0.0001f, (h - yOffset)) / g);
        time = Mathf.Max(0.01f, tUp + tDown);

        Vector3 velocity = horizontal / time;
        velocity.y = Mathf.Sqrt(2f * g * h);
        // ---------------------------

        GameObject bullet = Instantiate(bulletPrefab, startPos, Quaternion.identity);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.linearVelocity = velocity;

        StartCoroutine(ReloadWeapon());
        attackTimer = attackCooldown;
    }


    public void TakeDamage(int amount)
    {
        StopAllCoroutines();
        StartCoroutine(HitAnimation());
        currentHealth -= amount;

        if (currentHealth <= 0)
            Destroy(gameObject);
    }

    private IEnumerator ReloadWeapon()
    {
        weapon.transform.localScale = Vector3.zero;
        weapon.gameObject.SetActive(true);

        Vector3 startScale = Vector3.zero;
        Vector3 endScale = weaponOriginalScale;
        float t = 0f;

        while (t < attackCooldown)
        {
            t += Time.deltaTime;
            float progress = t / attackCooldown;
            weapon.transform.localScale = Vector3.Lerp(startScale, endScale, progress);
            yield return null;
        }

        weapon.transform.localScale = endScale;
    }

    private IEnumerator HitAnimation()
    {
        Vector3 start = originalScale;
        Vector3 target = start * 0.9f;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / 0.2f;
            float eased = t < 0.5f ? 4f * t * t * t : 1f - Mathf.Pow(-2f * t + 2f, 3f) / 2f;
            transform.localScale = Vector3.Lerp(start, target, eased);
            yield return null;
        }

        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / 0.2f;
            float eased = t < 0.5f ? 4f * t * t * t : 1f - Mathf.Pow(-2f * t + 2f, 3f) / 2f;
            transform.localScale = Vector3.Lerp(target, start, eased);
            yield return null;
        }

        transform.localScale = start;
    }
}