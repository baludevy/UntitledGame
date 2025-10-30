using UnityEngine;

public abstract class BaseEnemy : MonoBehaviour, IDamageable
{
    public EnemyData data;

    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;

    public float MaxHealth
    {
        get => data.MaxHealth;
        set { }
    }

    public float CurrentHealth
    {
        get => currentHealth;
        set => currentHealth = Mathf.Clamp(value, 0, MaxHealth);
    }

    public void TakeDamage(float damage, bool crit)
    {
        Debug.Log($"{data.Name} took {damage}");
    }
}