using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance;

    [SerializeField] private int rows = 3;
    [SerializeField] private int columns = 6;
    [SerializeField] private PlayerUIManager UIManager;

    private ItemData[,] items;
    private int activeHotbarSlot;
    private bool inventoryOpen;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        items = new ItemData[rows, columns];
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
    }

    private void HandleHotBarInput()
    {
        for (int i = 1; i <= columns; i++)
            if (Input.GetKeyDown(KeyCode.Alpha0 + i))
                SwitchToSlot(i);
    }

    public void AddItem(ItemData item)
    {
        int bottomRow = rows - 1;

        int startSlot = 0;
        int slot = startSlot;

        // was a bit hard to figure out, used AI for this :(

        do
        {
            if (items[bottomRow, slot] == null)
            {
                PlaceItem(item, bottomRow, slot);
                return;
            }

            slot = (slot + 1) % columns;
        } while (slot != startSlot);

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                if (items[r, c] == null)
                {
                    PlaceItem(item, r, c);
                    RefreshHotbar();
                    RefreshInventory();
                    return;
                }
            }
        }

        Debug.LogWarning("inventory full");
    }

    private void PlaceItem(ItemData item, int row, int column)
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

    public void RemoveHeldItem()
    {
        RemoveItem(rows - 1, activeHotbarSlot);
    }

    public void RefreshHotbar()
    {
        int row = rows - 1;
        for (int col = 0; col < columns; col++)
        {
            if(col < UIManager.hotbarSlots.Count)
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

    public ItemData GetItem(int row, int column)
    {
        if (row < 0 || row >= rows || column < 0 || column >= columns)
        {
            return null;
        }
        return items[row, column];
    }

    public ItemData GetActiveItem()
    {
        ItemData item = items[rows - 1, activeHotbarSlot];
        
        return item;
    }

    public InventorySlot GetInventorySlot(int row, int column)
    {
        if (activeHotbarSlot < 0 || activeHotbarSlot >= UIManager.hotbarSlots.Count)
            return null;
        return UIManager.inventorySlots[row * columns + column];
    }
}