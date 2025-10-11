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
    private bool inventoryOpen;

    private void Awake()
    {
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
        HandleInventoryInput();
        HandleHotBarInput();
    }

    private void HandleInventoryInput()
    {
        if (Input.GetButtonDown("Inventory"))
        {
            inventoryOpen = !inventoryOpen;

            PlayerUIManager.Instance.inventory.GetComponent<Canvas>().enabled = inventoryOpen;

            if (inventoryOpen)
                CursorManager.UnlockCursor();
            else
                CursorManager.LockCursor();
            PlayerMovement.Instance.canLook = !inventoryOpen;
        }

        if (Input.GetKeyDown(KeyCode.Q))
            DropActiveItem();
    }

    private void HandleHotBarInput()
    {
        for (int i = 1; i <= columns; i++)
            if (Input.GetKeyDown(KeyCode.Alpha0 + i))
                SwitchToSlot(i);
    }

    public void AddItem(ItemInstance newItem)
    {
        // took some time to replace the AI generated code with code that works and thats mine
        
        for (int pass = 0; pass < 2; pass++)
        {
            for (int row = rows - 1; row >= 0; row--)
            {
                for (int col = 0; col < columns; col++)
                {
                    var slot = items[row, col];

                    if (pass == 0)
                    {
                        if (slot != null && slot.data == newItem.data && slot.data.Stackable &&
                            slot.stack < slot.data.MaxStack)
                        {
                            int canAdd = Mathf.Min(newItem.stack, slot.data.MaxStack - slot.stack);
                            slot.stack += canAdd;
                            newItem.stack -= canAdd;
                            if (newItem.stack <= 0)
                            {
                                RefreshHotbar();
                                RefreshInventory();
                                return;
                            }
                        }
                    }
                    else
                    {
                        if (slot == null)
                        {
                            PlaceItem(newItem, row, col);
                            RefreshHotbar();
                            RefreshInventory();
                            return;
                        }
                    }
                }
            }
        }
    }


    private void PlaceItem(ItemInstance item, int row, int column)
    {
        items[row, column] = item;

        int invIndex = row * columns + column;
        if (invIndex < UIManager.inventorySlots.Count)
            UIManager.inventorySlots[invIndex].SetItem(item);

        if (row == rows - 1 && column < UIManager.hotbarSlots.Count)
            UIManager.hotbarSlots[column].SetItem(item);
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

        int invIndex = row * columns + column;
        if (invIndex < UIManager.inventorySlots.Count)
            UIManager.inventorySlots[invIndex].Clear();

        if (row == rows - 1 && column < UIManager.hotbarSlots.Count)
            UIManager.hotbarSlots[column].Clear();

        RefreshHotbar();
        RefreshInventory();
    }

    public void RemoveItemByID(Guid id)
    {
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                ItemInstance item = items[r, c];
                if (item != null && item.id == id)
                {
                    RemoveItem(r, c);
                }
            }
        }
    }

    public void UseActiveItem(int amount = 1)
    {
        ItemInstance item = GetActiveItem();
        if (item == null) return;

        item.stack -= amount;
        item.OnUse();

        if (item.stack <= 0)
            RemoveHeldItem();

        RefreshHotbar();
        RefreshInventory();
    }

    public void RemoveHeldItem()
    {
        RemoveItem(rows - 1, activeHotbarSlot);
    }

    public void RefreshHotbar()
    {
        int row = rows - 1;
        for (int col = 0; col < columns; col++)
        {
            if (col < UIManager.hotbarSlots.Count)
                UIManager.hotbarSlots[col].SetItem(items[row, col]);
        }
    }

    public void RefreshInventory()
    {
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                int index = r * columns + c;
                if (index < UIManager.inventorySlots.Count)
                    UIManager.inventorySlots[index].SetItem(items[r, c]);
            }
        }
    }

    // ai
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
        if (row < 0 || row >= rows || column < 0 || column >= columns)
        {
            return null;
        }

        return items[row, column];
    }

    public ItemInstance GetActiveItem()
    {
        ItemInstance item = items[rows - 1, activeHotbarSlot];

        return item;
    }

    public InventorySlot GetInventorySlot(int row, int column)
    {
        if (activeHotbarSlot < 0 || activeHotbarSlot >= UIManager.hotbarSlots.Count)
            return null;
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