using System;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class BaseMineable : MonoBehaviour, IMineable, IDamageable
{
    [Header("Data")] [SerializeField] private MineableData data;

    private float currentHealth;
    private Vector3 originalScale;

    private bool flash;
    private Color flashColor;

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
    public AudioClip Sound => data.Sound;

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

    private void Awake()
    {
        currentHealth = MaxHealth;
        canBeMined = true;
        originalScale = transform.localScale;
    }

    private void Update()
    {
        if (flash)
        {
            flash = false;
            MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
            foreach (var r in renderers) StartCoroutine(Flash(r));
        }
    }

    private IEnumerator Flash(MeshRenderer r)
    {
        Color[] originalColors = new Color[r.materials.Length];
        for (int i = 0; i < r.materials.Length; i++) originalColors[i] = r.materials[i].color;

        for (int i = 0; i < r.materials.Length; i++) r.materials[i].color = flashColor;
        for (int i = 0; i < 3; i++) yield return null;
        for (int i = 0; i < r.materials.Length; i++) r.materials[i].color = originalColors[i];
    }

    public void TakeDamage(float damage, bool crit)
    {
        currentHealth -= damage;

        flashColor = crit ? Color.yellow : Color.white;
        flash = true;

        StopAllCoroutines();
        StartCoroutine(HitAnimation());

        if (Sound != null)
            AudioManager.Play(Sound, transform.position, 0.8f, 1.2f);
        if (currentHealth <= 0)
            DropLoot();
    }

    public IEnumerator HitAnimation()
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
        if (DroppedItem?.floorPrefab != null)
        {
            int amount = Random.Range(MinDropAmount, MaxDropAmount);
            DroppedItem dropped =
                Instantiate(DroppedItem.floorPrefab, transform.position + Vector3.up * 1.5f, transform.rotation)
                    .GetComponent<DroppedItem>();

            if (dropped.itemData is ToolData toolData)
            {
                dropped.Initialize(new ToolInstance(toolData, amount), false);
            }
            else if (dropped.itemData is PlaceableData placeableData)
            {
                dropped.Initialize(new PlaceableInstance(placeableData, amount), false);
            }
            else
            {
                dropped.Initialize(new ItemInstance(dropped.itemData, amount), true);
            }
        }

        Destroy(gameObject);
    }
}