using System;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance;
    private PlayerUIManager UIManager;

    [SerializeField] private int rows = 4;
    [SerializeField] private int columns = 6;
    private ItemInstance[,] grid;

    private int HotbarRow => rows - 1;
    public ItemInstance ActiveItem => grid[HotbarRow, activeHotbarSlot];
    public int TotalSlots => rows * columns;
    public int activeHotbarSlot;
    public bool inventoryOpen;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        grid = new ItemInstance[rows, columns];
    }

    private void Start()
    {
        UIManager = PlayerUIManager.Instance;
        SwitchToHotbarSlot(0);
    }

    private void Update()
    {
        HeldItemController.Instance.UpdateHeldItem(ActiveItem);

        if (Input.GetButtonDown("Inventory") && !PlayerUIManager.Instance.containerOpen) ToggleInventory();
        if (Input.GetKeyDown(KeyCode.Q)) DropActiveItem();

        for (int i = 0; i < columns; i++)
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                SwitchToHotbarSlot(i);
    }

    private void ToggleInventory()
    {
        inventoryOpen = !inventoryOpen;
        UIManager.SetInventoryState(inventoryOpen);
        CursorManager.SetCursorLock(!inventoryOpen);
        PlayerMovement.Instance.canLook = !inventoryOpen;
    }

    private void SwitchToHotbarSlot(int slot)
    {
        activeHotbarSlot = slot;
        for (int i = 0; i < UIManager.hotbarSlots.Count; i++)
            UIManager.hotbarSlots[i].SetActive(i == activeHotbarSlot);
    }

    private HotbarSlot GetHotbarSlot(int index) => UIManager.hotbarSlots[index].GetComponent<HotbarSlot>();
    public HotbarSlot GetActiveHotbarSlot() => GetHotbarSlot(activeHotbarSlot);

    public void AddItem(ItemInstance item)
    {
        if (item == null)
        {
            Debug.Log("a");
            return;
        }

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

    private bool TryStackItem(ItemInstance newItem)
    {
        if (newItem == null) return false;

        // add new iteminstances until the leftover is under the item's stack cap
        while (newItem.stackAmount > newItem.data.MaxStack && newItem.data.Stackable)
        {
            int leftover = newItem.stackAmount - newItem.data.MaxStack;

            newItem.stackAmount = newItem.data.MaxStack;

            AddItem(new ItemInstance(newItem.data, leftover));
        }

        for (int row = HotbarRow; row >= 0; row--)
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

        for (int row = HotbarRow; row >= 0; row--)
        for (int col = 0; col < columns; col++)
            if (grid[row, col] == null)
            {
                SetItemStrict(item, row, col);
                return;
            }
    }

    // this sets the slot's item strictly, without trying to merge, overwriting the existing item
    public void SetItemStrict(ItemInstance item, int row, int col)
    {
        grid[row, col] = item;

        UpdateSlotUI(row, col);
    }

    public void SetItem(ItemInstance item, int row, int col)
    {
        // the existing item if there is any
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

        // the item we're swapping to the newitem
        var targetItem = grid[toSlot.row, toSlot.col];

        // first try merging
        if (CanMergeItem(newItem, targetItem))
        {
            AddAmountToItem(targetItem, newItem.stackAmount);
            grid[fromSlot.row, fromSlot.col] = null;
            fromSlot.Clear();
        }
        else
        {
            // couldnt merge, move the item from the target slot to the slot we dragged the item from
            grid[fromSlot.row, fromSlot.col] = targetItem;

            // move the new item to the target slot
            grid[toSlot.row, toSlot.col] = newItem;
        }

        UpdateSlotUI(fromSlot.row, fromSlot.col);
        UpdateSlotUI(toSlot.row, toSlot.col);
    }


    public void RemoveItem(int row, int col)
    {
        SetItemStrict(null, row, col);

        if (row == HotbarRow)
        {
            UpdateSlotUI(row, col);
        }
    }

    public void DropItem(ItemInstance item, bool removeFromInventory = true)
    {
        if (item == null) return;

        var cam = PlayerCamera.Instance.transform;

        var drop = Instantiate(item.data.floorPrefab, cam.position + cam.forward * 1.5f,
            Quaternion.LookRotation(-cam.forward) * Quaternion.Euler(0, 90, 0)).GetComponent<DroppedItem>();
        drop.GetComponent<Rigidbody>().AddForce(cam.forward * 5f + Vector3.up * 1.5f, ForceMode.Impulse);
        drop.Initialize(item, PlayerMovement.Instance.GetComponent<Collider>());

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

    private void DropActiveItem() => DropItem(ActiveItem);

    public void UpdateSlotUI(int row, int col)
    {
        int index = row * columns + col;


        if (index < UIManager.inventorySlots.Count)
        {
            Debug.Log("a");
            
            UIManager.inventorySlots[index].SetItem(grid[row, col]);
        }
            

        if (row == HotbarRow && col < UIManager.hotbarSlots.Count)
        {
            UIManager.hotbarSlots[col].SetItem(grid[row, col]);
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

    public (int, int) GetSize()
    {
        return (rows, columns);
    }
}