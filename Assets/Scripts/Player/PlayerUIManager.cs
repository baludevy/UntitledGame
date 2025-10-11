using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PlayerUIManager : MonoBehaviour
{
    [Header("Player UI Elements")]
    public GameObject inventory;
    public GameObject crosshair;
    public GameObject hud;
    
    [Header("HUD")]
    [SerializeField] private Image staminaBar;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private Image healthBar;
    public ObjectInfo objectInfo;
    public ItemInfo itemInfo;
    public List<HotbarSlot> hotbarSlots;
    [SerializeField] private Transform hotbarSlotsParent;
    
    [Header("Inventory")]
    [SerializeField] private Transform inventoryHolder;
    public List<InventorySlot> inventorySlots;
    
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
        
        for (int i = 0; i < inventoryHolder.childCount; i++)
        {
            Transform row = inventoryHolder.GetChild(i);
            for (int j = 0; j < row.childCount; j++)
            {
                inventorySlots.Add(row.GetChild(j).GetComponent<InventorySlot>());
            }
        }
    }

    private void Update()
    {
        UpdateStaminaBar();
        UpdateHealth();
    }

    private void UpdateStaminaBar()
    {
        staminaBar.fillAmount = PlayerStatistics.Instance.stamina / 100f;
    }

    public void UpdateHealth()
    {
        healthText.text = PlayerStatistics.Instance.health.ToString();
        healthBar.fillAmount = PlayerStatistics.Instance.health / 100f;
    }
}