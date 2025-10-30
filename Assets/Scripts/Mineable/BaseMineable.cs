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
        get => data.mineableName;
        set => data.mineableName = value;
    }

    public string Description
    {
        get => data.description;
        set => data.description = value;
    }

    public ToolType CanBeMinedWith => data.canBeMinedWith;
    public int MinDropAmount => data.minDropAmount;
    public int MaxDropAmount => data.maxDropAmount;
    public ItemData DroppedItem => data.droppedItem;
    public AudioClip Sound => data.sound;

    public float MaxHealth
    {
        get => data.maxHealth;
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
            StartCoroutine(Effects.Flash(renderers, flashColor));
        }
    }

    public void TakeDamage(float damage, bool crit)
    {
        currentHealth -= damage;

        flashColor = crit ? Color.yellow : Color.white;
        flash = true;

        StopAllCoroutines();
        StartCoroutine(Effects.HitAnimation(originalScale, transform));

        if (Sound != null)
            AudioManager.Play(Sound, transform.position, 0.8f, 1.2f);
        if (currentHealth <= 0)
            DropLoot();
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