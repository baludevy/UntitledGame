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
    
    [Header("Stats")]
    [SerializeField] private Image staminaBar;
    // [SerializeField] private TMP_Text healthText;
    [SerializeField] private Image healthBar;
    
    [Header("Looking at type shit Info")]
    public ObjectInfo objectInfo;
    public ItemInfo itemInfo;
    
    [Header("Hotbar")]
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
                InventorySlot slot = row.GetChild(j).GetComponent<InventorySlot>();

                slot.row = i;
                slot.col = j;
                
                inventorySlots.Add(slot);
            }
        }
    }

    private void Update()
    {
        UpdateStaminaBar();
        UpdateHealth();
        UpdateHudInfo();
    }
    
    private void UpdateHudInfo()
    {
        LayerMask mask = LayerMask.GetMask("DroppedItem", "Mineable");
        
        if (Physics.Raycast(PlayerCamera.GetRay(), out RaycastHit hit, 5f, mask))
        {
            if (hit.collider.CompareTag("Mineable"))
            {
                IMineable obj = hit.collider.GetComponent<IMineable>();

                objectInfo.SetState(true);
                objectInfo.SetObject(obj);
            }
            else if (hit.collider.CompareTag("Item"))
            {
                DroppedItem item = hit.collider.GetComponent<DroppedItem>();

                itemInfo.SetState(true);
                itemInfo.SetItem(item.itemInstance);
            }
            else
            {
                objectInfo.SetState(false);
                itemInfo.SetState(false);
            }
        }
        else
        {
            objectInfo.SetState(false);
            itemInfo.SetState(false);
        }
    }

    private void UpdateStaminaBar()
    {
        staminaBar.fillAmount = PlayerStatistics.Instance.stamina / 100f;
    }

    public void UpdateHealth()
    {
        // healthText.text = PlayerStatistics.Instance.health.ToString();
        healthBar.fillAmount = PlayerStatistics.Instance.health / 100f;
    }
}