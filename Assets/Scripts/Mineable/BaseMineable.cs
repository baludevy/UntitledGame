using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class BaseMineable : MonoBehaviour, IMineable, IDamageable
{
    [Header("Data")] [SerializeField] private MineableData data;

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
    public int MinDropAmount => data.MinDropAmount;
    public int MaxDropAmount => data.MaxDropAmount;
    
    public ItemData DroppedItem => data.DroppedItem;

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

        if (currentHealth <= 0)
            DropLoot();
    }

    private IEnumerator HitAnimation()
    {
        Vector3 start = originalScale;
        Vector3 target = start * 0.85f;

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
        ItemInstance droppedItem = new ItemInstance(DroppedItem, Random.Range(MinDropAmount, MaxDropAmount));
        PlayerInventory.Instance.AddItem(droppedItem);
        
        Destroy(gameObject);
    }
}