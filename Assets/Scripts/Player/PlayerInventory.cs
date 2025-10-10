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
        HandleUseInput();
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

    private void HandleUseInput()
    {
        ItemData current = items[rows - 1, activeHotbarSlot];
        if (current != null && Input.GetKeyDown(KeyCode.E))
            current.OnUse();
    }

    public void AddItem(ItemData item)
    {
        int bottomRow = rows - 1;

        int startSlot = activeHotbarSlot;
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
    }

    public void RemoveHeldItem()
    {
        RemoveItem(rows - 1, activeHotbarSlot);
    }
}