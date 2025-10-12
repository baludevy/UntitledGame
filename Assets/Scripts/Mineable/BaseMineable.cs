using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMineable : MonoBehaviour, IMineable
{
    [Header("Info")] [SerializeField] private string itemName;
    [SerializeField] private string description;

    public string Name
    {
        get => itemName;
        set => itemName = value;
    }

    public string Description
    {
        get => description;
        set => description = value;
    }

    [Header("Mineable data")]
    
    [SerializeField] private int maxHealth;
    [SerializeField] int currentHealth;

    public int MaxHealth { get; set; }
    public int CurrentHealth
    {
        get => currentHealth;
        set => currentHealth = Mathf.Clamp(value, 0, MaxHealth);
    }

    [Header("Drop")] [SerializeField] protected int minDropAmount;
    [SerializeField] protected int maxDropAmount;
    [SerializeField] protected ToolType canBeMinedWith;
    [SerializeField] protected DroppedItem dropPrefab;

    public ToolType CanBeMinedWith => canBeMinedWith;
    public DroppedItem DropPrefab => dropPrefab;
    public int MinDropAmount => minDropAmount;
    public int MaxDropAmount => maxDropAmount;

    protected virtual void Start()
    {
        currentHealth = maxHealth;
    }

    public virtual void TakeDamage(int damage)
    {
        currentHealth -= damage;
        StopAllCoroutines();
        StartCoroutine(HitAnimation());

        if (currentHealth <= 0)
            DropLoot();
    }

    protected IEnumerator HitAnimation()
    {
        Vector3 start = transform.localScale;
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
    }

    public virtual void DropLoot()
    {
        int amount = UnityEngine.Random.Range(minDropAmount, maxDropAmount);
        DroppedItem dropped = Instantiate(dropPrefab, transform.position, Quaternion.identity);
        dropped.Initialize(new ItemInstance(dropped.itemData, amount));
        Destroy(gameObject);
    }
}