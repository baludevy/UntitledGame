using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class BaseMineable : MonoBehaviour, IMineable, IDamageable
{
    [Header("Data")] 
    
    [SerializeField] private MineableData data;
    
    private float currentHealth;
    private Vector3 originalScale;

    public string Name
    {
        get => data.Name;
        set => data.Name = value;
    }

    public string Description
    {
        get => data.Description;
        set => data.Description = value;
    }

    public ToolType CanBeMinedWith => data.CanBeMinedWith;
    public DroppedItem DropPrefab => data.DropPrefab;
    public int MinDropAmount => data.MinDropAmount;
    public int MaxDropAmount => data.MaxDropAmount;
    
    public int Sound => data.Sound;

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

    public bool canBeMined;
    
    private void Start()
    {
        originalScale = transform.localScale;
        currentHealth = MaxHealth;
        canBeMined = true;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        StopAllCoroutines();
        StartCoroutine(HitAnimation());

        if (currentHealth <= 0 && DropPrefab != null)
            DropLoot();
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

    public void DropLoot()
    {
        int amount = Random.Range(MinDropAmount, MaxDropAmount);
        DroppedItem dropped = Instantiate(DropPrefab, transform.position + Vector3.up * 1.5f, Quaternion.identity);

        if (dropped.itemData is ToolItem toolData)
        {
            dropped.Initialize(new ToolInstance(toolData, amount), false);
        }
        else
        {
            dropped.Initialize(new ItemInstance(dropped.itemData, amount), true);
        }

        Destroy(gameObject);
    }
}