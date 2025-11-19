using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance;
    private PlayerUIManager UIManager;

    [SerializeField] private int rows = 4;
    [SerializeField] private int columns = 6;
    private ItemInstance[,] grid;

    public int TotalSlots => rows * columns;
    public bool inventoryOpen;

    public List<ItemData> startingItems;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        grid = new ItemInstance[rows, columns];
    }

    private void Start()
    {
        UIManager = PlayerUIManager.Instance;
        AddStartingItems();
    }

    private void AddStartingItems()
    {
        if (startingItems.Count == 0) return;

        foreach (ItemData item in startingItems)
        {
            AddItem(new ItemInstance(item, 64));
        }
    }

    private void Update()
    {
        if (Input.GetButtonDown("Inventory")) ToggleInventory();
    }

    private void ToggleInventory()
    {
        inventoryOpen = !inventoryOpen;
        UIManager.SetInventoryState(inventoryOpen);
        CursorManager.SetCursorLock(!inventoryOpen);
        PlayerMovement.Instance.SetCanLook(!inventoryOpen);
    }

    public void AddItem(ItemInstance item)
    {
        if (item == null) return;

        if (!TryStackItem(item)) TryAddToEmptySlot(item);
    }

    public (int, int) GetPositionOfItem(ItemInstance item)
    {
        if (item == null) return (-1, -1);

        for (int i = 0; i < rows; i++)
        for (int j = 0; j < columns; j++)
            if (grid[i, j] == item)
                return (i, j);

        return (-1, -1);
    }

    public ItemInstance GetItem(int row, int col) =>
        (row >= 0 && row < rows && col >= 0 && col < columns) ? grid[row, col] : null;

    public ItemInstance GetArrow()
    {
        ItemInstance arrowInstance = null;

        /* for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                if (grid[row,col] != null && grid[row, col].data is Arrow)
                {
                    if(arrowInstance == null)
                        arrowInstance = grid[row, col];
                    else if(arrowInstance.stackAmount < grid[row, col].stackAmount)
                    {
                        arrowInstance = grid[row, col];
                    }
                }
            }
        } */

        return arrowInstance;
    }

    private bool TryStackItem(ItemInstance newItem)
    {
        if (newItem == null) return false;

        while (newItem.stackAmount > newItem.data.MaxStack && newItem.data.Stackable)
        {
            int leftover = newItem.stackAmount - newItem.data.MaxStack;
            newItem.stackAmount = newItem.data.MaxStack;
            AddItem(new ItemInstance(newItem.data, leftover));
        }

        for (int row = rows - 1; row >= 0; row--)
        for (int col = 0; col < columns; col++)
        {
            var existingItem = grid[row, col];
            if (existingItem == null || existingItem.data != newItem.data || !existingItem.data.Stackable ||
                existingItem.stackAmount >= existingItem.data.MaxStack)
                continue;

            int toAdd = Mathf.Min(newItem.stackAmount, existingItem.data.MaxStack - existingItem.stackAmount);
            if (toAdd > 0)
            {
                AddAmountToItem(existingItem, toAdd);
                newItem.stackAmount -= toAdd;
                if (newItem.stackAmount <= 0) return true;
            }
        }

        return false;
    }

    public void TryAddToEmptySlot(ItemInstance item)
    {
        if (item == null) return;

        for (int row = rows - 1; row >= 0; row--)
        for (int col = 0; col < columns; col++)
            if (grid[row, col] == null)
            {
                SetItemStrict(item, row, col);
                return;
            }
    }

    public void SetItemStrict(ItemInstance item, int row, int col)
    {
        grid[row, col] = item;
        UpdateSlotUI(row, col);
    }

    public void SetItem(ItemInstance item, int row, int col)
    {
        var target = grid[row, col];

        if (target == null)
        {
            grid[row, col] = item;
        }
        else if (CanMergeItem(item, target))
        {
            AddAmountToItem(target, item.stackAmount);
        }

        UpdateSlotUI(row, col);
    }


    public void SwapItems(ItemInstance newItem, InventorySlot fromSlot, InventorySlot toSlot)
    {
        if (newItem == null) return;

        var targetItem = grid[toSlot.row, toSlot.col];

        if (CanMergeItem(newItem, targetItem))
        {
            AddAmountToItem(targetItem, newItem.stackAmount);
            grid[fromSlot.row, fromSlot.col] = null;
            fromSlot.Clear();
        }
        else
        {
            grid[fromSlot.row, fromSlot.col] = targetItem;
            grid[toSlot.row, toSlot.col] = newItem;
        }

        UpdateSlotUI(fromSlot.row, fromSlot.col);
        UpdateSlotUI(toSlot.row, toSlot.col);
    }


    public void RemoveItem(int row, int col)
    {
        SetItemStrict(null, row, col);
    }

    public void DropItem(ItemInstance item, bool removeFromInventory = true)
    {
        if (item == null) return;

        var cam = PlayerCamera.Instance.transform;

        var drop = Instantiate(item.data.floorPrefab, cam.position + cam.forward * 1.5f,
            Quaternion.LookRotation(-cam.forward) * Quaternion.Euler(0, 90, 0)).GetComponent<DroppedItem>();
        drop.GetComponent<Rigidbody>().AddForce(cam.forward * 5f + Vector3.up * 1.5f, ForceMode.Impulse);
        drop.Initialize(item, true, PlayerMovement.Instance.GetComponent<Collider>());

        if (removeFromInventory) RemoveItemByID(item.id);
    }

    public void RemoveItemByID(Guid id)
    {
        for (int row = 0; row < rows; row++)
        for (int col = 0; col < columns; col++)
            if (grid[row, col]?.id == id)
            {
                RemoveItem(row, col);
                return;
            }
    }

    public void UpdateSlotUI(int row, int col)
    {
        int index = row * columns + col;

        if (index < UIManager.inventorySlots.Count)
        {
            UIManager.inventorySlots[index].SetItem(grid[row, col]);
        }
    }

    public void AddAmountToItem(ItemInstance item, int amount)
    {
        if (item == null || item.stackAmount >= item.data.MaxStack) return;

        int toAdd = Mathf.Clamp(amount, 0, item.data.MaxStack - item.stackAmount);
        if (toAdd <= 0) return;

        item.stackAmount += toAdd;

        var (row, col) = GetPositionOfItem(item);
        UpdateSlotUI(row, col);
    }

    public void SubtractAmountFromItem(ItemInstance item, int amount)
    {
        if (item == null || item.stackAmount <= 0) return;

        int toSubtract = Mathf.Clamp(amount, 0, item.stackAmount);
        item.stackAmount -= toSubtract;

        if (item.stackAmount <= 0)
        {
            RemoveItemByID(item.id);
            return;
        }

        var (row, col) = GetPositionOfItem(item);
        UpdateSlotUI(row, col);
    }

    public bool CanMergeItem(ItemInstance item1, ItemInstance item2)
    {
        if (item1 == null || item2 == null) return false;

        if (item1.data == item2.data)
        {
            if (item1.data.Stackable && item2.data.Stackable)
            {
                if (item1.stackAmount + item2.stackAmount <= item2.data.MaxStack) return true;
            }
        }

        return false;
    }

    public bool HasItem(string itemName)
    {
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                if (grid[row, col] == null)
                    continue;

                ItemData itemData = grid[row, col].data;
                if (itemData != null && itemData.Name == itemName)
                    return true;
            }
        }

        return false;
    }
}