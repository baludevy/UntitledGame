using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIManager : MonoBehaviour
{
    [Header("HUD")]
    [Header("Stats")]
    [SerializeField]
    private TMP_Text staminaText;

    [SerializeField] private Image staminaBar;

    [SerializeField] private TMP_Text healthText;
    [SerializeField] private Image healthBar;

    [Header("Inventory")]
    [SerializeField] public CanvasGroup inventoryHolder;
    public Transform inventorySlotsHolder;
    [NonSerialized] public List<InventorySlot> inventorySlots;

    private PlayerStatistics statistics;

    public static PlayerUIManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        inventorySlots = new List<InventorySlot>();

        for (int i = 0; i < inventorySlotsHolder.childCount; i++)
        {
            Transform row = inventorySlotsHolder.GetChild(i);
            for (int j = 0; j < row.childCount; j++)
            {
                InventorySlot slot = row.GetChild(j).GetComponent<InventorySlot>();

                slot.row = i;
                slot.col = j;

                inventorySlots.Add(slot);
            }
        }
    }

    private void Update()
    {
        if (statistics == null)
            statistics = PlayerStatistics.Instance;

        UpdateHealth();
    }

    private void UpdateHealth()
    {
        if (healthBar == null || healthText == null) return;

        float health = statistics.Health.GetHealth();
        float maxHealth = statistics.Health.GetMaxHealth();

        healthText.text = $"{health:F0}/{maxHealth:F0}";
        healthBar.fillAmount = health / maxHealth;
    }

    public void SetInventoryState(bool state)
    {
        inventoryHolder.alpha = state ? 1f : 0f;
        inventoryHolder.interactable = state;
        inventoryHolder.blocksRaycasts = state;
    }

    public bool GetInventoryState()
    {
        return inventoryHolder.alpha > 0f;
    }
}