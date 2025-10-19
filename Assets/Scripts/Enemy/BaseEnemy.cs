using System;
using System.Collections;
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
    private NavMeshAgent agent;

    private void Start()
    {
        currentHealth = MaxHealth;
        originalScale = transform.localScale;
        
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = attackRange;
    }

    private void Update()
    {
        if(attackTimer > 0)
            attackTimer -= Time.deltaTime;
        
        if (!PlayerMovement.Instance) return;
            
        agent.destination = PlayerMovement.Instance.transform.position;

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            if(attackTimer <= 0)
                Attack();
        }
    }

    private void Attack()
    {
        Debug.Log("attack");
        PlayerStatistics.Instance.health -= damage;

        attackTimer += attackCooldown;
    }

    public void TakeDamage(int amount)
    {
        StopAllCoroutines();
        StartCoroutine(HitAnimation());
        currentHealth -= amount;
        
        if(currentHealth <= 0) 
            Destroy(gameObject);
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