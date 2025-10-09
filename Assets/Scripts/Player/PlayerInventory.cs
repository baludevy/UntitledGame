using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance;

    [Header("Inventory")]
    [SerializeField] private int slotCount = 6;
    [SerializeField] private PlayerUIManager UIManager;

    private ItemData[] equippedItems;
    private int activeSlot = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        equippedItems = new ItemData[slotCount];
    }

    private void Start()
    {
        if (UIManager == null) UIManager = PlayerUIManager.Instance;
        SwitchToSlot(1);
    }

    private void Update()
    {
        HandleSlotInput();
        HandleUseInput();
    }

    private void HandleSlotInput()
    {
        for (int i = 1; i <= slotCount; i++)
            if (Input.GetKeyDown(KeyCode.Alpha0 + i))
                SwitchToSlot(i);
    }

    private void HandleUseInput()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ItemData current = equippedItems[activeSlot];
            if (current != null)
                current.OnUse();
        }
    }

    public void AddItem(ItemData item)
    {
        equippedItems[activeSlot] = item;
        UIManager.inventorySlots[activeSlot].SetItem(item);
    }

    private void SwitchToSlot(int slot)
    {
        activeSlot = slot - 1;
        for(int i = 0; i < UIManager.inventorySlots.Count; i++)
            UIManager.inventorySlots[i].SetActive(i == activeSlot);
    }

    public void RemoveItem()
    {
        equippedItems[activeSlot] = null;
        UIManager.inventorySlots[activeSlot].Clear();
    }
}