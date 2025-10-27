using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PlayerUIManager : MonoBehaviour
{
    [Header("HUD")] [Header("Stats")] [SerializeField]
    private TMP_Text staminaText;

    [SerializeField] private Image staminaBar;

    [SerializeField] private TMP_Text healthText;
    [SerializeField] private Image healthBar;

    [Header("Hotbar")] public List<HotbarSlot> hotbarSlots;
    [SerializeField] private Transform hotbarSlotsParent;

    [Header("Inventory")] [SerializeField] public CanvasGroup inventoryHolder;
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

        hotbarSlots = new List<HotbarSlot>();
        inventorySlots = new List<InventorySlot>();

        for (int i = 0; i < hotbarSlotsParent.childCount; i++)
        {
            hotbarSlots.Add(hotbarSlotsParent.GetChild(i).GetComponent<HotbarSlot>());
        }

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
        if(statistics == null)
            statistics = PlayerStatistics.Instance;
            
        UpdateStamina();
        UpdateHealth();
    }


    private void UpdateStamina()
    {
        if (staminaBar == null || staminaText == null)
        {
            Debug.LogWarning("No stamina bar or text assigned");
            return;
        }

        float stamina = statistics.Stamina.GetStamina();
        float maxStamina = statistics.Stamina.GetMaxStamina();
        
        staminaText.text = $"{stamina:F0}/{stamina:F0}";
        staminaBar.fillAmount = stamina / maxStamina;
    }

    private void UpdateHealth()
    {
        if (healthBar == null || healthText == null)
        {
            Debug.LogWarning("No health bar or text assigned");
            return;
        }

        float stamina = statistics.Stamina.GetStamina();
        float maxStamina = statistics.Stamina.GetMaxStamina();
        
        healthText.text = $"{stamina:F0}/{stamina:F0}";
        healthBar.fillAmount = stamina / maxStamina;
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