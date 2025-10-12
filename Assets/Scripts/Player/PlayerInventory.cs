using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance;

    [SerializeField] private int rows = 4;
    [SerializeField] private int columns = 6;
    [SerializeField] private PlayerUIManager UIManager;

    private ItemInstance[,] items;
    private int activeHotbarSlot;
    public bool inventoryOpen;

    private void Awake()
    {
        Application.targetFrameRate = 200;
        
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        items = new ItemInstance[rows, columns];
    }

    private void Start()
    {
        if (UIManager == null) UIManager = PlayerUIManager.Instance;
        SwitchToSlot(1);
    }

    private void Update()
    {
        if (Input.GetButtonDown("Inventory")) ToggleInventory();
        if (Input.GetKeyDown(KeyCode.Q)) DropActiveItem();

        for (int i = 1; i <= columns; i++)
            if (Input.GetKeyDown(KeyCode.Alpha0 + i))
                SwitchToSlot(i);
    }

    private void ToggleInventory()
    {
        inventoryOpen = !inventoryOpen;
        PlayerUIManager.Instance.inventory.GetComponent<Canvas>().enabled = inventoryOpen;

        if (inventoryOpen) CursorManager.UnlockCursor();
        else CursorManager.LockCursor();
        
        PlayerMovement.Instance.canLook = !inventoryOpen;
    }

    public void AddItem(ItemInstance newItem)
    {
        TryStackItem(newItem);
        if (newItem.stackAmount > 0) TryFindEmptySlot(newItem);
    }

    private void TryStackItem(ItemInstance newItem)
    {
        for (int row = rows - 1; row >= 0; row--)
        {
            for (int col = 0; col < columns; col++)
            {
                var slot = items[row, col];
                if (slot == null || slot.data != newItem.data || !slot.data.Stackable) continue;
                if (slot.stackAmount >= slot.data.MaxStack) continue;

                int canAdd = Mathf.Min(newItem.stackAmount, slot.data.MaxStack - slot.stackAmount);
                slot.stackAmount += canAdd;
                newItem.stackAmount -= canAdd;

                if (newItem.stackAmount <= 0)
                {
                    RefreshUI();
                    return;
                }
            }
        }
    }

    private void TryFindEmptySlot(ItemInstance newItem)
    {
        for (int row = rows - 1; row >= 0; row--)
        {
            for (int col = 0; col < columns; col++)
            {
                if (items[row, col] == null)
                {
                    PlaceItem(newItem, row, col);
                    RefreshUI();
                    return;
                }
            }
        }
    }

    private void PlaceItem(ItemInstance item, int row, int column)
    {
        items[row, column] = item;
        UpdateSlotUI(row, column, item);
    }

    private void SwitchToSlot(int slot)
    {
        activeHotbarSlot = slot - 1;
        for (int i = 0; i < UIManager.hotbarSlots.Count; i++)
            UIManager.hotbarSlots[i].SetActive(i == activeHotbarSlot);
    }

    public void RemoveItem(int row, int column)
    {
        items[row, column] = null;
        UpdateSlotUI(row, column, null);
        RefreshUI();
    }

    public void RemoveItemByID(Guid id)
    {
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                if (items[r, c] != null && items[r, c].id == id)
                {
                    RemoveItem(r, c);
                    return;
                }
            }
        }
    }

    public void RemoveHeldItem()
    {
        RemoveItem(rows - 1, activeHotbarSlot);
    }

    private void UpdateSlotUI(int row, int column, ItemInstance item)
    {
        int index = row * columns + column;
        if (index < UIManager.inventorySlots.Count)
            UIManager.inventorySlots[index].SetItem(item);

        if (row == rows - 1 && column < UIManager.hotbarSlots.Count)
            UIManager.hotbarSlots[column].SetItem(item);
    }

    private void RefreshUI()
    {
        RefreshHotbar();
        RefreshInventory();
    }

    public void RefreshHotbar()
    {
        int row = rows - 1;
        for (int col = 0; col < columns; col++)
            if (col < UIManager.hotbarSlots.Count)
                UIManager.hotbarSlots[col].SetItem(items[row, col]);
    }

    public void RefreshInventory()
    {
        for (int r = 0; r < rows; r++)
            for (int c = 0; c < columns; c++)
                UpdateSlotUI(r, c, items[r, c]);
    }

    public void SwapItems(InventorySlot slotA, InventorySlot slotB)
    {
        int indexA = UIManager.inventorySlots.IndexOf(slotA);
        int indexB = UIManager.inventorySlots.IndexOf(slotB);

        int rowA = indexA / columns;
        int colA = indexA % columns;
        int rowB = indexB / columns;
        int colB = indexB % columns;

        (items[rowA, colA], items[rowB, colB]) = (items[rowB, colB], items[rowA, colA]);
    }

    public ItemInstance GetItem(int row, int column)
    {
        if (row < 0 || row >= rows || column < 0 || column >= columns) return null;
        return items[row, column];
    }

    public ItemInstance GetActiveItem()
    {
        return items[rows - 1, activeHotbarSlot];
    }

    public InventorySlot GetInventorySlot(int row, int column)
    {
        if (activeHotbarSlot < 0 || activeHotbarSlot >= UIManager.hotbarSlots.Count) return null;
        return UIManager.inventorySlots[row * columns + column];
    }

    private void DropActiveItem()
    {
        DropItem(GetActiveItem());
    }

    public void DropItem(ItemInstance item)
    {
        if (item == null) return;

        Transform o = PlayerMovement.Instance.orientation;
        Vector3 dropFrom = o.position + o.forward * 2f;

        GameObject dropped = Instantiate(item.data.floorPrefab, dropFrom, Quaternion.identity);
        dropped.GetComponent<Rigidbody>().AddForce(o.forward * 5f + Vector3.up * 1.5f, ForceMode.Impulse);
        dropped.GetComponent<DroppedItem>().Initialize(item);

        RemoveItemByID(item.id);
    }
}