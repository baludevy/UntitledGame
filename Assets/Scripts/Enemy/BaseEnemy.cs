using System;
using System.Collections;
using UnityEngine;

public class BaseEnemy : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth;
    [SerializeField] private int damage;
    [SerializeField] private float attackCooldown;
    [SerializeField] private float moveSpeed = 5;

    private float currentHealth;
    private Vector3 futurePos;
    private Rigidbody rb;

    public float MaxHealth
    {
        get => maxHealth;
        set => maxHealth = value;
    }

    public float CurrentHealth
    {
        get => currentHealth;
        set => currentHealth = Mathf.Clamp(value, 0, MaxHealth);
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    
    private void FixedUpdate()
    {
        if (!PlayerMovement.Instance) return;

        Vector3 playerPos = PlayerMovement.Instance.transform.position;
        Vector3 dir = playerPos - transform.position;
        dir.y = 0;

        dir.Normalize();
        rb.MovePosition(rb.position + dir * moveSpeed * Time.fixedDeltaTime);
    }

    public void TakeDamage(float amount)
    {
        
    }
}