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
    private ItemInstance ActiveItem => grid[HotbarRow, activeHotbarSlot];
    public int TotalSlots => rows * columns;
    public int activeHotbarSlot;
    public bool inventoryOpen;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

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

        if (Input.GetButtonDown("Inventory")) ToggleInventory();
        if (Input.GetKeyDown(KeyCode.Q)) DropActiveItem();

        for (int i = 0; i < columns; i++)
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                SwitchToHotbarSlot(i);
    }

    private void ToggleInventory()
    {
        inventoryOpen = !inventoryOpen;
        UIManager.inventory.GetComponent<Canvas>().enabled = inventoryOpen;
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
        if (!TryStackItem(item)) TryAddToEmptySlot(item);
    }

    private bool TryStackItem(ItemInstance newItem)
    {
        for (int row = HotbarRow; row >= 0; row--)
        for (int col = 0; col < columns; col++)
        {
            var existingItem = grid[row, col];
            if (existingItem == null || existingItem.data != newItem.data || !existingItem.data.Stackable ||
                existingItem.stackAmount >= existingItem.data.MaxStack)
                continue;

            int toAdd = Mathf.Min(newItem.stackAmount, existingItem.data.MaxStack - existingItem.stackAmount);
            existingItem.stackAmount += toAdd;
            newItem.stackAmount -= toAdd;
            UpdateHotbarUI(row, col);
            if (newItem.stackAmount <= 0) return true;
        }

        return false;
    }

    private void TryAddToEmptySlot(ItemInstance item)
    {
        for (int row = HotbarRow; row >= 0; row--)
        for (int col = 0; col < columns; col++)
            if (grid[row, col] == null)
            {
                SetItem(item, row, col);
                return;
            }
    }

    public void SetItem(ItemInstance item, int row, int col)
    {
        grid[row, col] = item;
        HeldItemController.Instance.UpdateHeldItem(grid[HotbarRow, activeHotbarSlot]);
        UpdateHotbarUI(row, col);
    }

    public void SwapItems(ItemInstance newItem, InventorySlot fromSlot, InventorySlot toSlot)
    {
        if (toSlot.item != null && toSlot.item.data == newItem.data)
        {
            fromSlot.Clear();
            grid[fromSlot.row, fromSlot.col] = null;
            toSlot.item.stackAmount += newItem.stackAmount;
        }
        else
        {
            grid[fromSlot.row, fromSlot.col] = grid[toSlot.row, toSlot.col];
            grid[toSlot.row, toSlot.col] = newItem;

            fromSlot.SetItem(toSlot.item);
            toSlot.SetItem(newItem);
        }

        if (fromSlot.row == HotbarRow)
        {
            UpdateHotbarUI(fromSlot.row, fromSlot.col);
        }

        if (toSlot.row == HotbarRow)
        {
            UpdateHotbarUI(toSlot.row, toSlot.col);
        }
    }

    public void RemoveItem(int row, int col)
    {
        SetItem(null, row, col);

        if (row != HotbarRow) return;
        GetHotbarSlot(col).Clear();
        if (activeHotbarSlot == col)
            HeldItemController.Instance.UpdateHeldItem(null);
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

    public void DropItem(ItemInstance item)
    {
        if (item == null) return;

        var cam = PlayerCamera.Instance.transform;

        var drop = Instantiate(item.data.floorPrefab, cam.position + cam.forward * 1.5f,
            Quaternion.LookRotation(-cam.forward) * Quaternion.Euler(0, 90, 0)).GetComponent<DroppedItem>();
        drop.GetComponent<Rigidbody>().AddForce(cam.forward * 5f + Vector3.up * 1.5f, ForceMode.Impulse);
        drop.Initialize(item, PlayerMovement.Instance.GetComponent<Collider>());

        RemoveItemByID(item.id);
    }

    private void UpdateHotbarUI(int row, int col)
    {
        int index = row * columns + col;
        
        if (index < UIManager.inventorySlots.Count)
            UIManager.inventorySlots[index].SetItem(grid[row, col]);
        
        if (row == HotbarRow && col < UIManager.hotbarSlots.Count)
            UIManager.hotbarSlots[col].SetItem(grid[row, col]);
    }
}