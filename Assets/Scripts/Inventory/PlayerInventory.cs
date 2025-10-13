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

    private int activeHotbarSlot;
    public bool inventoryOpen;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        InitializeInventory();
    }

    private void Start()
    {
        UIManager = PlayerUIManager.Instance;
        SwitchToHotbarSlot(0);
    }

    private void Update()
    {
        if (Input.GetButtonDown("Inventory")) ToggleInventory();
        if (Input.GetKeyDown(KeyCode.Q)) DropActiveItem();

        for (int i = 1; i <= columns; i++)
            if (Input.GetKeyDown(KeyCode.Alpha0 + i))
                SwitchToHotbarSlot(i - 1);
    }

    private void ToggleInventory()
    {
        inventoryOpen = !inventoryOpen;
        PlayerUIManager.Instance.inventory.GetComponent<Canvas>().enabled = inventoryOpen;

        if (inventoryOpen)
            CursorManager.UnlockCursor();
        else
            CursorManager.LockCursor();

        PlayerMovement.Instance.canLook = !inventoryOpen;
    }

    private void InitializeInventory()
    {
        grid = new ItemInstance[rows, columns];
    }

    private void SwitchToHotbarSlot(int slot)
    {
        activeHotbarSlot = slot;
        
        for(int i = 0; i < UIManager.hotbarSlots.Count; i++)
            UIManager.hotbarSlots[i].SetActive(i == activeHotbarSlot);
    }
    
    public ItemInstance GetItem(int row, int col)
    {
        if (row < 0 || row >= rows || col < 0 || col >= columns) return null;
        return grid[row, col];
    }

    #region Appending to the inventory
    
    public void AddItem(ItemInstance item)
    {
        if (TryStackItem(item)) return;
        TryAddToEmptySlot(item);
    }

    private bool TryStackItem(ItemInstance newItem)
    {
        // first check the hotbar, then the rest
        for (int row = HotbarRow; row >= 0; row--)
        {
            for (int col = 0; col < columns; col++)
            {
                ItemInstance existingItem = grid[row, col];

                if (!CanStack(existingItem, newItem)) continue;

                int space = existingItem.data.MaxStack - existingItem.stackAmount;
                int toAdd = Mathf.Min(newItem.stackAmount, space);

                existingItem.stackAmount += toAdd;
                newItem.stackAmount -= toAdd;
                
                UpdateSlotUI(row,col);

                if (newItem.stackAmount <= 0) return true;
            }
        }

        return false;
    }

    private void TryAddToEmptySlot(ItemInstance item)
    {
        // again, check the hotbar first, then the rest
        for (int row = rows - 1; row >= 0; row--)
        {
            for (int col = 0; col < columns; col++)
            {
                if (grid[row, col] == null)
                {
                    SetItem(item, row, col);
                    return;
                }
            }
        }
    }
    
    private bool CanStack(ItemInstance a, ItemInstance b)
    {
        return a != null && b != null && a.data == b.data && a.data.Stackable && a.stackAmount < a.data.MaxStack;
    }

    #endregion

    #region Modifying the inventory
    
    private void SetItem(ItemInstance item, int row, int col)
    {
        grid[row, col] = item;
        UpdateSlotUI(row, col);
    }

    public void SwapSlots(InventorySlot from, InventorySlot to)
    {
        int fromIndex = UIManager.inventorySlots.IndexOf(from);
        int toIndex = UIManager.inventorySlots.IndexOf(to);

        int fromRow = fromIndex / columns;
        int fromCol = fromIndex % columns;
        int toRow = toIndex / columns;
        int toCol = toIndex % columns;

        (grid[fromRow, fromCol], grid[toRow, toCol]) = (grid[toRow, toCol], grid[fromRow, fromCol]);
        
        UpdateSlotUI(fromRow, fromCol);
        UpdateSlotUI(toRow, toCol);
    }

    public void RemoveItem(int row, int col)
    {
        SetItem(null, row, col);
    }

    public void RemoveItemByID(Guid id)
    {
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                if (grid[row, col]?.id == id)
                {
                    RemoveItem(row, col);
                    return;
                }
            }
        }
    }

    private void DropActiveItem()
    {
        DropItem(ActiveItem);
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
    
    #endregion
    
    private void UpdateSlotUI(int row, int col)
    {
        int index = row * columns + col;
        var item = grid[row, col];

        if (index < UIManager.inventorySlots.Count)
            UIManager.inventorySlots[index].SetItem(item);

        if (row == HotbarRow && col < UIManager.hotbarSlots.Count)
            UIManager.hotbarSlots[col].SetItem(item);
    }

}